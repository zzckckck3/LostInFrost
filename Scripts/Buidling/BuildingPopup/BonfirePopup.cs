using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class BonfirePopup : MonoBehaviour
{
    public string playerTag = "Player"; // 플레이어 태그
    [SerializeField]
    private bool canInteractWithThis;
    private int viewID;

    PopupOpener popupOpener;

    private void Start()
    {
        canInteractWithThis = false;
        popupOpener = KeyManager.Instance.GetComponent<PopupOpener>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && !canInteractWithThis)
        {
            viewID = other.GetComponent<PhotonView>().ViewID;
            transform.Find("Canvas_Bonfire").Find("Interact_Alarm").gameObject.SetActive(true);
            canInteractWithThis = true;
            KeyManager.Instance.InteractingWithBonfire = true;
            KeyManager.Instance.BuildingParameter = "BonfirePopup";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && canInteractWithThis)
        {
            viewID = 0;
            canInteractWithThis = false;
            transform.Find("Canvas_Bonfire").Find("Interact_Alarm").gameObject.SetActive(false);
            KeyManager.Instance.InteractingWithBonfire = false;
            if (KeyManager.Instance.bonfireOn && !UserStatusManager.Instance.IsMake)
            {
                KeyManager.Instance.OpenInterface(ref KeyManager.Instance.bonfireOn, "BonfirePopup");
            }
        }
    }

    private void OnDisable()
    {
        if (viewID == 0) return;
        KeyManager.Instance.InteractingWithBonfire = false;
        if (KeyManager.Instance.bonfireOn && !UserStatusManager.Instance.IsMake)
        {
            KeyManager.Instance.OpenInterface(ref KeyManager.Instance.bonfireOn, "BonfirePopup");
        }
        UserStatusManager.Instance.IsMake = false;
    }
}
