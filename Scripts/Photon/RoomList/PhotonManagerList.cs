using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonManagerList : MonoBehaviourPunCallbacks
{
    public static PhotonManagerList instance = null;
    public static PhotonManagerList Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public GameObject RoomInfos; // 방 정보를 가진 오브젝트들을 담을 오브젝트.
    public List<GameObject> objectsWithTag; // 태그를 가진 오브젝트를 담을 리스트
    public GameObject RoomInfo; // 룸 정보를 담을 오브젝트
    // 버튼을 찾았을 경우 버튼의 색상 변경
    public Color newColor1 = new Color(0.2f, 0.6039216f, 0.9411765f, 1.0f); // 입장가능 : 푸른색
    public Color newColor2 = new Color(0.9433962f, 0.308532f, 0.3525073f, 1.0f); // 입장불가 : 붉은색
    public GameObject failUi; // 입장 실패시 안내창
    public TextMeshProUGUI failInfo; // 입장 실패시 안내문구
    
    void Start()
    {
        GameObject userStatusManager = GameObject.Find("UserStatusManager");
        Destroy(userStatusManager);
        if (PhotonNetwork.IsConnected)
        {
            OnConnectedToMaster();
        }

        // 강퇴당해서 오게된 경우
        if (AuthManager.Instance.isKicked == 1)
        {
            AuthManager.Instance.isKicked = 0;// 다시 초기화
            failUi.SetActive(true);
            failInfo.text = "방장에 의해 강퇴되었습니다ㅋㅋㅋ";
        }

        // 만약 bgm1이 꺼져있는 상태면 키기
        if (!BgmManager.Instance.IsPlayBgm1)
        {
            BgmManager.Instance.PlayBgm1();
        }
        // bgm2,3이 켜져있는 상태면 끄기
        if (BgmManager.Instance.IsPlayBgm2)
        {
            BgmManager.Instance.StopBgm2();
        }
        
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 입장 유무: " + PhotonNetwork.InLobby);
        // 플레이어 커스텀 옵션: ready 상태, 바퀴 수 초기화.
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties["IsReady"] = "0"; // 공통 CustomProperties : Ready 정보 설정
        customProperties["Level"] = AuthManager.Instance.userData.level+""; // 공통 CustomProperties : Ready 정보 설정
        customProperties["IsKicked"] = "N"; // 강퇴할 플레이어 닉네임

        PhotonNetwork.LocalPlayer.CustomProperties = customProperties;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 엡데이트된 방 개수를 출력
        Debug.Log($"업데이트방 개수: {roomList.Count}\n\n");
        // RoomInfos 오브젝트 아래에서 특정 태그를 가진 오브젝트를 찾음
        Transform[] foundObjects = RoomInfos.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("RoomInfo")).ToArray();

        // 각 방의 정보 업데이트
        foreach (RoomInfo room in roomList)
        {
            // 로비방에 대한 정보는 인원수만 파악
            if (room.Name == "Lobby")
            {
                continue;
            }

            bool isIn = false; // 현재 존재하는 방인가?
            for(int i = 0; i< foundObjects.Length; i++)
            {   // TextMesh Pro 오브젝트 배열
                TextMeshProUGUI[] textObjects = foundObjects[i].GetComponentsInChildren<TextMeshProUGUI>(true);
                // 현재 존재하는 게임방일때.
                if (room.Name.Equals(textObjects[3].text))
                {
                    isIn = true;
                    // 플레이어가 0명이면 사라진 방
                    if(room.PlayerCount == 0)
                    {
                        Destroy(foundObjects[i].gameObject);
                        break;
                    }

                    // 플레이어가 존재하는 방 정보 변경 : 플레이어 입장, 퇴장, 게임 시작 등
                    textObjects[4].text = room.PlayerCount+""; // 현재 인원
                    textObjects[7].text = room.IsOpen ? "대기중" : "게임중"; // 방 상태

                    // 상태에 따른 버튼 디자인 변경
                    textObjects[0].text = room.IsOpen ? "입장하기" : "입장불가"; // 버튼 텍스트
                    Button button = foundObjects[i].GetComponentInChildren<Button>(); // 버튼 찾기
                    // 버튼을 찾았을 경우 버튼의 색상 변경
                    button.image.color = room.IsOpen ? newColor1: newColor2;
                }
                
            }
            // 만약 존재하지 않았던 방이라면
            if (!isIn)
            {
                if (room.PlayerCount == 0) continue;
                // 프리팹을 인스턴스화
                GameObject instance = Instantiate(RoomInfo);

                // 원하는 부모 오브젝트를 찾거나 직접 지정
                Transform parent = RoomInfos.transform;

                // 부모 아래에 추가
                instance.transform.SetParent(parent);

                // 추가된 룸 정보 form에 데이터 출력
                TextMeshProUGUI[] textObjects = instance.GetComponentsInChildren<TextMeshProUGUI>(true);
                
                textObjects[2].text = room.CustomProperties["gameLevel"]+"";
                textObjects[3].text = room.Name;
                textObjects[4].text = room.PlayerCount+""; // 현재 인원
                textObjects[6].text = room.MaxPlayers+""; // 최대 정원
                textObjects[7].text = room.IsOpen ? "대기중" : "게임중"; // 방 상태
                // 방 상태에 따른 버튼 디자인 변경
                textObjects[0].text = room.IsOpen ? "입장하기" : "입장불가"; // 버튼 텍스트
                textObjects[1].text = room.Name;
                Button button = instance.GetComponentInChildren<Button>(); // 버튼 찾기
                
                button.image.color = room.IsOpen ? newColor1 : newColor2;
            }
        }
    }

    public void MakeRoom()
    {
        GameObject titleObject = GameObject.Find("TitleText");
        GameObject maxCntObject = GameObject.Find("MaxPlayerCnt");
        GameObject gameLevelObject = GameObject.Find("GameLevelText");
        TextMeshProUGUI title = titleObject.GetComponent<TextMeshProUGUI>(); // 입력한 방제
        TextMeshProUGUI maxPlayerCnt = maxCntObject.GetComponent<TextMeshProUGUI>(); // 입력한 정원
        TextMeshProUGUI gameLevel = gameLevelObject.GetComponent<TextMeshProUGUI>(); // 설정한 난이도

        // 방제 입력 확인
        if (title.text.Length == 1)
        {
            failUi.SetActive(true);
            failInfo.text = "방제를 입력하세요";
            Debug.Log("방제를 입력하세요");
            return;
        }
        if (PhotonNetwork.InLobby)
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = int.Parse(maxPlayerCnt.text);
            ro.IsOpen = true;
            ro.IsVisible = true;

            // 커스텀 속성 추가
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["captain"] = AuthManager.Instance.userData.nickname; // 방장정보 커스텀 속성
            customProperties["startTime"] = setStartDateTime(); // 시작시간 커스텀 속성
            customProperties["uuid"] = GetGUID(); // 시작시간 커스텀 속성
            customProperties["gameLevel"] = gameLevel.text;
            ro.CustomRoomProperties = customProperties;
            ro.CustomRoomPropertiesForLobby = new string[] {"captain", "startTime", "uuid", "gameLevel" }; // 로비에서 커스텀 속성을 볼 수 있도록 설정

            PhotonNetwork.JoinOrCreateRoom(title.text, ro, null);
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room!");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
        
        SceneManager.LoadScene("WaitingRoom");
    }

    public void EnterRoom(string roomTitle)
    {
        PhotonNetwork.JoinRoom(roomTitle);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 입장 실패: " + message);
        failUi.SetActive(true);
        failInfo.text = "입장 실패: " + message;
        // 입장 실패에 대한 사용자 지정 처리 추가
        PhotonNetwork.JoinLobby();
    }
    public void EnterLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public string setStartDateTime()
    {
        string startDateTime;
        // 현재 날짜와 시간 가져오기
        DateTime currentDateTime = DateTime.Now;
        // 시간 문자화.
        startDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        return startDateTime;
    }
    // GUID 생성
    public string GetGUID()
    {
        // 새로운 GUID 생성
        System.Guid uuid = System.Guid.NewGuid();
        Debug.Log("Generated UUID: " + uuid.ToString());
        return uuid.ToString();
    }

}
