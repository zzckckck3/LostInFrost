using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// 유저의 상태에 관한 매니저
// 피로도, 배고픔, 추위, 체력 현재들고 있는 도구, 건물 건설 상태인지 기타 등등
public class UserStatusManager : MonoBehaviour
{
    public static UserStatusManager Instance { get; private set; }
    public ChangeAnimatorLayer changeAnimator;
    public Animator anim;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI staminaText;
    public TextMeshProUGUI hungryText;
    public TextMeshProUGUI coldText;

    public float difficulty = 0.5f;
    private bool isInGame;
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

        isInGame = !SceneManager.GetActiveScene().name.Equals("Home");

        // 최대 수치
        maxHP = 100.0f;
        maxStamina = 100.0f;
        maxHungry = 100.0f;
        maxCold = 100.0f;
        // 최소 수치
        minHP = 0.0f;
        minStamina = 0.0f;
        minHungry = 0.0f;
        minCold = 0.0f;

        // 상태 초기화
        if (!SceneManager.GetActiveScene().name.Equals("Home"))
        {
            hpText.text = "100";
            staminaText.text = "0";
            hungryText.text = "0";
            coldText.text = "0";
        }

        defense = 0;
        hp = 100.0f;
        stamina = 0.0f;
        hungry = 0.0f;
        cold = 0.0f;
        hpChangeAmounts = 1.0f;
        staminaChangeAmounts = 1.0f;
        hungryChangeAmounts = 1.0f;
        coldChangeAmounts = 1.0f;

        isBuild = false;
        isBuilding = false;
        isAxe = false;
        isPickAxe = false;
        isRoot = false;
        isRun = false;
        isMove = false;
        isDead = false;
        isAttackReady = false;
        isAttack = false;
        isInHouse = false;

