using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HousePopup : MonoBehaviour
{
    public string playerTag = "Player"; // 플레이어 태그
    [SerializeField]
    private bool canInteractWithThis;

    [SerializeField]
    private float staminaChangeValue;
    [SerializeField]
    private float coldChangeValue;


    [SerializeField]
    private Collider collidePlayer;
    [SerializeField]
    private Transform outPosition;
    [SerializeField]
    private bool inHouse;
    private bool lockPosition;
    private PhotonView housePv;
    private int viewID;

    private void Start()
    {
        canInteractWithThis = false;
        inHouse = false;
        lockPosition = false;
        housePv = GetComponent<PhotonView>();
        viewID = 0;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.G)
            && canInteractWithThis
            && !inHouse
            && !UserStatusManager.Instance.IsBuilding) 
        {   
            if(!UserStatusManager.Instance.IsInHouse)
            {
                StartCoroutine(EnterHouse());
            }
            
        }
        else if (Input.GetKeyUp(KeyCode.G) && canInteractWithThis && inHouse)
        {
            StartCoroutine(ExitHouse());
        }

        if (inHouse)
        {
            UserStatusManager.Instance.Stamina -= staminaChangeValue * Time.deltaTime;
            UserStatusManager.Instance.Cold -= coldChangeValue * Time.deltaTime;
        }
        else if (!inHouse && lockPosition)
        {
            if(collidePlayer.transform.position != null)
            {
                collidePlayer.transform.position = outPosition.position;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && !canInteractWithThis)
        {
            collidePlayer = other;
            transform.Find("Canvas_House").Find("Interact_Alarm").gameObject.SetActive(true);
            canInteractWithThis = true;
            KeyManager.Instance.interactingWithHouse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && canInteractWithThis)
        {
            collidePlayer = null;
            viewID = 0;
            transform.Find("Canvas_House").Find("Interact_Alarm").gameObject.SetActive(false);
            canInteractWithThis = false;
            KeyManager.Instance.interactingWithHouse = false;
        }
    }

    

    public IEnumerator EnterHouse()
    {
        UserStatusManager.Instance.IsInHouse = true;
        canInteractWithThis = false;
        inHouse = true;
        UserStatusManager.Instance.CanMove = false;
        viewID = collidePlayer.GetComponent<PhotonView>().ViewID;

        yield return new WaitForSeconds(1f);
        housePv.RPC("SetCollAndScale", RpcTarget.All, viewID, false, 0.5f);
        canInteractWithThis = true;
        
    }

    public IEnumerator ExitHouse()
    {
        if (!lockPosition)
        {
            housePv.RPC("SetCollAndScale", RpcTarget.All, viewID, true, 1f);
        }

        canInteractWithThis = false;
        inHouse = false;
        lockPosition = true;
        UserStatusManager.Instance.CanMove = true;
        viewID = 0;

        yield return new WaitForSeconds(1f);
        UserStatusManager.Instance.IsInHouse = false;
        canInteractWithThis = true;
        lockPosition = false;
    }

    [PunRPC]
    public void SetCollAndScale(int viewID, bool coll, float scale)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView == null) return;
        GameObject tObj = targetView.gameObject;
        tObj.SetActive(coll);
        tObj.GetComponent<Collider>().enabled = coll;
        tObj.transform.localScale = Vector3.one * scale;
    }

    void OnDisable()
    {
        if (viewID == 0) return;
        housePv.RPC("SetCollAndScale", RpcTarget.All, viewID, true, 1f);
        canInteractWithThis = false;
        inHouse = false;
        lockPosition = true;
        UserStatusManager.Instance.CanMove = true;
        UserStatusManager.Instance.IsInHouse = false;
        canInteractWithThis = true;
        lockPosition = false;
    }
}
