using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }
    // Unity Editor에서 할당
    public List<AnimalData> animalDataList; 
    // 동물 데이터 딕셔너리
    private Dictionary<string, AnimalData> animalDictionary;
    // 동물 오브젝트 풀링 큐 딕셔너리
    private Dictionary<string, Queue<GameObject>> animalPoolingObjectQueueDictionary;
    public GameObject animals;
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
            CreateAnimalObject(animalData);
            StartCoroutine(Spawn(animalData));
        }
    }

    private void CreateAnimalObject(AnimalData animalData)
    {
        for (int i = 0; i < animalData.animalLimit; i++)
        {
            // 오브젝트 정보 로드 후 생성
            GameObject animal = Instantiate(Resources.Load<GameObject>("Animal/" + animalData.animalName));
            // 오브젝트 비활성화 후 매니저 오브젝트 하위에 할당
            animal.SetActive(false);
            animal.transform.SetParent(animals.transform);
            // 큐에 오브젝트 넣음
            Instance.animalPoolingObjectQueueDictionary[animalData.animalName].Enqueue(animal);
        }
    }

    public void SpawnAnimal(String animalName)
    {
        // 큐에 동물 오브젝트가 있다면 반복
        while (Instance.animalPoolingObjectQueueDictionary[animalName].Count > 0)
        {
            GameObject animal = Instance.animalPoolingObjectQueueDictionary[animalName].Dequeue();
            animal.transform.SetParent(null);
            animal.SetActive(true);
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
        animal.SetActive(false);
        animal.transform.SetParent(animals.transform);
        Instance.animalPoolingObjectQueueDictionary[animal.GetComponent<AnimalAI>().animalName].Enqueue(animal);
    }

    private bool ObjectCheck(GameObject animal)
    {
        // 리스폰 지역 랜덤 할당
        int randomX = Random.Range(-170, 22);
        int randomZ = Random.Range(-120, 102);
        Vector3 randomVector = new Vector3(randomX, 0, randomZ);
        // 동물의 인식 범위 안의 콜라이더 탐색 후 배열에 할당
        Collider[] colliders = Physics.OverlapSphere(randomVector, animal.GetComponent<CapsuleCollider>().height);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return false;
            }
        }
        
        animal.transform.position = randomVector;
        return true;
    }

    private IEnumerator Spawn(AnimalData animalData)
    {
        bool firstSpawn = false;
        
        while (true)
        {
            if (!firstSpawn)
            {
                firstSpawn = true;
                yield return new WaitForSeconds(animalData.animalSpawnStartTime);
            }

            SpawnAnimal(animalData.animalName);
            
            yield return new WaitForSeconds(animalData.animalRespawnTime);
        }

    }
}
