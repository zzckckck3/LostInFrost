using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRoomList : MonoBehaviour
{
    public void EnterRoomList()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("RoomList");
    }
}
