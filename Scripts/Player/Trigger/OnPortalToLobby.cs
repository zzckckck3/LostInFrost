using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnPortalToLobby : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ToLobbyPortal"))
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                SceneManager.LoadScene("Lobby");
            }
            else
            {
                Debug.Log("서버에 연결중");
            }
        }
    }
}