        canBuild = true;
        canAxe = true;
        canPickAxe = true;
        canRoot = true;
        canRun = true;
        canMove = true;
        canAttackReady = true;
        canAttack = true;
    }
    private void Start()
    {

    }

    #region FIELDS AND STATUS
    // 유저의 max hp
    private float maxHP;
    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }
    // 유저의 max 스테미너
    private float maxStamina;
    public float MaxStamina
    {
        get { return maxStamina; }
        set { maxStamina = value; }
    }
    // 유저의 max 배고픔
    private float maxHungry;
    public float MaxHungry
    {
        get { return maxHungry; }
        set { maxHungry = value; }
    }
    // 유저의 max 추위
    private float maxCold;
    public float MaxCold
    {
        get { return maxCold; }
        set { maxCold = value; }
    }

    // 유저의 min hp
    private float minHP;
    public float MinHP
    {
        get { return minHP; }
        set { minHP = value; }
    }
    // 유저의 min 스테미너
    private float minStamina;
    public float MinStamina
    {
        get { return minStamina; }
        set { minStamina = value; }
    }
    // 유저의 min 배고픔
    private float minHungry;
    public float MinHungry
    {
        get { return minHungry; }
        set { minHungry = value; }
    }
    // 유저의 min 추위
    private float minCold;
    public float MinCold
    {
        get { return minCold; }
        set { minCold = value; }
    }
    // 유저의 데미지 감소 수치
    private float defense;
    public float Defense
    {
        get { return defense; }
        set { defense = value;  }
    }
    // 유저의 hp
    private float hp;
    public float HP
    {
        get { return hp; }

        // 여기 안에서 필요한 로직 ex) hp<=0이면 isDead를 true 아니면
        // IsDead에서 hp<0인지를 보고 isDead를 true로 바꿔줄 수있음
        set { hp = value; }
    }
    private float hpChangeAmounts;
    public float HpChangeAmounts
    {
        get { return hpChangeAmounts; }
        set { hpChangeAmounts = value; }
    }

    // 유저의 stamina
    private float stamina;
    public float Stamina
    {
        get { return stamina; }
        set { stamina = value; }  // 여기 안에서 필요한 로직 ex) 
    }
    private float staminaChangeAmounts;
    public float StaminaChangeAmounts
    {
        get { return staminaChangeAmounts; }
        set { staminaChangeAmounts = value; }
    }

    // 유저의 배고픔 수치
    private float hungry;
    public float Hungry
    {
        get { return hungry; }
        set { hungry = value; }  // 여기 안에서 검증과정
    }
    private float hungryChangeAmounts;
    public float HungryChangeAmounts
    {
        get { return hungryChangeAmounts; }
        set { hungryChangeAmounts = value; }
    }

    // 유저의 추위 수치
    private float cold;
    public float Cold
    {
        get { return cold; }
        set { cold = value; }  // 여기 안에서 검증과정
    }
    private float coldChangeAmounts;
    public float ColdChangeAmounts
    {
        get { return coldChangeAmounts; }
        set { coldChangeAmounts = value; }
    }
    private void UpdateCapabilities()
    {
        canAxe = !isRun && !isRoot && !isPickAxe && !isBuild && !isBuilding && !isAttackReady && !isAttack && !isMake && !isDead; // 뛸때, 주울때, 곡괭이질일 때, 건설 모드일때 도끼질 불가능
        canPickAxe = !isRun && !isRoot && !isAxe && !isBuild && !isBuilding && !isAttackReady && !isAttack && !isMake && !isDead;
        canRoot = !isRoot && !isBuild && !isBuilding && !isMake && !isDead;
        canMove = !isRoot && !isBuilding && !isMake && !isDead;
        canBuild = !isRoot && !isAttackReady && !isBuild && !isBuilding && !isAttack && !isMake && !isDead;
        canAttackReady = !isRoot && !isBuild && !isBuilding && !isMake && !isDead;
        canAttack = !isRoot && !isAttack && !isMake && !isDead;
        canMake =  !isAttackReady && !isAttack && !isBuilding && !isMake && !isDead;
        //canRun

        // 다른 canX 설정들...
        if(IsInHouse)
        {
            canMove = false;
            canRun = false;
            canAxe = false;
            canPickAxe = false;
            canRoot = false;
            canMake = false;
            canBuild = false;
            canAttackReady = false;
            canAttack = false;
            canMake = false;
        }
    }
    // 공격 모션
    private bool canMake;
    public bool CanMake
    {
        get { return canMake; }
    }

    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        set
        {
            isDead = value;
            if (isDead)
            {
                if (anim)
                {
                    anim.SetTrigger("Dead");
                }
            }
            UpdateCapabilities();
        }
    }

    private bool isMake;
    public bool IsMake
    {
        get { return isMake; }
        set
        {
            isMake = value;
            if (isMake)
            {
                if (anim)
                {
                    anim.SetBool("Make", true);
                }
            }
            else
            {
                if (anim)
                {
                    anim.SetBool("Make", false);
                }
            }
            UpdateCapabilities();
        }
    }

    // 공격 모션
    private bool canAttack;
    public bool CanAttack
    {
        get { return canAttack; }
    }

    private bool isAttack;
    public bool IsAttack
    {
        get { return isAttack; }
        set
        {
            isAttack = value;
            UpdateCapabilities();
        }
    }
    // 공격 준비 모션
    private bool canAttackReady;
    public bool CanAttackReady
    {
        get { return canAttackReady; }
    }

    private bool isAttackReady;
    public bool IsAttackReady
    {
        get { return isAttackReady; }
        set
        {
            isAttackReady = value;
            if (isAttackReady) changeAnimator.ToWeaponLayer();
            else
            {
                changeAnimator.ToBaseLayer();
                if (isBuild) changeAnimator.ToWeaponLayer();
            }
            UpdateCapabilities();
        }
    }


    // 유저가 현재 건설 모드인지
    private bool isBuild;
    public bool IsBuild
    {
        get { return isBuild; }
        set
        {
            isBuild = value;
            if (isBuild) changeAnimator.ToWeaponLayer();
            else changeAnimator.ToBaseLayer();
            UpdateCapabilities();
        }
    }

    // 유저가 현재 건설 중인지
    private bool isBuilding;
    public bool IsBuilding
    {
        get { return isBuilding; }
        set
        {
            isBuilding = value;
            if (isBuilding) anim.SetBool("Building",true);
            else anim.SetBool("Building", false);
            UpdateCapabilities();
        }
    }

    // 유저가 현재 건설 모드로 전환 가능한지
    private bool canBuild;
    public bool CanBuild
    {
        get { return canBuild; }
    }

    // 유저가 현재 도끼질 모드인지
    private bool isAxe;
    public bool IsAxe
    {
        get { return isAxe; }
        set
        {
            isAxe = value;
            UpdateCapabilities();
        }
    }

    // 유저가 도끼질 모드로 전환 가능한지
    private bool canAxe;
    public bool CanAxe
    {
        get { return canAxe; }
    }

    // 유저가 현재 광질중인지
    private bool isPickAxe;
    public bool IsPickAxe
    {
        get { return isPickAxe; }
        set
        {
            isPickAxe = value;
            UpdateCapabilities();
        }
    }

    // 유저가 광질할 수 있는 상태인지
    private bool canPickAxe;
    public bool CanPickAxe
    {
        get { return canPickAxe; }
    }

    // 유저가 현재 수집중인지
    private bool isRoot;
    public bool IsRoot
    {
        get { return isRoot; }
        set
        {
            isRoot = value;
            UpdateCapabilities();

        }
    }

    // 유저가 수집할 수 있는 상태인지
    private bool canRoot;
    public bool CanRoot
    {
        get { return canRoot; }
    }

    // 유저가 현재 뛰고있는지
    private bool isRun;
    public bool IsRun
    {
        get { return isRun; }
        set
        {
            isRun = value;
            UpdateCapabilities();

        }
    }

    // 유저가 현재 뛸수 있는 상태인지
    private bool canRun;
    public bool CanRun
    {
        get { return canRun; }
    }

    // 유저가 현재 움직이고 있는지
    private bool isMove;
    public bool IsMove
    {
        get { return isMove; }
        set { isMove = value; }
    }
    // 유저가 현재 움직일 수 있는 상태인지
    private bool canMove;
    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

    private bool isInHouse;
    public bool IsInHouse
    {
        get { return isInHouse; }
        set { isInHouse = value; }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (isInGame)

        {
            // difficulty 변수로 초당 늘어나는 상태변화를 조절 가능
            // 시간이 지남에 따른 캐릭터 상태 변화 및 적용
            if (stamina < maxStamina) // 피로도
            {
                canRun = true;
                stamina += staminaChangeAmounts * difficulty * Time.deltaTime * 0.7f;
            }
            else if (stamina >= maxStamina)
            {
                canRun = false;
                hp -= difficulty * Time.deltaTime;
            }
            if (hungry < maxHungry) // 배고픔
            {
                hungry += hungryChangeAmounts * difficulty * Time.deltaTime * 0.5f;
            }
            else if (hungry >= maxHungry)
            {
                hp -= difficulty * Time.deltaTime;
            }
            if (cold < maxCold) // 추위
            {
                cold += coldChangeAmounts * difficulty * Time.deltaTime;
            }
            else if (cold >= maxCold)
            {
                hp -= difficulty * Time.deltaTime;
            }

            // 피로도, 배고픔, 추위가 일정 수치보다 낮고, 100 이하면 체력이 서서히 찬다.
            if (!isDead && stamina < 30 && hungry < 30 && cold < 30 && hp <= maxHP)
            {
                hp += 0.5f * Time.deltaTime;
            }

            // 밤일때 cold 수치 추가 증가
            if (TimeManager.Instance.IsNight && cold < maxCold)
            {
                cold += coldChangeAmounts * 0.5f * Time.deltaTime;
            }

            // 뛰는 상태면 난이도의 1/2만큼 초당 피로도가 쌓임
            if (isRun && stamina < maxStamina)
            {
                stamina += difficulty * 0.5f * Time.deltaTime;
            }

            // 최대 스탯보다 높은지 체크
            CheckOverStatus();

            staminaText.text = Mathf.FloorToInt(stamina).ToString();
            hungryText.text = Mathf.FloorToInt(hungry).ToString();
            coldText.text = Mathf.FloorToInt(cold).ToString();

            hpText.text = Mathf.FloorToInt(hp).ToString();
        }
        
    }

    public void GetDamage(float damage)
    {
        hp -= (damage - defense);
    }

    private void CheckOverStatus()
    {
        // 최대 수치 초과 시
        if(hp > maxHP)
        {
            hp = maxHP;
        }
        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
        if (hungry > maxHungry)
        {
            hungry = maxHungry;
        }
        if (cold > maxCold)
        {
            cold = maxCold;
        }
        // 최소 수치 미만 시
        if(hp < 0)
        {
            hp = 0;
        }
        if (stamina < 0)
        {
            stamina = 0;
        }
        if (hungry < 0)
        {
            hungry = 0;
        }
        if (cold < 0)
        {
            cold = 0;
        }
    }

    public void resetValues()
    {
        hp = 100.0f;
        stamina = 0;
        hungry = 0;
        cold = 0;
    }
}
