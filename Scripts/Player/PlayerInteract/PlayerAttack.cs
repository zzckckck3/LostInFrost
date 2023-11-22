using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerAttack : MonoBehaviour
{
    private InventoryItem equippedItem;
    private Animator anim;
    private LayerMask attackableLayers;

    private Vector3 characterPosition;
    private Quaternion characterRotation;
    private float attackRange;
    private int attackDamage;
    private Vector3 rotatedRootPosition;
    private Vector3 rootPosition;
    private PlayerAudio playerAudio;
    private InventoryItem arrowItem;
    private InventoryItem bulletItem;
    public ItemData arrowData;
    public ItemData bulletData;

    public GameObject arrowPrefab;
    public GameObject firePrefab;
    
    public Texture2D original;
    public Texture2D aim;

    [SerializeField]
    private int gunDamage;

    private PhotonView pv;

    int animalLayer;
    int buildingLayer;
    int playerLayer;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<PlayerAudio>();
        attackableLayers = (1 << LayerMask.NameToLayer("Building")) |
                   (1 << LayerMask.NameToLayer("Animal"))|
                   (1 << LayerMask.NameToLayer("Player"));
        characterRotation = new Quaternion(0, 0, 0, 1);
        arrowItem = new InventoryItem(arrowData, 1, 1);
        bulletItem = new InventoryItem(bulletData, 1, 1);

        animalLayer = LayerMask.NameToLayer("Animal");
        buildingLayer = LayerMask.NameToLayer("Building");
        playerLayer = LayerMask.NameToLayer("Player");
        gunDamage = 50;
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) return;
        if (UserStatusManager.Instance.CanAttackReady && Input.GetKeyDown(KeyCode.Mouse1))
        {
            Cursor.SetCursor(aim, Vector2.zero, CursorMode.Auto);
            UserStatusManager.Instance.IsAttackReady = true;
        }
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.SetCursor(original, Vector2.zero, CursorMode.Auto);
            UserStatusManager.Instance.IsAttackReady = false;
        }
        if (UserStatusManager.Instance.IsAttackReady)
        {
            if (UserStatusManager.Instance.CanAttack &&Input.GetKeyDown(KeyCode.Mouse0))
            {
                UserStatusManager.Instance.IsAttack = true;
                equippedItem = InventoryManager.Instance.equippedItem;
                attackRange = 0;
                attackDamage = 0;

                if (equippedItem == null || equippedItem.itemData == null)
                {
                    attackRange = 0.7f;
                    attackDamage = 3;
                    anim.SetTrigger("Punch");
                }
                else
                {
                    ToolData toolData = (ToolData) equippedItem.itemData;
                    attackRange = toolData.toolAttackRange;
                    attackDamage = toolData.toolAttackPower;
                    if(toolData.toolType == ToolType.Spear)
                    {
                        Debug.Log("창");
                        anim.SetTrigger("SpearAttack");
                    }else if (toolData.toolType == ToolType.Bow)
                    {
                        anim.SetTrigger("Bow");
                    }else if(toolData.toolType == ToolType.Gun)
                    {
                        anim.SetTrigger("Gun");
                    }
                    else
                    {
                        anim.SetTrigger("CloseAttack");
                    }
                }
            }
        }
    }
    private void ShootGun()
    {
        UserStatusManager.Instance.IsAttack = false;
        if (InventoryManager.Instance.UseConsumableItem(bulletItem))
        {
            UseWeqpon();
            // 총알 발사 위치와 방향을 설정합니다.
            Vector3 firePosition = transform.parent.position + transform.parent.forward * 1f + Vector3.up * 1.5f;
            Quaternion fireRotation = transform.parent.rotation;


            pv.RPC("FireGunEffect", RpcTarget.All, firePosition, fireRotation);
            //GameObject fire = Instantiate(firePrefab, firePosition, fireRotation);
            playerAudio.PlaySoundByName("gunSound");
            //Destroy(fire, 2f);

            Vector3 rayStart = firePosition - Vector3.up * 0.5f;
            Vector3 rayDirection = transform.parent.forward;

            // 레이캐스트를 쏘기 전에 레이를 그려줍니다.
            Debug.DrawRay(rayStart, rayDirection * 100f, Color.red, 1f); // 빨간색으로 1초 동안 레이를 그립니다.

            // 레이캐스트를 쏩니다.
            RaycastHit hit;
            if (Physics.Raycast(firePosition - Vector3.up * 0.5f, transform.parent.forward, out hit, 100f, attackableLayers))
            {
                // 레이캐스트가 무언가와 충돌하면, 여기서 처리합니다.
                Debug.Log(hit.collider.name);
                PhotonView targetPhotonView = hit.collider.gameObject.GetPhotonView();

                // 충돌한 오브젝트에 대한 추가 처리가 필요하면 여기서 진행합니다.
                if (hit.collider.gameObject.layer == animalLayer)
                {
                    targetPhotonView.RPC("GetDamage", RpcTarget.All, attackDamage);
                }
                else if (hit.collider.gameObject.layer == buildingLayer)
                {
                    targetPhotonView.RPC("GetDamage", RpcTarget.All, attackDamage);
                }
                else if (hit.collider.gameObject.layer == playerLayer && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // 유저 status 스크립트가 필요함
                    targetPhotonView.RPC("GetDamage", RpcTarget.All, attackDamage);
                }
            }
        }
        else
        {
            playerAudio.PlaySoundByName("emptyGunSound");
        }
    }

    [PunRPC]
    void FireGunEffect(Vector3 position, Quaternion rotation)
    {
        // 총 발사 이펙트 생성
        GameObject fire = Instantiate(firePrefab, position, rotation);
        Destroy(fire, 2f);
    }

    private void ShootArrow()
    {
        UserStatusManager.Instance.IsAttack = false;
        UseWeqpon();
        if (InventoryManager.Instance.UseConsumableItem(arrowItem))
        {
            Vector3 arrowSpawnPosition = transform.parent.position + transform.parent.forward * 1f + Vector3.up;
            Quaternion arrowRotation = transform.parent.rotation;
            pv.RPC("InstantiateArrowRPC", RpcTarget.All, arrowSpawnPosition, arrowRotation);
        }
    }

    [PunRPC]
    public void InstantiateArrowRPC(Vector3 position,Quaternion rotation)
    {
        GameObject arrow = NetworkManager.Instance.inventoryPools["Arrow"].Dequeue();
        arrow.SetActive(true);
        arrow.transform.SetParent(null);
        arrow.transform.rotation = rotation;
        arrow.transform.position = position;
        arrow.GetComponent<ItemStatus>().itemQuantity = 1;
        arrow.GetComponent<ConsumableStatus>().consumableQuantity = 1;
        arrow.GetComponent<ConsumableStatus>().rotationSpeed = 0;
        arrow.GetComponent<Arrow>().arrowAudio = playerAudio;
        arrow.GetComponent<ItemStatus>().destroyTime = 15f;
        arrow.GetComponent<Arrow>().isShoot = true;
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(transform.parent.forward * 500f);
        StartCoroutine(DelayBuildStart(arrow.gameObject));
    }


    [PunRPC]
    public void InitializeArrowRPC(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.GetComponent<ItemStatus>().itemQuantity = 1;
            targetView.GetComponent<ConsumableStatus>().consumableQuantity = 1;
            targetView.GetComponent<ConsumableStatus>().rotationSpeed = 0;
            targetView.GetComponent<Arrow>().arrowAudio = playerAudio;
            targetView.GetComponent<ItemStatus>().destroyTime = 5f;
            targetView.GetComponent<Arrow>().isShoot = true;
            Rigidbody rb = targetView.GetComponent<Rigidbody>();
            rb.AddForce(transform.parent.forward * 500f);
            StartCoroutine(DelayBuildStart(targetView.gameObject));
        }
    }

    private void Attack()
    {

        characterPosition = gameObject.GetComponentInParent<Transform>().position;
        characterRotation = gameObject.GetComponentInParent<Transform>().rotation;
        rootPosition =  new Vector3(0, 0.5f, attackRange);
        rotatedRootPosition = characterRotation * rootPosition;
        Collider[] hitColliders = Physics.OverlapBox(characterPosition+rotatedRootPosition, new Vector3(0.8f, 1f, attackRange) / 2, characterRotation, attackableLayers);
        UserStatusManager.Instance.IsAttack = false;
        if(hitColliders.Length > 0)
        {
            playerAudio.PlaySoundByName("hitSound");
            UseWeqpon();
        }
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider);
            if (hitCollider.isTrigger) continue;
            PhotonView targetPhotonView = hitCollider.gameObject.GetPhotonView();
            if (hitCollider.gameObject.layer == animalLayer)
            {
                Debug.Log("동물공격");
                targetPhotonView.RPC("GetDamage", RpcTarget.All, attackDamage);
            }
            else if (hitCollider.gameObject.layer == buildingLayer)
            {
                Debug.Log("빌딩공격");
                targetPhotonView.RPC("GetDamage", RpcTarget.All, attackDamage);
            }
            else if(hitCollider.gameObject.layer == playerLayer && !hitCollider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                // 유저 status 스크립트가 필요함
                Debug.Log("유저공격");
                targetPhotonView.RPC("GetDamage", RpcTarget.All, attackDamage);
            }
        }

    }

    private void UseWeqpon()
    {
        if (equippedItem != null && equippedItem.itemData != null)
        {
            if (!InventoryManager.Instance.UseTool(equippedItem, 1))
            {
                playerAudio.PlaySoundByName("brokenSound");
            }
        }
    }

    void OnDrawGizmos()
    {
        // characterRotation이 유효하지 않은 경우에 대한 검사를 추가합니다.
        if (characterRotation.x == 0 && characterRotation.y == 0 &&
            characterRotation.z == 0 && characterRotation.w == 0)
        {
            // 유효하지 않은 경우 항등 쿼터니언으로 재설정합니다.
            characterRotation = Quaternion.identity;
        }

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(characterPosition + rotatedRootPosition, characterRotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.8f, 1f, attackRange));
    }

    private IEnumerator DelayBuildStart(GameObject arrow)
    {
        arrow.layer = LayerMask.NameToLayer("Default");
        yield return new WaitForSeconds(0.3f);
        arrow.layer = LayerMask.NameToLayer("CanRoot");

    }

}
