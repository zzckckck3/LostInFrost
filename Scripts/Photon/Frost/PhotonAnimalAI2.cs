using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Photon.Pun;

public class PhotonAnimalAI2 : MonoBehaviour
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
    public IngredientDrop ingredientDrop;
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
    // 동물 체력
    [SerializeField] private int hp;
    // 공격한 타겟
    public GameObject attacker;
    // 피격 시
    private bool isHit;
    private CapsuleCollider myCollider;
    // 동물 이름
    public string animalName;
    private PhotonView pv;
    private Vector3 ranVector;
    private Vector3 runPosition;

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
        if (!pv.IsMine)
        {
            return;
        }
        // 동물이 활성화될 때 돔울의 상태를 "Idle"로 설정
        pv.RPC("ChangeState", RpcTarget.All,AnimalState.Idle);
        hp = animalData.animalHp;
        isDead = false;
        animator.SetBool("isDead", isDead);
    }

    private void OnDisable()
    {
        // 동물이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "None"으로 설정
        StopCoroutine(animalState.ToString());
        animalState = AnimalState.None;
    }

    [PunRPC]
    public void ChangeState(AnimalState newState)
    {
        if (!pv.IsMine)
        {
            return;
        }
        // 현재 재생중인 상태와 바꾸려는 상태가 같으면 바꿀 필요가 없기 때문에 return
        if (animalState == newState)
        {
            return;
        }

        // 이전에 재생중이던 상태 종료
        StopCoroutine(animalState.ToString());
        // 현재 적의 상태를 newState로 설정
        animalState = newState;
        // 새로운 상태 재생
        StartCoroutine(animalState.ToString());
    }

    private IEnumerator Idle()
    {
        pv.RPC("RPCStopAnim", RpcTarget.All);
        // n초 후에 "Wander" 상태로 변경하는 코루틴 실행
        StartCoroutine(AutoChangeFromIdleToWander());

        while (true)
        {
            // 자신의 죽음 검사
            DeadCheck();
            if (isDead)
            {
                break;
            }

            // "Idle" 상태일 때 하는 행동
            // 선제공격 동물일 때 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            if (animalData.isFirstAttack)
            {
                pv.RPC("CalculateDistanceToTarget", RpcTarget.All);
            }

            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander()
    {
        // Idle 상태일때 10% 확률로 동물마다 가진 특별한 액션을 취한다.
        int actionChance = Random.Range(1, 10);
        if (actionChance == 4)
        {
            animator.SetBool("isSpecialAction", true);
            yield return new WaitForSeconds(actionChance);
            animator.SetBool("isSpecialAction", false);

        }

        // 1~4초 시간 대기
        int changeTime = Random.Range(1, 5);

        yield return new WaitForSeconds(changeTime);

        // 상태를 "Wander"로 변경
        pv.RPC("ChangeState", RpcTarget.All, AnimalState.Wander);
    }

    private IEnumerator Wander()
    {
        pv.RPC("RPCStopAnim", RpcTarget.All);
        animator.SetBool("isWalking", true);
        float currentTime = 0;
        float maxTime = 10;

        // 쫒던 중 배회 상태가 되면 주변 탐색
        pv.RPC("Aware", RpcTarget.All);

        // 이동 속도 설정
        nav.speed = animalData.animalWalkSpeed;

        // 목표 위치 설정
        pv.RPC("CalculateWanderPosition", RpcTarget.All);
        pv.RPC("RPCMove", RpcTarget.All, ranVector);

        // 목표 위치로 회전
        Vector3 to = new Vector3(nav.destination.x, 0, nav.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        pv.RPC("RPCRotate", RpcTarget.All, to, from);
        
        while (true)
        {
            // 자신의 죽음 검사
            DeadCheck();
            if (isDead)
            {
                break;
            }

            currentTime += Time.deltaTime;

            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있다면
            to = new Vector3(nav.destination.x, 0, nav.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);
            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
            {
                // 상태를 "Idle"로 변경
                pv.RPC("ChangeState", RpcTarget.All, AnimalState.Idle);
            }

            // 선제공격 동물일 때 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            if (animalData.isFirstAttack)
            {
                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                pv.RPC("CalculateDistanceToTarget", RpcTarget.All);
            }
            else
            {
                RunawayFromTarget();
            }

            yield return null;
        }
    }

    [PunRPC]
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

        ranVector = targetPosition;
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
        pv.RPC("RPCStopAnim", RpcTarget.All);
        animator.SetBool("isRunning", true);
        float currentTime = 0;
        float maxTime = 5;

        while (true)
        {
            // 자신의 죽음 검사
            DeadCheck();
            if (isDead)
            {
                break;
            }

            currentTime += Time.deltaTime;

            // 이동 속도 설정 (Wander 시 걷기, Chase 시 달리기)
            nav.speed = animalData.animalRunSpeed;

            // 피격 시
            if (isHit)
            {
                // 타겟을 공격한 대상으로 변경
                pv.RPC("RPCSetTarget", RpcTarget.All, attacker.GetComponent<PhotonView>().ViewID);
                CheckTargetType(target);
            }

            Vector3 attackRange = new Vector3(target.transform.position.x - 1f, target.transform.position.y, target.transform.position.z - 1f);

            // 목표 위치를 현재 플레이어의 위치로 설정
            nav.SetDestination(attackRange);

            // 타겟 방향을 계속 주시하도록 함
            LookRotationToTarget();

            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있다면
            if (currentTime >= maxTime)
            {
                pv.RPC("Aware", RpcTarget.All);
                if (target == null)
                {
                    // 상태를 "Idle"로 변경
                    pv.RPC("ChangeState", RpcTarget.All, AnimalState.Idle);
                }

                currentTime = 0;
            }
            
            // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            pv.RPC("CalculateDistanceToTarget", RpcTarget.All);

            yield return null;
        }
    }

    private void LookRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);

        // 자신 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // 서서히 돌기
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(to - from), 1f);
    }

    [PunRPC]
    private void CalculateDistanceToTarget()
    {
        if (target == null)
        {
            return;
        }

        float distance;
        float attackDistance;

        if (targetType == TargetType.Building)
        {
            MeshCollider buildingCollider = target.GetComponent<MeshCollider>();
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

            Vector3 toTargetCenter = targetCollider.bounds.center - transform.position;
            distance = toTargetCenter.magnitude;
            // 건물의 반지름만큼 접근하여 공격
            attackDistance = radius + myRadius + 0.5f;
        }


        if (distance <= attackDistance)
        {
            // 공격 범위에 들어오면 공격
            pv.RPC("ChangeState", RpcTarget.All, AnimalState.Attack);
        }
        else if (distance > attackDistance && (distance <= animalData.animalAwareness || isHit))
        {
            // 대상이 인식 범위에 들어왔거나 피격 시 추격
            pv.RPC("ChangeState", RpcTarget.All,AnimalState.Chase);
        }
        else if ((animalState == AnimalState.Chase || animalState == AnimalState.Attack) && distance > animalData.animalAwareness)
        {
            // 인식 범위를 벗어나면 Wander상태
            target = null;
            targetType = TargetType.None;
            pv.RPC("ChangeState", RpcTarget.All,AnimalState.Wander);
        }
    }

    private IEnumerator RunAway()
    {
        pv.RPC("RPCStopAnim", RpcTarget.All);
        animator.SetBool("isRunning", true);

        while (true)
        {
            // 자신의 죽음 검사
            DeadCheck();
            if (isDead)
            {
                break;
            }
            // 이동 속도 설정 (Wander 시 걷기, Chase 시 달리기)
            nav.speed = animalData.animalRunSpeed;

            // 타겟 방향 반대를 향함
            OppositeRotationToTarget();

            // 보는 방향으로 MoveRange만큼 이동
            pv.RPC("CalculateRunAwayPosition", RpcTarget.All);
            pv.RPC("RPCRunAway", RpcTarget.All, runPosition);

            // 타겟과의 거리에 따라 행동 선택 (Wander, RunAway)
            RunawayFromTarget();

            if (isHit && animalData.isCounterAttack)
            {
                pv.RPC("RPCSetTarget", RpcTarget.All, attacker.GetComponent<PhotonView>().ViewID);
                CheckTargetType(target);
                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                pv.RPC("CalculateDistanceToTarget", RpcTarget.All);
            }

            yield return null;
        }
    }

    [PunRPC]
    private void CalculateRunAwayPosition()
    {
        // 동물이 바라보는 방향으로 도망
        Vector3 targetPosition = transform.position + transform.forward * (animalData.animalMoveRange);

        runPosition = targetPosition;
    }

    private void OppositeRotationToTarget()
    {
        // 목표 위치
        Vector3 to = new Vector3(target.transform.position.x, 0, target.transform.position.z);

        // 자신 위치
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        // 서서히 돌기
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(from - to), 1f);
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
            // 추적
            pv.RPC("ChangeState", RpcTarget.All,AnimalState.RunAway);
        }
        else if (animalState == AnimalState.RunAway && distance > animalData.animalAwareness + 5f)
        {
            // 인식 범위를 벗어나면 Wander상태
            target = null;
            targetType = TargetType.None;
            pv.RPC("ChangeState", RpcTarget.All,AnimalState.Wander);
        }
    }

    private IEnumerator Attack()
    {
        pv.RPC("RPCStopAnim", RpcTarget.All);
        // 공격 주기 할당
        float lastAttackTime = animalData.animalAttackSpeed;
        float attackRate = animalData.animalAttackSpeed;
        // 이동 위치 초기화
        pv.RPC("RPCStopMove", RpcTarget.All);
        animator.SetBool("isAttacking", true);

        while (true)
        {
            // 자신의 죽음 검사
            DeadCheck();
            if (isDead)
            {
                break;
            }

            lastAttackTime += Time.deltaTime;

            // 피격 시
            if (isHit)
            {
                // 타겟을 공격한 대상으로 변경
                pv.RPC("RPCSetTarget", RpcTarget.All, attacker.GetComponent<PhotonView>().ViewID);
                CheckTargetType(target);
            }

            // 공격시 대상을 바라봄
            LookRotationToTarget();

            // 공격 주기
            if (lastAttackTime >= attackRate)
            {
                lastAttackTime = 0;

                // 타겟이 죽었는지 확인
                pv.RPC("TargetDeadCheck", RpcTarget.All);

                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                pv.RPC("CalculateDistanceToTarget", RpcTarget.All);
            }

            yield return null;
        }
    }

    private void AttackTarget()
    {
        if (!pv.IsMine || pv == null)
        {
            return;
        }
        // 타겟이 동물인지 플레이어인지에 따라 다름
        if (targetType == TargetType.Animal)
        {
            target.GetComponent<PhotonView>().RPC("RPCSetAttacker", RpcTarget.All, gameObject);
            // 동물 체력 감소
            target.GetComponent<PhotonView>().RPC("GetDamage", RpcTarget.All, animalData.animalDamage);
        }
        else if (targetType == TargetType.Player)
        {
            // 플레이어 체력 감소
            UserStatusManager.Instance.HP -= animalData.animalDamage;
        }
        else if (targetType == TargetType.Building)
        {
            // 플레이어 체력 감소
            target.GetComponent<BuildingStatus>().hp -= animalData.animalDamage;
        }
    }

    [PunRPC]
    public void GetDamage(int damage)
    {
        hp -= damage;
    }

    [PunRPC]
    private void TargetDeadCheck()
    {
        if (!pv.IsMine || pv == null)
        {
            return;
        }
        // 타겟이 동물인지 플레이어인지에 따라 다름
        if ((targetType == TargetType.Animal && target.GetComponent<PhotonAnimalAI>().isDead) ||
            (targetType == TargetType.Player && UserStatusManager.Instance.HP <= 0) ||
            (targetType == TargetType.Building && target.GetComponent<BuildingStatus>().hp <= 0))
        {
            // 주변 탐색
            pv.RPC("Aware", RpcTarget.All);

            if (target == null)
            {
                // 주변에 아무도 없으면 배회
                StopAttack();
            }
        }
    }

    private void StopAttack()
    {
        if (!pv.IsMine)
        {
            return;
        }
        pv.RPC("ChangeState", RpcTarget.All,AnimalState.Wander);
    }

    private void DeadCheck()
    {
        if (!pv.IsMine)
        {
            return;
        }
        // 체력이 0 이하일 때 Dead
        if (hp <= 0)
        {
            pv.RPC("RPCSetTarget", RpcTarget.All, 0);
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
            pv.RPC("RPCSetAttacker", RpcTarget.All, 0);
            pv.RPC("ChangeState", RpcTarget.All,AnimalState.Dead);
        }
    }

    [PunRPC]
    private void Aware()
    {
        if (!pv.IsMine)
        {
            return;
        }
        // 선제공격 동물이 아니라면 추격이 오래 지속될 때 끝까지 쫓아가지 않음
        if (!animalData.isFirstAttack)
        {
            target = null;
            
            targetType = TargetType.None;
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
            pv.RPC("RPCSetTarget", RpcTarget.All, 0);
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.None.ToString());
        }
        else
        {
            pv.RPC("RPCSetTarget", RpcTarget.All, col.gameObject.GetComponent<PhotonView>().ViewID);
            CheckTargetType(target);
        }
    }

    public bool CheckTargetType(GameObject check)
    {
        // 타겟이 플레이어라면
        if (check.CompareTag("Player"))
        {
            pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.Player.ToString());
            return true;
        }
        // 타겟이 동물이라면
        else if (check.CompareTag("Animal") && !check.GetComponent<PhotonAnimalAI>().isDead)
        {
            // if (!check.GetComponent<PhotonAnimalAI>().animalData.isFirstAttack && !animalData.isFirstAttack)
            // {
            //     return false;
            // }
            // pv.RPC("RPCSetTargetType", RpcTarget.All, TargetType.Animal.ToString());
            // return true;
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
        isHit = false;
        // 움직일 위치 초기화
        pv.RPC("RPCStopMove", RpcTarget.All);
        pv.RPC("RPCStopAnim", RpcTarget.All);
        isDead = true;
        animator.SetBool("isDead", isDead);
        ingredientDrop.DropIngredients(animalData.ingredientList);
        while (true)
        {
            // 10초 뒤 시체 사라짐
            yield return new WaitForSeconds(10);
            AnimalManager.Instance.ReturnObject(gameObject);
        }
    }
    
    [PunRPC]
    private void RPCSetTarget(int viewID)
    {
        if (viewID == 0)
        {
            target = null;
            return;
        }
        PhotonView targetView = PhotonView.Find(viewID);
        target = targetView.gameObject;
    }
    
    [PunRPC]
    private void RPCSetAttacker(int viewID)
    {
        if (viewID == 0)
        {
            target = null;
            return;
        }
        PhotonView targetView = PhotonView.Find(viewID);
        isHit = true;
        attacker = targetView.gameObject;
    }
    
    [PunRPC]
    private void RPCSetTargetType(string type)
    {
        targetType = (TargetType)Enum.Parse(typeof(TargetType), type);
    }

    [PunRPC]
    private void RPCMove(Vector3 v)
    {
        
        nav.SetDestination(v);
    }
    
    [PunRPC]
    private void RPCRunAway(Vector3 v)
    {
        
        nav.SetDestination(v);
    }

    [PunRPC]
    private void RPCStopMove()
    {
        nav.ResetPath();
    }

    [PunRPC]
    private void RPCStopAnim()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
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
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(to - from), Time.deltaTime * 2.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, animalData.animalAwareness);
    }
}
