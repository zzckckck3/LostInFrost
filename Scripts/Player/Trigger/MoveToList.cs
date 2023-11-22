using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToList : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public string myNickname;
    public GameObject myCharacter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ToListPortal"))
        {
            // 현재 씬 정보 가져오기
            string myNickname = AuthManager.Instance.userData.nickname;

            if (pv.IsMine && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.NickName.Equals(myNickname))
            {
                Debug.Log("룸 리스트로 이동 시도");
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Destroy(pv);
                PhotonNetwork.Destroy(myCharacter);
                // 게임방 리스트로
                PhotonNetwork.LoadLevel("RoomList");
            }
        }
    }
}
