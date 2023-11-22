using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class GunsmithPopup : MonoBehaviour
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
        viewID = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && !canInteractWithThis)
        {
            viewID = other.GetComponent<PhotonView>().ViewID;
            transform.Find("Canvas_GunSmith").Find("Interact_Alarm").gameObject.SetActive(true);
            canInteractWithThis = true;
            KeyManager.Instance.interactingWithGunsmith = true;
            KeyManager.Instance.BuildingParameter = "GunsmithPopup";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView pv = other.gameObject.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine && other.CompareTag(playerTag) && canInteractWithThis)
        {
            viewID = 0;
            canInteractWithThis = false;
            transform.Find("Canvas_GunSmith").Find("Interact_Alarm").gameObject.SetActive(false);
            KeyManager.Instance.interactingWithGunsmith = false;
            if (KeyManager.Instance.gunsmithOn && !UserStatusManager.Instance.IsMake)
            {
                KeyManager.Instance.OpenInterface(ref KeyManager.Instance.gunsmithOn, "GunsmithPopup");
            }
        }
    }

    private void OnDisable()
    {
        if (viewID == 0) return;
        KeyManager.Instance.interactingWithGunsmith = false;
        if (KeyManager.Instance.gunsmithOn && !UserStatusManager.Instance.IsMake)
        {
            KeyManager.Instance.OpenInterface(ref KeyManager.Instance.gunsmithOn, "GunsmithPopup");
        }
        UserStatusManager.Instance.IsMake = false;
    }
}
