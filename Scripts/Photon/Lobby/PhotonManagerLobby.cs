using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PhotonManagerLobby : MonoBehaviourPunCallbacks
{
    public static PhotonManagerLobby instance = null;
    public static PhotonManagerLobby Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 500;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.JoinOrCreateRoom("Lobby", ro, null);
    }

    public GameObject loading;
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        GameObject playerCharacter;

        Debug.Log($"In Room = {PhotonNetwork.InRoom}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 캐릭터 배치 코드
        Transform[] points = GameObject.Find("RespawnSpot").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);
        playerCharacter = PhotonNetwork.Instantiate("PlayerECM 2", points[idx].position, points[idx].rotation, 0);


        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("플레이어 수 확인 : "+players.Length);

        loading.SetActive(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("입장실패");
    }

    public void DisconnectFromServer()
    {
        // Photon 서버와의 연결 해제
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 연결이 해제되면 호출될 콜백 함수
        Debug.Log("연결이 해제되었습니다. 원인: " + cause.ToString());
    }
    public void GoHome()
    {
        PhotonNetwork.LeaveRoom();
    }
}
