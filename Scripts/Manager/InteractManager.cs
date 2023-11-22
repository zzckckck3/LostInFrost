using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractManager : MonoBehaviourPunCallbacks
{
    public static InteractManager Instance { get; private set; }
    // Unity Editor에서 나무 프리펩 할당
    public List<GameObject> treeList;
    // 나무 스폰 지역 할당
    public GameObject treeSpawnPoints;
    // 오브젝트 풀링을 위한 나무 오브젝트 큐
    private Queue<GameObject> treeQueue;
    // 랜덤 생성 나무 수량
    [SerializeField]
    private int randomTreeQty;
    // Unity Editor에서 돌 프리펩 할당
    public List<GameObject> rockList;
    // 나무 오브젝트 모아놓는 용도의 빈 게임 오브젝트
    public GameObject rockRespawn;
    // 오브젝트 풀링을 위한 돌 오브젝트 큐
    private Queue<GameObject> rockQueue;
    // 랜덤 생성 돌 수량
    [SerializeField]
    private int randomRockQty;
    public GameObject ironObject;
    // 오브젝트 풀링을 위한 철광성 오브젝트 큐
    private Queue<GameObject> ironQueue;
    // 랜덤 생성 유황 수량
    [SerializeField]
    private int randomIronQty;
    public GameObject brimstoneObject;
    // 오브젝트 풀링을 위한 유황 오브젝트 큐
    private Queue<GameObject> brimstoneQueue;
    // 랜덤 생성 나무 수량
    [SerializeField]
    private int randomBrimstoneQty;
    // 나무 리스폰 타임
    [SerializeField]
    private int treeRespawnTime;
    // 돌 리스폰 타임
    [SerializeField]
    private int rockRespawnTime;
    // 철광석 리스폰 타임
    [SerializeField]
    private int ironRespawnTime;
    // 유황 리스폰 타임
    [SerializeField]
    private int brimstoneRespawnTime;
    private PhotonView pv;
    
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
        pv = GetComponent<PhotonView>();
        // 오브젝트 풀링을 위한 게임 오브젝트 큐 할당
        treeQueue = new Queue<GameObject>();
        rockQueue = new Queue<GameObject>();
        ironQueue = new Queue<GameObject>();
        brimstoneQueue = new Queue<GameObject>();

        // 고정된 위치에 나무 생성하는 함수
        if (pv.IsMine)
        {
            pv.RPC("TreesFixSpawn", RpcTarget.All);
            // 비활성화의 게임 오브젝트 미리 생성
            pv.RPC("CreateTreeObject", RpcTarget.All);
            pv.RPC("CreateRockObject", RpcTarget.All);
            pv.RPC("CreateIronObject", RpcTarget.All);
            pv.RPC("CreateBrimstoneObject", RpcTarget.All);
        }
    }
    
    [PunRPC]
    public void CreateTreeObject()
    {
        for (int i = 0; i < randomTreeQty; i++)
        {
            // 0 ~ 3 나무는 죽은 나무로 밑동이 없음
            int random = Random.Range(0, 4);
            // 오브젝트 생성
            GameObject tree = PhotonNetwork.InstantiateRoomObject("Interact/Wood/" + treeList[random].name, Vector3.zero, Quaternion.identity);
            // 오브젝트 비활성화 후 리스폰 오브젝트 하위에 할당
            tree.transform.SetParent(treeSpawnPoints.transform);
            tree.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
            // 큐에 오브젝트 넣음
            Instance.treeQueue.Enqueue(tree);
        }
        StartCoroutine(TreesSpawn());
    }

    [PunRPC]
    public void CreateRockObject()
    {
        for (int i = 0; i < randomRockQty; i++)
        {
            // 돌 종류 랜덤으로 생성
            int random = Random.Range(0, rockList.Count);
            // 오브젝트 생성
            GameObject rock = PhotonNetwork.InstantiateRoomObject("Interact/Rock/" + rockList[random].name, Vector3.zero, Quaternion.identity);
            // 오브젝트 비활성화 후 리스폰 오브젝트 하위에 할당
            rock.transform.SetParent(rockRespawn.transform);
            rock.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
            // 큐에 오브젝트 넣음
            Instance.rockQueue.Enqueue(rock);
        }
        StartCoroutine(RocksSpawn());
    }

    [PunRPC]
    public void CreateIronObject()
    {
        for (int i = 0; i < randomIronQty; i++)
        {
            // 오브젝트 생성
            GameObject iron = PhotonNetwork.InstantiateRoomObject("Interact/Rock/" + ironObject.name, Vector3.zero, Quaternion.identity);
            // 오브젝트 비활성화 후 리스폰 오브젝트 하위에 할당
            iron.transform.SetParent(rockRespawn.transform);
            iron.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
            // 큐에 오브젝트 넣음
            Instance.ironQueue.Enqueue(iron);
        }
        StartCoroutine(IronsSpawn());
    }

    [PunRPC]
    public void CreateBrimstoneObject()
    {
        for (int i = 0; i < randomBrimstoneQty; i++)
        {
            // 오브젝트 생성
            GameObject brimstone = PhotonNetwork.InstantiateRoomObject("Interact/Rock/" + brimstoneObject.name, Vector3.zero, Quaternion.identity);
            // 오브젝트 비활성화 후 리스폰 오브젝트 하위에 할당
            brimstone.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
            brimstone.transform.SetParent(rockRespawn.transform);
            // 큐에 오브젝트 넣음
            Instance.brimstoneQueue.Enqueue(brimstone);
        }
        StartCoroutine(BrimstonesSpawn());
    }
    
    public void ReturnTree(GameObject tree)
    {
        // 다시 비활성화 후 큐에 넣음
        tree.transform.SetParent(treeSpawnPoints.transform);
        tree.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
        Instance.treeQueue.Enqueue(tree);
    }
    
    public void ReturnRock(GameObject rock)
    {
        // 다시 비활성화 후 큐에 넣음
        rock.transform.SetParent(rockRespawn.transform);
        rock.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
        Instance.rockQueue.Enqueue(rock);
    }
    
    public void ReturnIron(GameObject iron)
    {
        // 다시 비활성화 후 큐에 넣음
        iron.transform.SetParent(rockRespawn.transform);
        iron.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
        Instance.ironQueue.Enqueue(iron);
    }
    
    public void ReturnBrimstone(GameObject brimstone)
    {
        // 다시 비활성화 후 큐에 넣음
        brimstone.transform.SetParent(rockRespawn.transform);
        brimstone.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, false);
        Instance.brimstoneQueue.Enqueue(brimstone);
    }

    [PunRPC]
    public void TreesFixSpawn()
    {
        if (!pv.IsMine)
        {
            return;
        }
        // 스폰 포인트 배열 할당
        Transform[] treeSpawnTransforms = treeSpawnPoints.GetComponentsInChildren<Transform>();
        for (int i = 1; i < treeSpawnTransforms.Length; i++)
        {
            // 랜덤한 나무 선택
            int random = Random.Range(0, treeList.Count);
            // 랜덤한 각도로 회전
            float angle = Random.Range(1, 360);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            // 랜덤한 나무 생성
            GameObject tree = PhotonNetwork.InstantiateRoomObject("Interact/Wood/"+treeList[random].name, treeSpawnTransforms[i].position, rotation);
            tree.transform.SetParent(treeSpawnTransforms[i]);
        }
    }

    public bool ObjectCheck(GameObject interactObject)
    {
        // 리스폰 지역 랜덤 할당
        int randomX = Random.Range(-170, 22);
        int randomZ = Random.Range(-120, 102);
        Vector3 randomVector = new Vector3(randomX, 0, randomZ);
        // 동물의 인식 범위 안의 콜라이더 탐색 후 배열에 할당
        Collider[] colliders = Physics.OverlapSphere(randomVector, 2f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return false;
            }
        }
        // 랜덤한 각도로 회전
        float angle = Random.Range(1, 360);
        Quaternion randomRotation = Quaternion.Euler(0, angle, 0);
        interactObject.GetComponent<PhotonView>().RPC("RPCInteractSetTransform", RpcTarget.All, randomVector, randomRotation);
        return true;
    }

    public void TreesRandomSpawn()
    {
        // 큐에 나무 오브젝트가 있다면 반복
        while (Instance.treeQueue.Count > 0)
        {
            // 오브젝트 활성화
            GameObject tree = Instance.treeQueue.Dequeue();
            tree.transform.SetParent(null);
            tree.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, true);
            while (true)
            {
                // 위치에 겹치는 오브젝트가 있는지 체크
                if (ObjectCheck(tree))
                {
                    break;
                }
            }
        }
    }

    public void RocksRandomSpawn()
    {
        // 큐에 나무 오브젝트가 있다면 반복
        while (Instance.rockQueue.Count > 0)
        {
            // 오브젝트 활성화
            GameObject rock = Instance.rockQueue.Dequeue();
            rock.transform.SetParent(null);
            rock.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, true);
            while (true)
            {
                // 위치에 겹치는 오브젝트가 있는지 체크
                if (ObjectCheck(rock))
                {
                    break;
                }
            }
        }
    }

    public void IronsRandomSpawn()
    {
        // 큐에 나무 오브젝트가 있다면 반복
        while (Instance.ironQueue.Count > 0)
        {
            // 오브젝트 활성화
            GameObject iron = Instance.ironQueue.Dequeue();
            iron.transform.SetParent(null);
            iron.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, true);
            while (true)
            {
                // 위치에 겹치는 오브젝트가 있는지 체크
                if (ObjectCheck(iron))
                {
                    break;
                }
            }
        }
    }

    public void BrimstonesRandomSpawn()
    {
        // 큐에 나무 오브젝트가 있다면 반복
        while (Instance.brimstoneQueue.Count > 0)
        {
            // 오브젝트 활성화
            GameObject brimstone = Instance.brimstoneQueue.Dequeue();
            brimstone.transform.SetParent(null);
            brimstone.GetComponent<PhotonView>().RPC("RPCInteractSetActive", RpcTarget.All, true);
            while (true)
            {
                // 위치에 겹치는 오브젝트가 있는지 체크
                if (ObjectCheck(brimstone))
                {
                    break;
                }
            }
        }
    }

    public IEnumerator TreesSpawn()
    {
        while (true)
        {
            TreesRandomSpawn();
            yield return new WaitForSeconds(treeRespawnTime);
        }
    }

    public IEnumerator RocksSpawn()
    {
        while (true)
        {
            RocksRandomSpawn();
            yield return new WaitForSeconds(rockRespawnTime);
        }
    }

    public IEnumerator IronsSpawn()
    {
        while (true)
        {
            IronsRandomSpawn();
            yield return new WaitForSeconds(ironRespawnTime);
        }
    }

    public IEnumerator BrimstonesSpawn()
    {
        while (true)
        {
            BrimstonesRandomSpawn();
            yield return new WaitForSeconds(brimstoneRespawnTime);
        }
    }
}