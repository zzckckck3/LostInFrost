using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class PlayerRaycastToFront : MonoBehaviour
{
    public float raycastLength;
    public float yOffset = 2.0f;
    public LayerMask raycastLayer;
    private int canGatherLayer;
    public Animator anim;
    private RaycastHit currentHit; // 현재 타겟을 저장할 변수
    public PlayerAudio playerAudio;
    private PhotonView pv;

    private void Awake()
    {
        // 상호작용 할 수 있는 오브젝트
        canGatherLayer = LayerMask.NameToLayer("CanGather");
        // 먹을 수 있는 오브젝트
        //int canRootLayer = LayerMask.NameToLayer("CanRoot");
    }

    private void Start()
    {
        pv = GetComponentInParent<PhotonView>();
    }

    void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }
        // 캐릭터의 현재 위치
        Vector3 characterPosition = transform.position + Vector3.up *0.5f;

        // 캐릭터의 정면 방향 벡터 계산
        Vector3 forwardDirection = transform.forward;

        Ray ray = new Ray(characterPosition, forwardDirection * raycastLength*2); // 레이 생성

        
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, raycastLength, raycastLayer, QueryTriggerInteraction.Collide);
        Debug.DrawRay(ray.origin, ray.direction * raycastLength, isHit ? Color.green : Color.red);

        ToolType nowToolType = ToolType.None;
        // 현재 사용하고 있는 도구 타입

        if (InventoryManager.Instance.equippedItem != null && InventoryManager.Instance.equippedItem.itemData is ToolData toolData)
        {
            nowToolType = toolData.toolType;
        }

        // 애니메이션 상태 업데이트
        if (isHit && hit.collider.gameObject.layer == canGatherLayer)
        {
            if (hit.collider.CompareTag("Tree") && nowToolType == ToolType.Axe && !hit.collider.GetComponent<RespawnAnimation>().isRespawn && UserStatusManager.Instance.CanAxe)
            {
                // 도끼질 애니메이션 시작
                anim.SetBool("Axing", true);
                anim.SetBool("PickAxe", false);
                UserStatusManager.Instance.IsAxe = true;
                UserStatusManager.Instance.IsPickAxe = false;
            }
            else if ((hit.collider.CompareTag("Rock") || hit.collider.CompareTag("Iron") || hit.collider.CompareTag("Brimstone")) && nowToolType == ToolType.PickAxe && UserStatusManager.Instance.CanPickAxe)
            {
                // 곡괭이질 애니메이션 시작
                anim.SetBool("Axing", false);
                anim.SetBool("PickAxe", true);
                UserStatusManager.Instance.IsAxe = false;
                UserStatusManager.Instance.IsPickAxe = true;
            }
            else
            {
                // 다른 상호작용이 없으므로 모든 상호작용 애니메이션 종료
                anim.SetBool("Axing", false);
                anim.SetBool("PickAxe", false);
                UserStatusManager.Instance.IsAxe = false;
                UserStatusManager.Instance.IsPickAxe = false;
            }
            currentHit = hit;
        }
        else
        {
            // Raycast가 감지하지 못하면 모든 상호작용 애니메이션 종료
            anim.SetBool("Axing", false);
            anim.SetBool("PickAxe", false);
            UserStatusManager.Instance.IsAxe = false;
            UserStatusManager.Instance.IsPickAxe = false;
        }
            
            // 여기에 아마도 건물 상호작용 넣을 듯 루팅을 여기에 넣었는데 조작감이 거지같아서 폐기
            //else if(hit.collider.gameObject.layer == canRootLayer)
            //{
            //    if (Input.GetKeyDown(KeyCode.F))
            //    {
            //        Destroy(hit.collider.gameObject);
            //    }
            //}
    }

    // 애니메이션 이벤트에서 호출될 InteractionDamage 함수
    public void InteractionDamage()
    {
        // 현재 타겟에 데미지를 적용합니다.
        if (currentHit.collider != null)
        {
            InteractStatus interactStatus = currentHit.collider.GetComponent<InteractStatus>();
            WoodFallAnimation woodFallAnimation = currentHit.collider.GetComponent<WoodFallAnimation>();
            InventoryItem equippedItem = InventoryManager.Instance.equippedItem;

            if (interactStatus != null && (woodFallAnimation == null || !woodFallAnimation.isFalling))
            {
                string nowToolName = InventoryManager.Instance.equippedItem.itemData.itemName;
                ToolData toolData = (ToolData) InventoryManager.Instance.equippedItem.itemData;
                int nowToolInteractPower = toolData.toolInteractivePower;

                // 내구도 감소
                if(equippedItem != null)
                {
                    PhotonView targetPhotonView = currentHit.collider.gameObject.GetPhotonView();
                    float characterRotation = transform.rotation.eulerAngles.y;
                    if (InventoryManager.Instance.UseTool(equippedItem, interactStatus.interactData.InteractDurability))
                    {
                        if (targetPhotonView != null)
                        {
                            targetPhotonView.RPC("GetDamage", RpcTarget.All, nowToolInteractPower, characterRotation);
                        }
                        // 데미지를 적용합니다.
                    }
                    else
                    {
                        playerAudio.PlayBrokenSound();
                        if (targetPhotonView != null)
                        {
                            targetPhotonView.RPC("GetDamage", RpcTarget.All, nowToolInteractPower, characterRotation);
                        }
                    }
                }

                // 체력이 0 이하인지 확인하고, 필요한 경우 추가 액션을 수행합니다.
                if (interactStatus.interactHp <= 0)
                {
                    anim.SetBool("Axing", false); // 애니메이션 상태를 리셋합니다.
                    anim.SetBool("PickAxe", false); // 애니메이션 상태를 리셋합니다.
                }
            }
        }
    }
}
