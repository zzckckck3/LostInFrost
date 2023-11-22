using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Photon.Pun;

public enum AnimalState
{
    None,
    Idle,
    Wander,
    Chase,
    RunAway,
    Attack,
    Dead
}

public enum TargetType
{
    None,
    Player,
    Animal,
    Building
}

public class PhotonAnimalAI : MonoBehaviourPunCallbacks
{
    // 동물 상태 배회, 대기, 공격 등
    [SerializeField] private AnimalState animalState = AnimalState.None;
    // 동물 스크립터블 오브젝트 데이터
    [SerializeField] private AnimalData animalData;
    public AnimalData AnimalData
    {
        set { animalData = value; }
    }
    // 떨어지는 재료 스크립트
    // 네비게이션 AI
    private NavMeshAgent nav;
    // 애니메이터
    private Animator animator;
    // 동물 감지 범위 콜라이더
    private SphereCollider awareness;
    // 감지한 타겟
    public GameObject target;
    // 감지한 타겟의 종류
    [SerializeField] private TargetType targetType;
    // 죽음 유무
    [SerializeField] public bool isDead;
    // 수거 유무
    [SerializeField] public bool isPool;
    // 동물 체력
    [SerializeField] private int hp;
    // 피격 시
    private bool isHit;
    private CapsuleCollider myCollider;
    // 동물 이름
    public string animalName;

    private float lastStateChangeTime = 0f;
    private float stateChangeCooldown = 0.3f; // 쿨다운 시간, 예를 들어 1초
    private bool firstState = true;

    // 수민
    private IEnumerator currentCoroutine;

