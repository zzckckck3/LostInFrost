using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;
using Photon.Pun;

public class BlacksmithPopup : MonoBehaviour
{
    public string playerTag = "Player"; // 플레이어 태그
    [SerializeField]
    private bool canInteractWithThis;

    PopupOpener popupOpener;
    private int viewID;

    private void Start()
    {
        canInteractWithThis = false;
        popupOpener = KeyManager.Instance.GetComponent<PopupOpener>();
        viewID = 0;
    }

    private void OnTriggerEnter(Collider other) 
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine &&other.CompareTag(playerTag) && !canInteractWithThis)
        {
            viewID = other.GetComponent<PhotonView>().ViewID;
            transform.Find("Canvas_BlackSmith").Find("Interact_Alarm").gameObject.SetActive(true);
            canInteractWithThis = true;
            KeyManager.Instance.InteractingWithBlacksmith = true;
            KeyManager.Instance.BuildingParameter = "BlacksmithPopup";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && canInteractWithThis)
        {
            viewID = 0;
            canInteractWithThis = false;
            transform.Find("Canvas_BlackSmith").Find("Interact_Alarm").gameObject.SetActive(false);
            KeyManager.Instance.InteractingWithBlacksmith = false;
            if (KeyManager.Instance.blacksmithOn && !UserStatusManager.Instance.IsMake)
            {
                KeyManager.Instance.OpenInterface(ref KeyManager.Instance.blacksmithOn, "BlacksmithPopup");
            }
        }
    }

    private void OnDisable()
    {
        if (viewID == 0) return;
        KeyManager.Instance.InteractingWithBlacksmith = false;
        if (KeyManager.Instance.blacksmithOn && !UserStatusManager.Instance.IsMake)
        {
            KeyManager.Instance.OpenInterface(ref KeyManager.Instance.blacksmithOn, "BlacksmithPopup");
        }
        UserStatusManager.Instance.IsMake = false;
    }
}
