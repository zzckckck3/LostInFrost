using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UltimateClean;

public class PhotonManagerHome : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0f";
    public GameObject loading;
    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }
        else
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = version;
            PhotonNetwork.NickName = AuthManager.Instance.userData.nickname;


            Debug.Log(PhotonNetwork.SendRate);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 포톤 마스터 서버 연결시 콜백
    public override void OnConnectedToMaster()
    {
        // 로비에서 바로 실행 시 로그인 페이지가 없어서 테스트가 안됨 주의!
        PhotonNetwork.NickName = AuthManager.Instance.userData.nickname;

        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby(); // 포톤 로비에 접속
    }

    // 포톤 로비에 접속시 콜백
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 입장 유무: " + PhotonNetwork.InLobby);
        loading.SetActive(false);
        PopupOpenerForGuide.Instance.OpenPopup();
    }
    
    // 포톤 연결 끊기
    public void DisconnectFromServer()
    {
        // Photon 서버와의 연결 해제
        PhotonNetwork.Disconnect();
    }

    // 포톤 연결 끊겼을 때 콜백
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 연결이 해제되면 호출될 콜백 함수
        Debug.Log("연결이 해제되었습니다. 원인: " + cause.ToString());
    }
}
