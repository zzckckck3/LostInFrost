using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class SetBlacksmithRecipe : MonoBehaviour
{
    public static SetBlacksmithRecipe Instance { get; private set; }

    [SerializeField]
    private GameObject combinationListButtonPrefab;

    public ItemData[] combinationList;
    [SerializeField]
    private Transform combinationListContent;

    [SerializeField]
    private GameObject needIngredientButtonPrefab;
    [SerializeField]
    private Transform needIngredientImage;
    [SerializeField]
    private Transform needIngredientExplain;
    [SerializeField]
    private Transform needIngredientContent;

    public CleanButton nowClicked;
    public int nowIndex;

    [SerializeField]
    private CleanButton createButton;

    private bool preIsRoot;
    private bool canCreate = true;

    void Start()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        MakeCombinationList();
        needIngredientButtonPrefab.SetActive(false);
        preIsRoot = false;
    }

    void Update()
    {
        if (UserStatusManager.Instance.IsRoot && !preIsRoot)
        {
            preIsRoot = true;
            OnCombinationButtonClicked(nowClicked, nowIndex);
        }
        if (!UserStatusManager.Instance.IsRoot)
        {
            preIsRoot = false;
        }
        if (UserStatusManager.Instance.IsMake)
        {
            createButton.gameObject.SetActive(false);
        }
    }

    private void MakeCombinationList()
    {
        combinationListButtonPrefab.SetActive(true);
        for (int i = 0; i < combinationList.Length; i++)
        {
            GameObject buttonGO = Instantiate(combinationListButtonPrefab, combinationListContent);
            Transform buttonIcon = buttonGO.transform.Find("Icon");
            buttonIcon.gameObject.GetComponent<Image>().sprite = combinationList[i].Icon;

            CleanButton button = buttonGO.GetComponent<CleanButton>();
            int index = i; // 클로저로 인덱스 유지
            button.onClick.AddListener(() => OnCombinationButtonClicked(button, index));
            if (i == 0)
            {
                nowClicked = button;
                nowIndex = index;
            }
        }
        // 초기 화면 띄우기
        OnCombinationButtonClicked(nowClicked, nowIndex);
        combinationListButtonPrefab.SetActive(false);
    }

    private void OnCombinationButtonClicked(CleanButton clickedButton, int index)
    {
        nowClicked = clickedButton;
        nowIndex = index;
        canCreate = true;

        foreach (Transform child in needIngredientContent)
        {
            if (child.gameObject != needIngredientButtonPrefab)
            {
                Destroy(child.gameObject);
            }
        }
        ItemData selectedItem = combinationList[index];
        needIngredientImage.GetComponent<Image>().sprite = selectedItem.Icon;
        needIngredientExplain.GetComponent<TextMeshProUGUI>().text = selectedItem.itemDescription;
        if (selectedItem.itemType == ItemType.Ingredient
            || selectedItem.itemType == ItemType.Tool
            || selectedItem.itemType == ItemType.Consumable)
        {
            if (selectedItem.itemType == ItemType.Ingredient)
            {
                IngredientData selected = (IngredientData)selectedItem;

                for (int i = 0; i < selected.ingredients.Count; i++)
                {
                    GameObject buttonGO = Instantiate(needIngredientButtonPrefab, needIngredientContent);
                    buttonGO.SetActive(true);
                    buttonGO.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = selected.ingredients[i].itemData.Icon;
                    buttonGO.transform.Find("HaveText").GetComponent<TextMeshProUGUI>().text = InventoryManager.Instance.NumberItem(selected.ingredients[i].itemData).ToString();
                    buttonGO.transform.Find("NeedText").GetComponent<TextMeshProUGUI>().text = selected.ingredients[i].quantity.ToString();
                }
                for (int i = 0; i < selected.ingredients.Count; i++)
                {
                    if (InventoryManager.Instance.NumberItem(selected.ingredients[i].itemData) < selected.ingredients[i].quantity)
                    {
                        canCreate = false;
                        break;
                    }
                }
            }
            else if (selectedItem.itemType == ItemType.Tool)
            {
                ToolData selected = (ToolData)selectedItem;

                for (int i = 0; i < selected.ingredients.Count; i++)
                {
                    GameObject buttonGO = Instantiate(needIngredientButtonPrefab, needIngredientContent);
                    buttonGO.SetActive(true);
                    buttonGO.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = selected.ingredients[i].itemData.Icon;
                    buttonGO.transform.Find("HaveText").GetComponent<TextMeshProUGUI>().text = InventoryManager.Instance.NumberItem(selected.ingredients[i].itemData).ToString();
                    buttonGO.transform.Find("NeedText").GetComponent<TextMeshProUGUI>().text = selected.ingredients[i].quantity.ToString();
                }
                for (int i = 0; i < selected.ingredients.Count; i++)
                {
                    if (InventoryManager.Instance.NumberItem(selected.ingredients[i].itemData) < selected.ingredients[i].quantity)
                    {
                        canCreate = false;
                        break;
                    }
                }
            }
            else if (selectedItem.itemType == ItemType.Consumable)
            {
                ConsumableData selected = (ConsumableData)selectedItem;

                for (int i = 0; i < selected.ingredients.Count; i++)
                {
                    GameObject buttonGO = Instantiate(needIngredientButtonPrefab, needIngredientContent);
                    buttonGO.SetActive(true);
                    buttonGO.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = selected.ingredients[i].itemData.Icon;
                    buttonGO.transform.Find("HaveText").GetComponent<TextMeshProUGUI>().text = InventoryManager.Instance.NumberItem(selected.ingredients[i].itemData).ToString();
                    buttonGO.transform.Find("NeedText").GetComponent<TextMeshProUGUI>().text = selected.ingredients[i].quantity.ToString();
                }
                for (int i = 0; i < selected.ingredients.Count; i++)
                {
                    if (InventoryManager.Instance.NumberItem(selected.ingredients[i].itemData) < selected.ingredients[i].quantity)
                    {
                        canCreate = false;
                        break;
                    }
                }
            }

            if (canCreate)
            {
                //createButton.GetComponent<CleanButton>().onClick.RemoveAllListeners();
                createButton.gameObject.SetActive(true);
            }
            else
            {
                createButton.gameObject.SetActive(false);
            }
        }
    }

    public void Create()
    {
        ItemData selectedItem = combinationList[nowIndex];
        if (selectedItem.itemType == ItemType.Ingredient)
        {
            IngredientData selected = (IngredientData)selectedItem;
            List<InventoryItem> items = new List<InventoryItem>();
            for (int i = 0; i < selected.ingredients.Count; i++)
            {
                InventoryItem item = new InventoryItem(selected.ingredients[i].itemData, selected.ingredients[i].quantity, selected.ingredients[i].itemDurability);
                items.Add(item);
            }

            if (InventoryManager.Instance.CheckItem(items))
            {
                UserStatusManager.Instance.IsMake = true;
                StartCoroutine(CreateItemAfterDelay(2f, selected, items)); // 3초 후에 CreateItemCoroutine 코루틴을 시작합니다.
            }
            else
            {
                Debug.LogError("Create Failed!!!");
            }
        }
        else if (selectedItem.itemType == ItemType.Tool)
        {
            ToolData selected = (ToolData)selectedItem;
            List<InventoryItem> items = new List<InventoryItem>();
            for (int i = 0; i < selected.ingredients.Count; i++)
            {
                InventoryItem item = new InventoryItem(selected.ingredients[i].itemData, selected.ingredients[i].quantity, selected.ingredients[i].itemDurability);
                items.Add(item);
            }

            if (InventoryManager.Instance.CheckItem(items))
            {
                UserStatusManager.Instance.IsMake = true;
                StartCoroutine(CreateItemAfterDelay(2f, selected, items)); // 3초 후에 CreateItemCoroutine 코루틴을 시작합니다.
            }
            else
            {
                Debug.LogError("Create Failed!!!");
            }
        }
        else if (selectedItem.itemType == ItemType.Consumable)
        {
            ConsumableData selected = (ConsumableData)selectedItem;
            List<InventoryItem> items = new List<InventoryItem>();
            for (int i = 0; i < selected.ingredients.Count; i++)
            {
                InventoryItem item = new InventoryItem(selected.ingredients[i].itemData, selected.ingredients[i].quantity, selected.ingredients[i].itemDurability);
                items.Add(item);
            }

            if (InventoryManager.Instance.CheckItem(items))
            {
                UserStatusManager.Instance.IsMake = true;
                StartCoroutine(CreateItemAfterDelay(2f, selected, items)); // 3초 후에 CreateItemCoroutine 코루틴을 시작합니다.
            }
            else
            {
                Debug.LogError("Create Failed!!!");
            }
        }
    }

    private IEnumerator CreateItemAfterDelay(float delay, IngredientData ingredient, List<InventoryItem> items)
    {
        canCreate = false;
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기합니다.
        UserStatusManager.Instance.IsMake = false;
        OnCombinationButtonClicked(nowClicked, nowIndex); // 다시 클릭

        GameObject ingredientPrefab = Resources.Load<GameObject>("Ingredient/" + ingredient.itemName);

        // Ground에만 부딛힐 수 있도록 레이어마스크 지정
        int groundLayerMask = LayerMask.GetMask("Ground");
        Camera mainCamera = Camera.main;

        // 카메라 중앙 계산
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            // Raycast로 감지한 지면의 위치를 얻음
            Vector3 spawnPosition = hit.point;

            if (InventoryManager.Instance.UseItem(items))
            {
                InventoryItem temp = new InventoryItem(ingredient, ingredient.itemQuantity, 1);
                NetworkManager.Instance.DropIngredient(temp, spawnPosition);
            }
            else
            {
                yield break;
            }
        }
        else
        {
            yield break;
        }
    }

    private IEnumerator CreateItemAfterDelay(float delay, ToolData tool, List<InventoryItem> items)
    {
        canCreate = false;
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기합니다.
        UserStatusManager.Instance.IsMake = false;
        OnCombinationButtonClicked(nowClicked, nowIndex); // 다시 클릭

        GameObject toolPrefab = Resources.Load<GameObject>("Ingredient_Tool/" + tool.itemName);

        // Ground에만 부딛힐 수 있도록 레이어마스크 지정
        int groundLayerMask = LayerMask.GetMask("Ground");
        Camera mainCamera = Camera.main;

        // 카메라 중앙 계산
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            // Raycast로 감지한 지면의 위치를 얻음
            Vector3 spawnPosition = hit.point;

            if (InventoryManager.Instance.UseItem(items))
            {
                InventoryItem temp = new InventoryItem(tool, 1, tool.toolDurability);
                NetworkManager.Instance.DropTool(temp, spawnPosition);
            }
            else
            {
                yield break;
            }
        }
        else
        {
            yield break;
        }
    }

    private IEnumerator CreateItemAfterDelay(float delay, ConsumableData consumable, List<InventoryItem> items)
    {
        canCreate = false;
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기합니다.
        UserStatusManager.Instance.IsMake = false;
        OnCombinationButtonClicked(nowClicked, nowIndex); // 다시 클릭

        GameObject consumablePrefab = Resources.Load<GameObject>("Ingredient_Consumable/" + consumable.itemName);

        // Ground에만 부딛힐 수 있도록 레이어마스크 지정
        int groundLayerMask = LayerMask.GetMask("Ground");
        Camera mainCamera = Camera.main;

        // 카메라 중앙 계산
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            // Raycast로 감지한 지면의 위치를 얻음
            Vector3 spawnPosition = hit.point;

            if (InventoryManager.Instance.UseItem(items))
            {
                InventoryItem temp = new InventoryItem(consumable, consumable.itemQuantity, 1);
                NetworkManager.Instance.DropConsumable(temp, spawnPosition);
            }
            else
            {
                yield break;
            }
        }
        else
        {
            yield break;
        }
    }

    private void OnDestroy()
    {
        UserStatusManager.Instance.IsMake = false;
        StopAllCoroutines();
    }
}
