using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnclickExit : MonoBehaviour
{
    // 게임 클리어시 방 나가기.
    public void ClearExit()
    {
        string myNickname = AuthManager.Instance.userData.nickname;
        if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.NickName.Equals(myNickname))
        {
            Debug.Log("룸 리스트로 이동 시도");
            PhotonNetwork.LeaveRoom();
            // 게임방 리스트로
            PhotonNetwork.LoadLevel("RoomList");
        }
    }
}
