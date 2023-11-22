using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class SetBuildingRecipe : MonoBehaviour
{
    public static SetBuildingRecipe Instance { get; private set; }

    [SerializeField]
    private GameObject buildingListButtonPrefab;

    public BuildingData[] buildingList;
    [SerializeField]
    private Transform buildingListContent;

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

    void Start()
    {
        if (Instance != null)
        {
            Instance = this;
        }
        MakebuildingList();
        needIngredientButtonPrefab.SetActive(false);
        preIsRoot = false;
    }

    void Update()
    {
        if (UserStatusManager.Instance.IsRoot && !preIsRoot)
        {
            preIsRoot = true;
            OnBuildingButtonClicked(nowClicked, nowIndex);
        }
        if (!UserStatusManager.Instance.IsRoot)
        {
            preIsRoot = false;
        }
    }

    private void MakebuildingList()
    {
        buildingListButtonPrefab.SetActive(true);
        for (int i = 0; i < buildingList.Length; i++)
        {
            GameObject buttonGO = Instantiate(buildingListButtonPrefab, buildingListContent);
            Transform buttonIcon = buttonGO.transform.Find("Icon");
            buttonIcon.gameObject.GetComponent<Image>().sprite = buildingList[i].Icon;

            CleanButton button = buttonGO.GetComponent<CleanButton>();
            int index = i; // 클로저로 인덱스 유지
            button.onClick.AddListener(() => OnBuildingButtonClicked(button, index));
            if (i == 0)
            {
                nowClicked = button;
                nowIndex = index;
            }
        }
        // 초기 화면 띄우기
        OnBuildingButtonClicked(nowClicked, nowIndex);
        buildingListButtonPrefab.SetActive(false);
    }

    private void OnBuildingButtonClicked(CleanButton clickedButton, int index)
    {
        nowClicked = clickedButton;
        nowIndex = index;

        foreach (Transform child in needIngredientContent)
        {
            if (child.gameObject != needIngredientButtonPrefab)
            {
                Destroy(child.gameObject);
            }
        }
        BuildingData selectedBuilding = buildingList[index];
        needIngredientImage.GetComponent<Image>().sprite = selectedBuilding.Icon;
        needIngredientExplain.GetComponent<TextMeshProUGUI>().text = selectedBuilding.buildingExplanation;

        for (int i = 0; i < selectedBuilding.ingredients.Count; i++)
        {
            GameObject buttonGO = Instantiate(needIngredientButtonPrefab, needIngredientContent);
            buttonGO.SetActive(true);
            buttonGO.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = selectedBuilding.ingredients[i].itemData.Icon;
            buttonGO.transform.Find("HaveText").GetComponent<TextMeshProUGUI>().text = InventoryManager.Instance.NumberItem(selectedBuilding.ingredients[i].itemData).ToString();
            buttonGO.transform.Find("NeedText").GetComponent<TextMeshProUGUI>().text = selectedBuilding.ingredients[i].quantity.ToString();
        }

        bool canCreate = true;
        for (int i = 0; i < selectedBuilding.ingredients.Count; i++)
        {
            if (InventoryManager.Instance.NumberItem(selectedBuilding.ingredients[i].itemData) < selectedBuilding.ingredients[i].quantity)
            {
                canCreate = false;
                break;
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

    public void Build()
    {
        BuildingData selectedBuilding = buildingList[nowIndex];

        List<InventoryItem> items = new List<InventoryItem>();
        for (int i = 0; i < selectedBuilding.ingredients.Count; i++)
        {
            InventoryItem item = new InventoryItem(selectedBuilding.ingredients[i].itemData,
                                                   selectedBuilding.ingredients[i].quantity,
                                                   selectedBuilding.ingredients[i].itemDurability);
            items.Add(item);
        }

        if (UserStatusManager.Instance.CanBuild && InventoryManager.Instance.CheckItem(items))
        {
            UserStatusManager.Instance.IsBuild = true;
            BuildingManager.Instance.NowBuilding = selectedBuilding.buildingName;
            KeyManager.Instance.StartCoroutine(KeyManager.Instance.ToggleTrigger(KeyManager.Instance.BuildRecipesOn,
                                                                                 "BuildRecipes"));
            if(KeyManager.Instance.BuildRecipesOn)
            {
                KeyManager.Instance.BuildRecipesOn = !KeyManager.Instance.BuildRecipesOn;
            }
        }
        else
        {
            Debug.LogError("Build Failed!");
        }
    }
}
