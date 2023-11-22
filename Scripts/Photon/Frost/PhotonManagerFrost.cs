using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PhotonManagerFrost : MonoBehaviourPunCallbacks
{
    public static PhotonManagerFrost instance = null;
    public GameObject Player;
    private PhotonView pv;
    public static PhotonManagerFrost Instance
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

    public TextMeshProUGUI userCnt;
    public TextMeshProUGUI gameLevel;

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
            Debug.Log("인게임 입장 성공");
        }

        // 눈보라 bgm on
        BgmManager.Instance.PlayBgm2();

        userCnt.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        gameLevel.text = PhotonNetwork.CurrentRoom.CustomProperties["gameLevel"].ToString();

        string difficulty = PhotonNetwork.CurrentRoom.CustomProperties["gameLevel"].ToString();
        float difficultyFloat;
        Debug.Log(difficulty);
        if (float.TryParse(difficulty, out difficultyFloat))
        {
            // 변환 성공
            UserStatusManager.Instance.difficulty = difficultyFloat;
        }
        else
        {
            // 변환 실패
            Debug.LogError("변환할 수 없는 형식입니다.");
            difficultyFloat = 0.8f;
            UserStatusManager.Instance.difficulty = difficultyFloat;
        }
        Debug.Log("현재 난인도 : " + UserStatusManager.Instance.difficulty);
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("입장 성공");
        GameObject playerCharacter = Resources.Load<GameObject>("PlayerECM 4");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        // 리스폰 지역 할당
        while (!ObjectCheck(playerCharacter)) ;
    }
    private bool ObjectCheck(GameObject characterObject)
    {
        // 리스폰 지역 랜덤 할당
        int randomX = Random.Range(-170, 22);
        int randomZ = Random.Range(-120, 102);
        Vector3 randomVector = new Vector3(randomX, 1, randomZ);
        // 사람의 인식 범위 안의 콜라이더 탐색 후 배열에 할당
        Collider[] colliders = Physics.OverlapSphere(randomVector, characterObject.GetComponentInChildren<CapsuleCollider>().height);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return false;
            }
        }

        //animalObject.transform.position = randomVector;
        // 캐릭터 배치 코드
        GameObject setPlayerCharacter = PhotonNetwork.Instantiate("PlayerECM 4", randomVector, characterObject.transform.rotation, 0);

        return true;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        userCnt.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }

}
