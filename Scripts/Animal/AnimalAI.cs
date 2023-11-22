using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/*public enum AnimalState
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
}*/
public class AnimalAI : MonoBehaviour
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
    
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        awareness = GetComponentInChildren<SphereCollider>();
        myCollider = GetComponent<CapsuleCollider>();
        awareness.radius = animalData.animalAwareness;
        hp = animalData.animalHp;
        animalName = animalData.animalName;
    }

    private void OnEnable()
    {
        // 동물이 활성화될 때 돔울의 상태를 "Idle"로 설정
        ChangeState(AnimalState.Idle);
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

    public void ChangeState(AnimalState newState)
    {
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
                CalculateDistanceToTarget();
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
        ChangeState(AnimalState.Wander);
    }

    private IEnumerator Wander()
    {
        animator.SetBool("isWalking", true);
        float currentTime = 0;
        float maxTime = 10;
        
        // 쫒던 중 배회 상태가 되면 주변 탐색
        Aware();
        
        // 이동 속도 설정
        nav.speed = animalData.animalWalkSpeed;
        
        // 목표 위치 설정
        nav.SetDestination(CalculateWanderPosition());
        
        // 목표 위치로 회전
        Vector3 to = new Vector3(nav.destination.x, 0, nav.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        // 부드러운 회전을 위해 Lerp 사용
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(to - from), Time.deltaTime * 2.0f);

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
                animator.SetBool("isWalking", false);
                // 상태를 "Idle"로 변경
                ChangeState(AnimalState.Idle);
            }
            
            // 선제공격 동물일 때 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            if (animalData.isFirstAttack)
            {
                CalculateDistanceToTarget();
            }
            else
            {
                RunawayFromTarget();
            }
            
            yield return null;
        }
    }

    private Vector3 CalculateWanderPosition()
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

        return targetPosition;
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
                target = attacker;
                CheckTargetType(attacker);
            }

            Vector3 attackRange = new Vector3(target.transform.position.x - 1f, target.transform.position.y, target.transform.position.z - 1f);

            // 목표 위치를 현재 플레이어의 위치로 설정
            nav.SetDestination(attackRange);
            
            // 타겟 방향을 계속 주시하도록 함
            LookRotationToTarget();
            
            // 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있다면
            if (currentTime >= maxTime)
            {
                Aware();
                if (target == null)
                {
                    animator.SetBool("isRunning", false);
                    // 상태를 "Idle"로 변경
                    ChangeState(AnimalState.Idle);
                }

                currentTime = 0;
            }
            
            // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
            CalculateDistanceToTarget();
            
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
            animator.SetBool("isRunning", false);
            ChangeState(AnimalState.Attack);
        }
        else if (distance > attackDistance && (distance <= animalData.animalAwareness || isHit))
        {
            // 대상이 인식 범위에 들어왔거나 피격 시 추격
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", false);
            ChangeState(AnimalState.Chase);
        } 
        else if ((animalState == AnimalState.Chase || animalState == AnimalState.Attack) && distance > animalData.animalAwareness)
        {
            // 인식 범위를 벗어나면 Wander상태
            target = null;
            targetType = TargetType.None;
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRunning", false);
            ChangeState(AnimalState.Wander);
        }
    }
    
    private IEnumerator RunAway()
    {
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
            nav.SetDestination(CalculateRunAwayPosition());
            
            // 타겟과의 거리에 따라 행동 선택 (Wander, RunAway)
            RunawayFromTarget();

            if (isHit && animalData.isCounterAttack)
            {
                target = attacker;
                CheckTargetType(attacker);
                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                CalculateDistanceToTarget();
            }
            
            yield return null;
        }
    }
    
    private Vector3 CalculateRunAwayPosition()
    {
        // 동물이 바라보는 방향으로 도망
        Vector3 targetPosition = transform.position + transform.forward * (animalData.animalMoveRange);

        return targetPosition;
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
            animator.SetBool("isWalking", false);
            ChangeState(AnimalState.RunAway);
        } 
        else if (animalState == AnimalState.RunAway && distance > animalData.animalAwareness + 5f)
        {
            // 인식 범위를 벗어나면 Wander상태
            target = null;
            targetType = TargetType.None;
            animator.SetBool("isRunning", false);
            ChangeState(AnimalState.Wander);
        }
    }
    
    private IEnumerator Attack()
    {
        // 공격 주기 할당
        float lastAttackTime = animalData.animalAttackSpeed;
        float attackRate = animalData.animalAttackSpeed;
        // 이동 위치 초기화
        nav.ResetPath();
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
                target = attacker;
                CheckTargetType(attacker);
            }
            
            // 공격시 대상을 바라봄
            LookRotationToTarget();
            
            // 공격 주기
            if (lastAttackTime >= attackRate)
            {
                lastAttackTime = 0;
                
                // 타겟이 죽었는지 확인
                TargetDeadCheck();
                
                // 타겟과의 거리에 따라 행동 선택 (Wander, Chase, Attack)
                CalculateDistanceToTarget();
            }

            yield return null;
        }
    }

    private void AttackTarget()
    {
        // 타겟이 동물인지 플레이어인지에 따라 다름
        if (targetType == TargetType.Animal)
        {
            // 동물 체력 감소
            target.GetComponent<AnimalAI>().GetDamage(animalData.animalDamage);
        }
        else if(targetType == TargetType.Player)
        {
            // 플레이어 체력 감소
            UserStatusManager.Instance.HP -= animalData.animalDamage;
        }
        else if(targetType == TargetType.Building)
        {
            // 플레이어 체력 감소
            target.GetComponent<BuildingStatus>().hp -= animalData.animalDamage;
        }
    }
    
    public void GetDamage(int damage)
    {
        hp -= damage;
    }

    private void TargetDeadCheck()
    {
        // 타겟이 동물인지 플레이어인지에 따라 다름
        if ((targetType == TargetType.Animal && target.GetComponent<AnimalAI>().isDead) || 
            (targetType == TargetType.Player && UserStatusManager.Instance.HP <= 0) ||
            (targetType == TargetType.Building && target.GetComponent<BuildingStatus>().hp <= 0))
        {
            // 주변 탐색
            Aware();

            if (target == null)
            {
                // 주변에 아무도 없으면 배회
                StopAttack();
            }
        }
    }

    private void StopAttack()
    {
        animator.SetBool("isAttacking", false);
        ChangeState(AnimalState.Wander);
    }

    private void DeadCheck()
    {
        // 체력이 0 이하일 때 Dead
        if (hp <= 0)
        {
            target = null;
            targetType = TargetType.None;
            ChangeState(AnimalState.Dead);
        }
    }

    private void Aware()
    {
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
            target = null;
            targetType = TargetType.None;
        }
        else
        {
            target = col.gameObject;
            CheckTargetType(target);
        }
    }

    public bool CheckTargetType(GameObject check)
    {
        // 타겟이 플레이어라면
        if (check.CompareTag("Player"))
        {
            targetType = TargetType.Player;
            return true;
        }
        // 타겟이 동물이라면
        else if (check.CompareTag("Animal") && !check.GetComponent<AnimalAI>().isDead)
        {
            if (!check.GetComponent<AnimalAI>().animalData.isFirstAttack && !animalData.isFirstAttack)
            {
                return false;
            }
            targetType = TargetType.Animal;
            return true;
        }
        // 타겟이 건물이라면
        else if (check.layer == LayerMask.NameToLayer("Building"))
        {
            targetType = TargetType.Building;
            return true;
        }

        return false;
    }
    
    private IEnumerator Dead()
    {
        // 움직일 위치 초기화
        nav.ResetPath();
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, animalData.animalAwareness);
    }
}
