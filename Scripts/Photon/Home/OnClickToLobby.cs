using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickToLobby : MonoBehaviour
{
    public void GotoLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
        {
            //SceneManager.LoadScene("SuminDemoScene");
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            Debug.Log("서버에 연결중");
        }
    }
}
