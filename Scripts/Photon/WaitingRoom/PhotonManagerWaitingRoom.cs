using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.XR;

public class PhotonManagerWaitingRoom : MonoBehaviourPunCallbacks
{
    private static PhotonManagerWaitingRoom instance;

    public static PhotonManagerWaitingRoom Instance
    {
        get { return instance; }
    }


    float degree; // 배경회전 초기각도
    static Room roomInfo;  // 방정보 dto
    public TextMeshProUGUI title; // 방제목
    public TextMeshProUGUI playerCnt; // 현재 인원
    public TextMeshProUGUI maxPlayerCnt; // 최대인원
    public Transform playerListParent; // 플레이어 목록을 표시할 부모 Transform
    public GameObject playerInfo; // 플레이어 정보를 담을 ui 폼.
    public GameObject playerInfos; // 플레이어 정보를 가진 오브젝트들을 담을 오브젝트.
    public TextMeshProUGUI readyBtnText; // 레디버튼 텍스트
    public TextMeshProUGUI gameLevel; // 게임 난이도 텍트스
    public GameObject loading; // 로딩창
    void Start()
    {
        degree = 0;
        if (instance == null)
        {
            instance = this;
        }
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }

        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        degree += Time.deltaTime;
        if (degree >= 360)
            degree = 0;

        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("입장 성공");
        GameObject playerCharacter;
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Room DateTime = {PhotonNetwork.CurrentRoom.CustomProperties["startTime"]}");

        // 방 정보 업데이트
        roomInfo = PhotonNetwork.CurrentRoom;
        RoonInfoUpdate();

        // 플레이어 정보 업데이트
        UpdatePlayerList();

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 캐릭터 배치 코드
        Transform[] points = GameObject.Find("RespawnSpot").GetComponentsInChildren<Transform>();
        int idx = UnityEngine.Random.Range(0, points.Length);
        playerCharacter = PhotonNetwork.Instantiate("PlayerECM 2", points[idx].position, points[idx].rotation, 0);