    private PhotonView pv;
    
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        awareness = GetComponentInChildren<SphereCollider>();
        myCollider = GetComponent<CapsuleCollider>();
        awareness.radius = animalData.animalAwareness;
        hp = animalData.animalHp;
        animalName = animalData.animalName;
        pv = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        if (pv.IsMine)
        {
            // 동물이 활성화될 때 돔울의 상태를 "Idle"로 설정
            //firstState = true;
            pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Idle.ToString());
        }
        hp = animalData.animalHp;
        isDead = false;
        isPool = false;
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnDisable()
    {
        // 동물이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "None"으로 설정
        StopCoroutine(animalState.ToString());
        animalState = AnimalState.None;
        //firstState = true;
    }

    //private void Update()
    //{
    //    if (isDead && isPool && gameObject.activeInHierarchy)
    //    {
    //        PhotonAnimalManager.Instance.ReturnObject(gameObject);
    //    }
    //}

    public void ChangeState(AnimalState newState)
    {
        // 현재 재생중인 상태와 바꾸려는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (animalState == newState )
        {
            //firstState = false;
            return;
        }

        // 이전에 재생중이던 상태 종료
        StopCoroutine(animalState.ToString());
        // 현재 적의 상태를 newState로 설정
        animalState = newState;
        // 새로운 상태 재생
        StartCoroutine(animalState.ToString());

        // 수민
        //if (currentCoroutine != null)
        //{
        //    StopCoroutine(currentCoroutine);
        //}
        //StopCoroutine(animalState.ToString());
        //animalState = newState;
        //// 새로운 상태 재생
        //currentCoroutine = GetCoroutineForState(newState);
        //StartCoroutine(currentCoroutine);
    }

    private IEnumerator GetCoroutineForState(AnimalState state)
    {
        switch (state)
        {
            case AnimalState.Idle:
                return Idle();
            case AnimalState.Wander:
                return Wander();
            case AnimalState.Chase:
                return Chase();
            case AnimalState.RunAway:
                return RunAway();
            case AnimalState.Attack:
                return Attack();
            case AnimalState.Dead:
                return Dead();
            default: return null;
        }
    }

    private IEnumerator Idle()
    {
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        // n초 후에 "Wander" 상태로 변경하는 코루틴 실행
        StartCoroutine(AutoChangeFromIdleToWander());

        while (true)
        {
            if (pv.IsMine)
            {
                // 자신의 죽음 검사
                DeadCheck();
            }
            if (isDead)
            {
                break;
            }
            
            // "Idle" 상태일 때 하는 행동
            // 선제공격 동물일 때 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            if (animalData.isFirstAttack)
            {
                if (pv.IsMine)
                {
                    // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                    CalculateDistanceToTarget();
                }
            }
            
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
            DeadCheck();
        }
        if (isDead)
        {
            yield break;
        }
        // Idle 상태일때 10% 확률로 동물마다 가진 특별한 액션을 취한다.
        int actionChance = Random.Range(1, 10);
        if (actionChance == 4)
        {
            if (pv.IsMine)
            {
                pv.RPC("RPCSpecialAction", RpcTarget.All, true);
                yield return new WaitForSeconds(actionChance);
                pv.RPC("RPCSpecialAction", RpcTarget.All, false);
            }
            
        }

        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);
        
        yield return new WaitForSeconds(changeTime);

        if (pv.IsMine)
        {
            // 상태를 "Wander"로 변경
            pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Wander.ToString());
        }
    }

    private IEnumerator Wander()
    {
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        pv.RPC("SetAnimationTrue", RpcTarget.All, "isWalking");
        //animator.SetBool("isWalking", true);
        float currentTime = 0;
        float maxTime = 4;
        
        if (pv.IsMine)
        {
            // 쫒던 중 배회 상태가 되면 주변 탐색
            Aware();
        }
        
        
        // 이동 속도 설정
        nav.speed = animalData.animalWalkSpeed;
        
        // 목표 위치 설정
        if (pv.IsMine)
        {
            CalculateWanderPosition();
        }
        
        
        // 목표 위치로 회전
        Vector3 to = new Vector3(nav.destination.x, 0, nav.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        if (pv.IsMine)
        {
            // 부드러운 회전을 위해 Lerp 사용
            pv.RPC("RPCRotate", RpcTarget.All, to, from);
        }
        while (true)
        {
            if (pv.IsMine)
            {
                // 자신의 죽음 검사
                DeadCheck();
            }
            
            if (isDead)
            {
                break;
            }
            
            currentTime += Time.deltaTime;
            
            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있다면
            to = new Vector3(nav.destination.x, 0, nav.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if (pv.IsMine && ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime))
            {
                // 상태를 "Idle"로 변경
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Idle.ToString());
            }
            
            // 선제공격 동물일 때 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            if (animalData.isFirstAttack)
            {
                if (pv.IsMine)
                {
                    // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                    CalculateDistanceToTarget();
                }
            }
            else
            {
                if (pv.IsMine)
                {
                    RunawayFromTarget();
                }
            }
            
            yield return null;
        }
    }
    [PunRPC]
    public void SetAnimationTrue(String anim)
    {
        animator.SetBool(anim, true);
    }

    private void CalculateWanderPosition()
    {
        // 현재 위치를 원점으로 하는 원의 반지름
        float wanderRadius = animalData.animalMoveRange;
        // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
        int wanderJitter = 0;
        // 최소 각도
        int wanderJitterMin = 0;
        // 최대 각도
        int wanderJitterMax = 360;
        
        // 자신의 위치를 중심으로 반지름(wanderRadius) 거리, 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);
        
        // 생성된 목표 위치가 자신의 이동구역을 벗어나지 않게 조절
        targetPosition.x = Mathf.Clamp(targetPosition.x, transform.position.x - wanderRadius, transform.position.x + wanderRadius);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, transform.position.z - wanderRadius, transform.position.z + wanderRadius);

        pv.RPC("RPCMove", RpcTarget.All, targetPosition);
    }

    private Vector3 SetAngle(float radius, int angle)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private IEnumerator Chase()
    {
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        pv.RPC("SetAnimationTrue", RpcTarget.All, "isRunning");
        //animator.SetBool("isRunning", true);
        float currentTime = 0;
        float maxTime = 5;
        
        while (true)
        {
            if (pv.IsMine)
            {
                // 자신의 죽음 검사
                DeadCheck();
            }
            
            if (isDead)
            {
                break;
            }
            
            currentTime += Time.deltaTime;
            
            // 이동 속도 설정 (Wander 시 걷기, Chase 시 달리기)
            nav.speed = animalData.animalRunSpeed;
            
            Vector3 attackRange = new Vector3(target.transform.position.x - 1f, target.transform.position.y, target.transform.position.z - 1f);
            if (pv.IsMine)
            {
                // 목표 위치를 현재 플레이어의 위치로 설정
                pv.RPC("RPCMove", RpcTarget.All, attackRange);
            }

            if (pv.IsMine)
            {
                // 타겟 방향을 계속 주시하도록 함
                LookRotationToTarget();
            }
            
            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있다면
            if (currentTime >= maxTime)
            {
                if (pv.IsMine)
                {
                    Aware();
                }
                if (target == null)
                {
                    // 상태를 "Idle"로 변경
                    if (pv.IsMine)
                    {
                        pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Idle.ToString());
                    }
                }

                currentTime = 0;
            }
            
            if (pv.IsMine)
            {
                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                CalculateDistanceToTarget();
            }
            
            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        
        // 자신 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        if (pv.IsMine)
        {
            // 서서히 돌기
            pv.RPC("RPCRotate", RpcTarget.All, to, from);
        }
    }

    private void CalculateDistanceToTarget()
    {
        if (target == null)
        {
            return;
        }

        float distance;
        float attackDistance;
        bool isUnactive = false;


        if (targetType == TargetType.Building)
        {
            BoxCollider buildingCollider = target.GetComponent<BoxCollider>();
            float buildingRadius = buildingCollider.bounds.extents.magnitude;
            float myRadius = myCollider.bounds.extents.magnitude;

            Vector3 toBuildingCenter = buildingCollider.bounds.center - transform.position;
            distance = toBuildingCenter.magnitude;
            // 건물의 반지름만큼 접근하여 공격
            attackDistance = buildingRadius + myRadius + 0.5f;
        }
        else
        {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            float radius = targetCollider.bounds.extents.magnitude;
            float myRadius = myCollider.bounds.extents.magnitude;
            if (!targetCollider.gameObject.activeInHierarchy)
            {
                isUnactive = true;
            }

            Vector3 toTargetCenter = targetCollider.bounds.center - transform.position;
            distance = toTargetCenter.magnitude;
            // 건물의 반지름만큼 접근하여 공격
            attackDistance = radius + myRadius + 0.5f;
        }

        if (animalState != AnimalState.Attack && distance <= attackDistance)
        {
            if (pv.IsMine)
            {
                // 공격 범위에 들어오면 공격
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Attack.ToString());
            }
            
        }
        else if (animalState != AnimalState.Chase && distance > (attackDistance + 1f) && (distance <= animalData.animalAwareness || isHit))
        {
            if (pv.IsMine)
            {
                // 대상이 인식 범위에 들어왔거나 피격 시 추격
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Chase.ToString());
            }
            
        } 
        else if ((animalState == AnimalState.Chase || animalState == AnimalState.Attack) && (distance > (animalData.animalAwareness + 1f)|| isUnactive) )
        {
            if (pv.IsMine)
            {
                // 인식 범위를 벗어나면 Wander상태
                pv.RPC("RPCSetTarget", RpcTarget.All, 0);
                pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Idle.ToString());
            }
        }
    }
    
    private IEnumerator RunAway()
    {
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        pv.RPC("SetAnimationTrue", RpcTarget.All, "isRunning");
        //animator.SetBool("isRunning", true);
        
        while (true)
        {
            if (pv.IsMine)
            {
                // 자신의 죽음 검사
                DeadCheck();
            }
            
            if (isDead)
            {
                break;
            }
            // 이동 속도 설정 (Wander 시 걷기, Chase 시 달리기)
            nav.speed = animalData.animalRunSpeed;

            if (pv.IsMine)
            {
                // 타겟 방향 반대를 향함
                OppositeRotationToTarget();
            
                // 보는 방향으로 MoveRange만큼 이동
                CalculateRunAwayPosition();
            
                // 타겟과의 거리에 따라 행동 선택 (Wander, RunAway)
                RunawayFromTarget();
            }

            if (pv.IsMine && isHit && animalData.isCounterAttack)
            {
                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                CalculateDistanceToTarget();
            }
            
            yield return null;
        }
    }
    
    private void CalculateRunAwayPosition()
    {
        // 동물이 바라보는 방향으로 도망
        Vector3 targetPosition = transform.position + transform.forward * (animalData.animalMoveRange);
        if (pv.IsMine)
        {
                pv.RPC("RPCMove", RpcTarget.All, targetPosition);
        }
        
    }
    
    private void OppositeRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        
        // 자신 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        
        if (pv.IsMine)
        {
                
            // 서서히 돌기
            pv.RPC("RPCOpposite", RpcTarget.All, to, from);
        }
        
    }
    
    private void RunawayFromTarget()
    {
        if (target == null)
        {
            return;
        }
        
        // Target과 동물의 거리 계산 후 거리에 따라 행동 선택
        float distance = Vector3.Distance(target.transform.position, transform.position);

        if (distance <= animalData.animalAwareness)
        {
            if (pv.IsMine)
            {
                // 추적
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.RunAway.ToString());
            }
            
        } 
        else if (animalState == AnimalState.RunAway && distance > animalData.animalAwareness + 5f)
        {
            if (pv.IsMine)
            {
                // 인식 범위를 벗어나면 Wander상태
                pv.RPC("RPCSetTarget", RpcTarget.All, 0);
                pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Idle.ToString());
            }
            
        }
    }
    
    private IEnumerator Attack()
    {
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        // 공격 주기 할당
        float lastAttackTime = animalData.animalAttackSpeed;
        float attackRate = animalData.animalAttackSpeed;
        if (pv.IsMine)
        {
            // 이동 위치 초기화
            pv.RPC("RPCStopMove", RpcTarget.All);
        }
        pv.RPC("SetAnimationTrue", RpcTarget.All, "isAttacking");
        //animator.SetBool("isAttacking", true);
        
        while (true)
        {
            if (pv.IsMine)
            {
                // 자신의 죽음 검사
                DeadCheck();
            }
            
            if (isDead)
            {
                break;
            }
            
            lastAttackTime += Time.deltaTime;
            
            if (pv.IsMine)
            {
                // 공격시 대상을 바라봄
                LookRotationToTarget();
                // 타겟이 죽었는지 확인
                TargetDeadCheck();
            }
            
            // 공격 주기
            if (lastAttackTime >= attackRate)
            {
                lastAttackTime = 0;
                
                if (pv.IsMine)
                {
                    // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                    CalculateDistanceToTarget();
                }
            }

            yield return null;
        }
    }

    private void AttackTarget()
    {
        if (pv.IsMine)
        {
            // 타겟이 동물인지 플레이어인지에 따라 다름
            if (targetType == TargetType.Animal)
            {
                // 동물 체력 감소
                target.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, animalData.animalDamage);
                target.GetComponent<PhotonView>().RPC("RPCSetTarget", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID);
                CheckTargetType(target);
            }
            else if(targetType == TargetType.Player)
            {
                // 플레이어 체력 감소
                target.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, animalData.animalDamage);
            }
            else if(targetType == TargetType.Building)
            {
                // 플레이어 체력 감소
                target.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, animalData.animalDamage);
            }  
        }
    }
    
    [PunRPC]
    public void GetDamage(int damage)
    {
        hp -= damage;
        isHit = true;
    }

    private void TargetDeadCheck()
    {
        // 타겟이 동물인지 플레이어인지에 따라 다름
        if ((targetType == TargetType.Animal && target.GetComponent<PhotonAnimalAI>().isDead) || 
            (targetType == TargetType.Player && target.GetComponentInChildren<PlayerStatus>().hp <= 0) ||
            (targetType == TargetType.Building && target.GetComponent<BuildingStatus>().hp <= 0))
        {
            if (pv.IsMine)
            {
                // 주변 탐색
                Aware();
                if (target == null)
                {
                    // 주변에 아무도 없으면 배회
                    pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Idle.ToString());
                }
            }
        }
    }

    private void DeadCheck()
    {
        // 체력이 0 이하일 때 Dead
        if (hp <= 0)
        {
            if (pv.IsMine)
            {
                pv.RPC("RPCSetTarget", RpcTarget.All, 0);
                pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
                pv.RPC("RPCSetState", RpcTarget.All, AnimalState.Dead.ToString());
            }
            
        }
    }

    private void Aware()
    {
        // 선제공격 동물이 아니라면 추격이 오래 지속될 때 끝까지 쫓아가지 않음
        if (pv.IsMine && !animalData.isFirstAttack)
        {
            // target = null;
            pv.RPC("RPCSetTarget", RpcTarget.All, 0);
            // targetType = TargetType.None;
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
            return;
        }
        // 동물의 인식 범위 안의 콜라이더 탐색 후 배열에 할당
        Collider[] colliders = Physics.OverlapSphere(transform.position, animalData.animalAwareness);
        // 타겟이 될 콜라이더
        Collider col = null;
        // 최소 거리
        float minimumDistance = Mathf.Infinity;
        
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == gameObject)
            {
                // 본인을 제외
                continue;
            }
            // 타겟 체크 플레이어, 동물, 건물이라면 실행
            if (CheckTargetType(collider.gameObject))
            {
                // 타겟과 자신 사이의 거리 계산
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minimumDistance)
                {
                    // 최소 거리의 타겟 지정
                    col = collider;
                    minimumDistance = distance;
                }
            }
        }

        // 타겟이 없으면
        if (col == null)
        {
            if (pv.IsMine)
            {
                pv.RPC("RPCSetTarget", RpcTarget.All, 0);
                pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
            }
        }
        else
        {
            if (pv.IsMine)
            {
                pv.RPC("RPCSetTarget", RpcTarget.All, col.gameObject.GetComponent<PhotonView>().ViewID);
                CheckTargetType(target);
            }
        }
    }

    public bool CheckTargetType(GameObject check)
    {
        // 타겟이 플레이어라면
        if (check.CompareTag("Player") && check.GetComponentInChildren<PlayerStatus>().hp>0)
        {
            Debug.Log("여기와? 포톤애니멀AI");
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.Player.ToString());
            return true;
        }
        // 타겟이 동물이라면
        else if (check.CompareTag("Animal") && !check.GetComponent<PhotonAnimalAI>().isDead)
        {
            if (!check.GetComponent<PhotonAnimalAI>().animalData.isFirstAttack && !animalData.isFirstAttack)
            {
                return false;
            }
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.Animal.ToString());
            return true;
        }
        // 타겟이 건물이라면
        else if (check.layer == LayerMask.NameToLayer("Building"))
        {
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.Building.ToString());
            return true;
        }

        return false;
    }
    
    private IEnumerator Dead()
    {
        if (pv.IsMine && !isDead)
        {
            // 이동 위치 초기화
            pv.RPC("RPCStopMove", RpcTarget.All);
            Vector3 temp = new Vector3(0, 0, 1);
            for (int a = 0; a < animalData.ingredientList.Count; a++)
            {
                if (animalData.ingredientList[a] == null) continue;
                NetworkManager.Instance.DropIngredient(animalData.ingredientList[a], transform.position - temp);
                temp -= new Vector3(1, 0, 0);
            }
        }
        if (pv.IsMine)
        {
            pv.RPC("StopAnim", RpcTarget.All);
        }
        isDead = true;
        pv.RPC("SetAnimationTrue", RpcTarget.All, "isDead");
        //animator.SetBool("isDead", true);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        while (true)
        {
            // 10초 뒤 시체 사라짐
            yield return new WaitForSeconds(5);
            PhotonAnimalManager.Instance.ReturnObject(gameObject);
            isPool = true;
            yield break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, animalData.animalAwareness);
    }

    [PunRPC]
    public void RPCSetState(string state)
    {
        ChangeState((AnimalState)Enum.Parse(typeof(AnimalState), state));
    }
    
    [PunRPC]
    public void RPCSetTarget(int viewID)
    {
        if (viewID == 0)
        {
            target = null;
            isHit = false;
            return;
        }
        PhotonView targetView = PhotonView.Find(viewID);
        target = targetView.gameObject;
    }
    
    [PunRPC]
    public void RPCSetTargetType(string type)
    {
        targetType = (TargetType)Enum.Parse(typeof(TargetType), type);
    }

    [PunRPC]
    public void RPCMove(Vector3 v)
    {
        
        nav.SetDestination(v);
    }

    [PunRPC]
    public void RPCStopMove()
    {
        nav.ResetPath();
    }

    [PunRPC]
    public void StopAnim()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", false);
    }

    [PunRPC]
    public void RPCSetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    
    [PunRPC]
    public void RPCSetTransform(Vector3 vector, Quaternion rotation)
    {
        transform.position = vector;
        transform.rotation = rotation;
    }
    
    [PunRPC]
    public void RPCRotate(Vector3 to, Vector3 from)
    {
        // 부드러운 회전을 위해 Lerp 사용
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(to - from), 1.0f);
    }
    
    [PunRPC]
    public void RPCOpposite(Vector3 to, Vector3 from)
    {
        // 부드러운 회전을 위해 Lerp 사용
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(from - to), 1.0f);
    }

    [PunRPC]
    private void RPCSpecialAction(bool action)
    {
        animator.SetBool("isSpecialAction", action);
    }
}
