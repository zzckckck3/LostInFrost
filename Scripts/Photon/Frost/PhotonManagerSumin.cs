using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManagerSumin : MonoBehaviourPunCallbacks
{
    public static PhotonManagerSumin instance = null;
    public static PhotonManagerSumin Instance
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

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
            Debug.Log("인게임 입장 성공");
        }
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
        int randomX = Random.Range(10, 20);
        int randomZ = Random.Range(10, 20);
        Vector3 randomVector = new Vector3(randomX, 1, randomZ);
        // 동물의 인식 범위 안의 콜라이더 탐색 후 배열에 할당
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
}