        loading.SetActive(false);
    }
    // 플레이어 입장 콜백
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoonInfoUpdate();
        UpdatePlayerList();
    }

    // 플레이어 퇴장 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
        // 만약 나간 사람이 현재 방장이라면 
        if (otherPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"]))
        {
            // 다음으로 들어온 사람을 반장으로 임명.
            var firstPlayer = PhotonNetwork.CurrentRoom.Players.Values.First();
            string newCaptaionNickName = firstPlayer.NickName;

            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["captain"] = newCaptaionNickName; // 원하는 값을 여기에 넣으세요.

            // Hashtable을 사용하여 커스텀 속성을 업데이트합니다.
            PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
            // 방 정보가 변경이 완료된 후 ui 업데이트 : 코루틴
            StartCoroutine(WaitForCustomPropertiesToBeSetAndThenUpdate(newCaptaionNickName));
        }

        // 방장이 나간 경우가 아니라면 바로 호출.
        else
        {
            RoonInfoUpdate();
            UpdatePlayerList();
        }
        
    }
    // 방 정보 ui 업데이트 코루틴 : 유저 나갔을 때.
    IEnumerator WaitForCustomPropertiesToBeSetAndThenUpdate(string newCaptaionNickName)
    {
        // SetCustomProperties가 완료될 때까지 대기합니다.
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.CustomProperties["captain"].ToString() == newCaptaionNickName);

        // 대기 후 UI 업데이트 함수 호출
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["IsReady"] = "0"; // 업데이트
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        RoonInfoUpdate();
        UpdatePlayerList();
    }

    // 방 정보 업데이트
    public void RoonInfoUpdate()
    {
        roomInfo = PhotonNetwork.CurrentRoom;

        title.text = roomInfo.Name;
        playerCnt.text = roomInfo.PlayerCount.ToString();
        maxPlayerCnt.text = roomInfo.MaxPlayers.ToString();
        gameLevel.text = roomInfo.CustomProperties["gameLevel"].ToString();
    }

    // 플레이어 리스트 ui 업데이트 : 게임 시작 전
    public int UpdatePlayerList()
    {

        int readyCnt = 0;
        roomInfo = PhotonNetwork.CurrentRoom;
        Debug.Log("리스트업데이트");
        // 기존 UI 요소를 모두 삭제
        foreach (Transform child in playerListParent)
        {
            Destroy(child.gameObject);
        }

        if (PhotonNetwork.InRoom)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // 플레이어 정보 폼 프리팹을 인스턴스화
                GameObject instance = Instantiate(playerInfo);

                // 원하는 부모 오브젝트를 찾거나 직접 지정
                Transform parent = playerListParent;

                // 부모 아래에 추가
                instance.transform.SetParent(parent);
                instance.transform.localScale = new Vector3(0.93f, 1.05f, 1f);
                // 해당 ui 오브젝트에 포함된 텍스트 담기
                TextMeshProUGUI[] textObjects = instance.GetComponentsInChildren<TextMeshProUGUI>(true);
                TextMeshProUGUI playerNickname = textObjects[0];
                TextMeshProUGUI playerLevel = textObjects[2];


                // 유저 정보 입력 : 텍스트
                playerNickname.text = player.NickName; // 닉네임

                // 내 닉네임은 색표시
                if (player.NickName.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    Color newColor = new Color(97.0f / 255.0f, 255.0f / 255.0f, 66.0f / 255.0f); // 예: RGB(97, 255, 66)
                    playerNickname.color = newColor;
                }
                playerLevel.text = (string) player.CustomProperties["Level"]; // 커스텀 옵션: 레벨

                // 방장일때 아이콘 on
                bool isCap = roomInfo.CustomProperties["captain"].Equals(player.NickName);
                if (isCap)
                {
                    GameObject captainIcon = instance.transform.Find("CapIcons").gameObject;
                    captainIcon.SetActive(true);
                }

                // 일반 유저일떄 아이콘 on
                else
                {
                    GameObject playerIcon = instance.transform.Find("PlayerIcon").gameObject;
                    playerIcon.SetActive(true);
                }

                // 레디 정보 출력---------
                string isReady = (string)player.CustomProperties["IsReady"];

                // 방장일때 : 체크 표시는 기본. 시작 신호시 카운트 추가하고 continue
                // 카운트 이유 :
                // 기본적으로 ready 개수를 파악할 방법이 모든 플레이어 커스텀 정보 직접 조회
                if(isCap)
                {
                    //방장의 레디버튼 텍스트 : START
                    if (PhotonNetwork.LocalPlayer.NickName.Equals(player.NickName))
                    {
                        readyBtnText.text = "START";
                    }
                    // 방장의 레디 아이콘
                    GameObject readyIcon = instance.transform.Find("ReadyIcon").gameObject;
                    readyIcon.SetActive(true);
                    // 방장의 시작정보.
                    if(isReady.Equals("1"))
                        readyCnt++; // 카운트 추가
                    continue;
                }
                if (isReady.Equals("1")) // 방장, 레디한 경우 체크
                {
                    readyCnt++; // 카운트 추가
                    GameObject readyIcon = instance.transform.Find("ReadyIcon").gameObject;
                    readyIcon.SetActive(true);
                }

                // 만약 본인이 방장일때 플레이어 강퇴버튼 활성화
                if (PhotonNetwork.LocalPlayer.NickName.Equals(roomInfo.CustomProperties["captain"])
                    && !isCap)
                {
                    GameObject kickBtn = instance.transform.Find("KickIcon").gameObject;
                    kickBtn.SetActive(true);
                }
            }
        }
        return readyCnt;
    }
    public void Ready()
    {
        if (PhotonNetwork.InRoom)
        {
            // 방장 시작 반영
            if (PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                Debug.Log("방장의 레디");
                int readyCnt = UpdatePlayerList(); // 레디개수 파악
                if (readyCnt == PhotonNetwork.CurrentRoom.PlayerCount - 1) // 방장 빼고 모두 레디 상태
                {
                    Debug.Log("게임 시작 신호 보내기");
                    // 방장의 레디 정보를 업데이트 함으로서 콜백함수 호출.
                    ExitGames.Client.Photon.Hashtable CaptaincustomProperties = new ExitGames.Client.Photon.Hashtable();
                    CaptaincustomProperties["IsReady"] = "1"; // 업데이트
                    PhotonNetwork.LocalPlayer.SetCustomProperties(CaptaincustomProperties);
                    return;
                }
                else
                {
                    Debug.Log("아직 레디가 완료되지 않았습니다."+ readyCnt);
                    ResultInfoWaitingRoom.Instance.LaunchNotification(false, "아직 레디가 완료되지 않았습니다..!");
                    return;
                }
            }
            // 플레이어 레디 반영
            string isReady = (string)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"];
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["IsReady"] = isReady == "0" ? "1" : "0"; ; // 업데이트

            // 누른 플레이어 정보 변경
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        }
    }
    // 플레이어 커스텀 옵션 변경 감지 콜백
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 게임 대기중일 상태일 때
        if (PhotonNetwork.CurrentRoom.IsOpen)
        {
            // 방장의 강퇴 신호일때
            if(targetPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"].ToString())
                && (string)targetPlayer.CustomProperties["IsKicked"] != "N")
            {
                FindPlayerAndKick((string)targetPlayer.CustomProperties["IsKicked"]);
                return;
            }


            // 전체 레디 수
            int readyCnt = UpdatePlayerList();

            Debug.Log("레디정보 업데이트 완료");

            // 만약 반장의 시작 신호가 들어온다면 
            if (targetPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"].ToString())
                && (string)targetPlayer.CustomProperties["IsReady"] == "1" && readyCnt == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                Debug.Log("게임시작");
                PhotonNetwork.CurrentRoom.IsOpen = false; // 이제 방은 못들어오게 설정.

                // 시작 시간 업데이트.
                ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
                customProperties["startTime"] = getStartDateTime(); // 원하는 값을 여기에 넣으세요.

                // Hashtable을 사용하여 커스텀 속성을 업데이트합니다.
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

                // 게임 시작 사운드 on
                PlaneBroke.instance.StartInfoSound();
                return;
            }

            // 게임 시작이 가능한 조건일때 : 전부 ready 상태인 경우
            if (readyCnt >= PhotonNetwork.CurrentRoom.PlayerCount - 1
                && PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                Debug.Log("게임 시작 가능");
                ResultInfoWaitingRoom.Instance.LaunchNotification(true, "준비완료! 게임을 시작하세요!");
            }
        }
    }
    
    // 방장이 다른 플레이어 강퇴
    public void KickPlayer(string nickName)
    {
        if (PhotonNetwork.InRoom
            && PhotonNetwork.CurrentRoom.CustomProperties["captain"].Equals(PhotonNetwork.LocalPlayer.NickName))
        {
            Debug.Log(nickName);
            // 방장의 레디 정보를 업데이트 함으로서 콜백함수 호출.
            ExitGames.Client.Photon.Hashtable CaptaincustomProperties = new ExitGames.Client.Photon.Hashtable();
            CaptaincustomProperties["IsKicked"] = nickName; // 업데이트
            PhotonNetwork.LocalPlayer.SetCustomProperties(CaptaincustomProperties);
        }
    }

    // 강퇴할 플레이어 찾아서 강퇴
    public void FindPlayerAndKick(string nickName)
    {
        if (PhotonNetwork.LocalPlayer.NickName.Equals(nickName))
        {
            AuthManager.Instance.isKicked++;// 강퇴당함 표시.
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("RoomList");
            return;
        }
        // 강퇴 플레이어정보 초기화.
        if (PhotonNetwork.LocalPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"]))
        {
            ExitGames.Client.Photon.Hashtable CaptaincustomProperties = new ExitGames.Client.Photon.Hashtable();
            CaptaincustomProperties["IsKicked"] = "N"; // 업데이트
            PhotonNetwork.LocalPlayer.SetCustomProperties(CaptaincustomProperties);
        }
    }

    // 시작 시간 구하기
    public string getStartDateTime()
    {
        string startDateTime;
        // 현재 날짜와 시간 가져오기
        DateTime currentDateTime = DateTime.Now;
        // 시간 문자화.
        startDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        return startDateTime;
    }
}
