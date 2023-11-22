using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;

public class PhotonAnimalManager : MonoBehaviourPunCallbacks
{
    public static PhotonAnimalManager Instance { get; private set; }
    // Unity Editor에서 할당
    public List<AnimalData> animalDataList;
    // 동물 데이터 딕셔너리
    public Dictionary<string, AnimalData> animalDictionary;
    // 동물 오브젝트 풀링 큐 딕셔너리
    public Dictionary<string, Queue<GameObject>> animalPoolingObjectQueueDictionary;
    public GameObject animals;
    public PhotonView pv;
    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 동물 데이터 딕셔너리 할당
        animalDictionary = new Dictionary<string, AnimalData>();
        // 동물 오브젝트 풀링 큐 딕셔너리 할당
        animalPoolingObjectQueueDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (var animalData in animalDataList)
        {
            animalDictionary[animalData.animalName] = animalData;
            animalPoolingObjectQueueDictionary[animalData.animalName] = new Queue<GameObject>();
        // 빈 오브젝트 생성 후 동물 카테고리로 사용 오브젝트 별로 하위에 할당
            if (pv.IsMine)
            {
                pv.RPC("CreateAnimalObject", RpcTarget.All, animalData.animalName);
            }
        }
        
    }

    [PunRPC]
    public void CreateAnimalObject(String animalName)
    {
        if (!pv.IsMine) return;
        AnimalData animalData = animalDictionary[animalName];
        for (int i = 0; i < animalData.animalLimit; i++)
        {
            // 오브젝트 정보 로드 후 생성
            GameObject animal = PhotonNetwork.InstantiateRoomObject("Animal/" + animalName, Vector3.zero, Quaternion.identity);
            animal.transform.SetParent(animals.transform);
            animal.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);
            // 큐에 오브젝트 넣음
            Instance.animalPoolingObjectQueueDictionary[animalName].Enqueue(animal);
        }
        StartCoroutine(Spawn(animalName));
    }

    public void SpawnAnimal(AnimalData animalData)
    {
        // 큐에 동물 오브젝트가 있다면 반복
        while (Instance.animalPoolingObjectQueueDictionary[animalData.animalName].Count > 0)
        {
            GameObject animal = Instance.animalPoolingObjectQueueDictionary[animalData.animalName].Dequeue();
            animal.transform.SetParent(null);
            if (pv.IsMine)
            {
                animal.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, true);
            }
            while (true)
            {
                // 위치에 겹치는 오브젝트가 있는지 체크
                if (ObjectCheck(animal))
                {
                    break;
                }
            }
        }
    }

    public void ReturnObject(GameObject animal)
    {
        // 다시 비활성화 후 큐에 넣음
        // animal.transform.SetParent(animals.transform);
        animal.transform.SetParent(animals.transform);
        if (pv.IsMine)
        {
            animal.GetComponent<PhotonView>().RPC("RPCSetActive", RpcTarget.All, false);
        }
        Instance.animalPoolingObjectQueueDictionary[animal.GetComponent<PhotonAnimalAI>().animalName].Enqueue(animal);
    }

    public bool ObjectCheck(GameObject animal)
    {
        // 리스폰 지역 랜덤 할당
        int randomX = Random.Range(-170, 22);
        int randomZ = Random.Range(-120, 102);
        Vector3 randomVector = new Vector3(randomX, 0, randomZ);
        // 랜덤한 각도로 회전
        float angle = Random.Range(1, 360);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        // 동물의 인식 범위 안의 콜라이더 탐색 후 배열에 할당
        Collider[] colliders = Physics.OverlapSphere(randomVector, animal.GetComponent<CapsuleCollider>().height);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return false;
            }
        }

        if (pv.IsMine)
        {
            animal.GetComponent<PhotonView>().RPC("RPCSetTransform", RpcTarget.All, randomVector, rotation);
        }
        return true;
    }

    public IEnumerator Spawn(String animalName)
    {
        bool firstSpawn = false;
        AnimalData animalData = animalDictionary[animalName];
        while (true)
        {
            if (!firstSpawn)
            {
                firstSpawn = true;
                yield return new WaitForSeconds(animalData.animalSpawnStartTime);
            }

            SpawnAnimal(animalData);

            yield return new WaitForSeconds(animalData.animalRespawnTime);
        }

    }

}
