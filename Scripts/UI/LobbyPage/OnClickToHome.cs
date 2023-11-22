using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickToHome : MonoBehaviour
{
    public void ClickToHome()
    {
        PhotonNetwork.LeaveRoom();
        //PhotonManagerLobby.instance.GoHome();
        PhotonNetwork.LoadLevel("Home");
    }
}
