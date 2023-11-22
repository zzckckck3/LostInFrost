using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    // 외부 API를 객체로 선언
    PopupOpener popupOpener;
    
    private float lastToggleTime = 0f;
    private float toggleDelay = 0.3f; // 팝업 오픈 시 애니메이션 딜레이

    public static KeyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        inventoryOn = false;
        combinationRecipesOn = false;
        buildRecipesOn = false;
        buildingInteractOn = false;
    }

    // 무작정 Instance를 통해 값을 조절하려는건 나쁜 시도일 수 있음
    // 여기서는 Instance만 public으로 해야 하는데 해당하는 변수까지 public으로 한 이유는
    // ref와 out은 getter와 setter에는 접근할 수 없다.
    // 다른 코드에서 keyManager를 통해서 전부 해결하려 했으나 구조적으로 살짝 실패함
    #region POPUP STATE
    public bool inventoryOn;
    public bool InventoryOn
    {
        get { return inventoryOn; }
        set { inventoryOn = value; }
    }

    public bool combinationRecipesOn;
    public bool CombinationRecipesOn
    {
        get { return combinationRecipesOn; }
        set { combinationRecipesOn = value; }
    }

    public bool buildRecipesOn;
    public bool BuildRecipesOn
    {
        get { return buildRecipesOn; }
        set { buildRecipesOn = value; }
    }

    public bool buildingInteractOn;
    public bool BuildingInteractOn
    {
        get { return buildingInteractOn; }
        set { buildingInteractOn = value; }
    }

    public bool blacksmithOn;
    public bool BlacksmithOn
    {
        get { return blacksmithOn; }
        set { blacksmithOn = value; }
    }
    public bool gunsmithOn;
    public bool GunsmithOn
    {
        get { return gunsmithOn; }
        set { gunsmithOn = value; }
    }
    public bool bonfireOn;
    public bool BonfireOn
    {
        get { return bonfireOn; }
        set { bonfireOn = value; }
    }

    public bool interactingWithBlacksmith;
    public bool InteractingWithBlacksmith
    {
        get { return interactingWithBlacksmith; }
        set { interactingWithBlacksmith = value; }
    }

    public bool interactingWithGunsmith;
    public bool InteractingWithGunsmith
    {
        get { return interactingWithGunsmith; }
        set { interactingWithGunsmith = value; }
    }

    public bool interactingWithBonfire;
    public bool InteractingWithBonfire
    {
        get { return interactingWithBonfire; }
        set { interactingWithBonfire = value; }
    }

    public bool interactingWithHouse;

    public string buildingParameter;
    public string BuildingParameter
    {
        get { return buildingParameter; }
        set { buildingParameter = value; }
    }
    #endregion


    void Start()
    {
        popupOpener = GetComponent<PopupOpener>();
    }
    void Update()
    {
        /*
        ESC 버튼을 눌렀을 때 처리
        UI가 켜져있는지에 대한 상태를 받아서 켜져있다면 코루틴을 호출 > 자동으로 켜져있기에 끄는 분기로 이동
        */
        if (Input.GetButtonDown("Escape"))
        {
            if(inventoryOn)
            {
                StartCoroutine(ToggleTrigger(inventoryOn, "Inventory"));
                inventoryOn = !inventoryOn;
            }
            if (combinationRecipesOn)
            {
                StartCoroutine(ToggleTrigger(combinationRecipesOn, "CombinationRecipes"));
                combinationRecipesOn = !combinationRecipesOn;
            }
            if (buildRecipesOn)
            {
                StartCoroutine(ToggleTrigger(buildRecipesOn, "BuildRecipes"));
                buildRecipesOn = !buildRecipesOn;
            }
            if (blacksmithOn)
            {
                StartCoroutine(ToggleTrigger(blacksmithOn, "BlacksmithPopup"));
                blacksmithOn = !blacksmithOn;
            }
            if (gunsmithOn)
            {
                StartCoroutine(ToggleTrigger(gunsmithOn, "GunsmithPopup"));
                gunsmithOn = !gunsmithOn;
            }
            if (bonfireOn)
            {
                StartCoroutine(ToggleTrigger(bonfireOn, "BonfirePopup"));
                bonfireOn = !bonfireOn;
            }
            if (!inventoryOn && !combinationRecipesOn && !buildRecipesOn)
            {
                // 설정창 열기
            }
        }
        /*
        I = Inventory
        Tab = Combination
        B = Build
        G = 건물 상호작용
        각 키를 판별한 후 해당 키에 맞는 Prefab을 Resources에서 로드 후 코루틴 시작
        */
        if (Input.GetButtonDown("Inventory"))
        {
            OpenInterface(ref inventoryOn, "Inventory");
        }
        if (Input.GetButtonDown("Combination"))
        {
            OpenInterface(ref combinationRecipesOn, "CombinationRecipes");
        }
        if (Input.GetButtonDown("Build"))
        {
            OpenInterface(ref buildRecipesOn, "BuildRecipes");
        }
        if(Input.GetKeyDown(KeyCode.G) && !UserStatusManager.Instance.IsMake)
        {
            if (InteractingWithBlacksmith)
            {
                OpenInterface(ref blacksmithOn, "BlacksmithPopup");
            }
            else if(InteractingWithGunsmith) 
            {
                OpenInterface(ref gunsmithOn, "GunsmithPopup");
            }
            else if (interactingWithBonfire)
            {
                OpenInterface(ref bonfireOn, "BonfirePopup");
            }
        }
    }

    public void OpenInterface(ref bool isOn, string prefabName)
    {
        popupOpener.popupPrefab = Resources.Load<GameObject>("Popups/" + prefabName);
        if (Time.time - lastToggleTime > toggleDelay) // 시간 간격을 확인
        {
            StartCoroutine(ToggleTrigger(isOn, prefabName));
            isOn = !isOn;
            lastToggleTime = Time.time; // 최근 토글 시간 업데이트
        }
    }

    public IEnumerator ToggleTrigger(bool isOn, string prefabName)
    {
        if (!isOn)
        {
            popupOpener.OpenPopup();
            Transform canvas = GameObject.Find("Canvas").transform;

            foreach (Transform child in canvas)
            {
                if (child.name.StartsWith("PopupBackground")) // 이름이 PopupBackground로 시작하는 오브젝트들 선택
                {
                    child.transform.position = new Vector3(-2800.0f, 0, 0);
                }
            }
        }
        else if (isOn)
        {
            Transform canvas = GameObject.Find("Canvas").transform;
            Transform Ui = canvas.transform.Find(prefabName + "(Clone)");

            Ui.GetComponent<Popup>().Close();
        }

        yield return new WaitForSeconds(0.1f); // 조절 가능한 시간 지연
    }

}
