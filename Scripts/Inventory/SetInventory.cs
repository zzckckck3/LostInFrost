using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetInventory : MonoBehaviour
{
    public static SetInventory Instance { get; private set; }

    [SerializeField]
    private Transform viewPort;
    
    public GameObject buttonPrefab;

    public GameObject[] buttonList;

    [SerializeField]
    private float firstXPosition;
    [SerializeField]
    private float firstYPosition;

    private float xPosition;
    private float yPosition;

    public GameObject buttonDrag;

    void Start()
    {
        if (Instance != null)
        {
            Instance = this;
        }

        // buttonList에 maxInventorySlot만큼 크기 배정
        buttonList = new GameObject[InventoryManager.Instance.maxInventorySlot];
        xPosition = firstXPosition;
        yPosition = firstYPosition;

        // buttonList에 할당 및 생성
        MakeButton();

        buttonDrag = Instantiate(buttonPrefab, viewPort);
        buttonDrag.GetComponent<Image>().raycastTarget = false;
        buttonDrag.name = "ButtonDrag";
        buttonDrag.SetActive(false);

        // buttonPrefab삭제
        Destroy(buttonPrefab);
    }

    private void MakeButton()
    {
        for (int i = 0; i < InventoryManager.Instance.maxInventorySlot; i++)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, viewPort);

            RectTransform buttonTransform = buttonGO.GetComponent<RectTransform>();
            buttonTransform.anchoredPosition = new Vector2(xPosition, yPosition);

            xPosition += 150.0f; // 다음 열로 이동
            if (i == 4)
            {
                xPosition = firstXPosition; // x 위치 초기화
                yPosition -= 190.0f; // 다음 행으로 이동
            }
            else if (i > 5 && i % 5 == 4)
            {
                xPosition = firstXPosition; // x 위치 초기화
                yPosition -= 150.0f; // 다음 행으로 이동
            }

            buttonGO.name = "Button " + i;
            buttonList[i] = buttonGO;
        }

    }

    void Update()
    {
        //Debug.Log(InventoryManager.Instance.inventory[0].itemData.itemName);
        for (int i = 0; i < buttonList.Length; i++)
        {
            Transform icon = buttonList[i].transform.Find("Icon");
            Slider slider = buttonList[i].transform.Find("Slider").GetComponent<Slider>();
            if(icon != null)
            {
                Transform amount = icon.transform.Find("Amount");
                Image iconImage = icon.GetComponent<Image>();
                if (InventoryManager.Instance.inventory[i] != null && InventoryManager.Instance.inventory[i].itemData != null)
                {
                    iconImage.sprite = InventoryManager.Instance.inventory[i].itemData.Icon;
                    amount.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = InventoryManager.Instance.inventory[i].quantity.ToString();
                    amount.gameObject.SetActive(true);
                    if (InventoryManager.Instance.inventory[i].itemData.itemType == ItemType.Tool)
                    {
                        slider.gameObject.SetActive(true);
                        ItemData nowData = InventoryManager.Instance.inventory[i].itemData;
                        ToolData nowToolData = (ToolData)nowData;
                        slider.GetComponent<Slider>().value = (float)InventoryManager.Instance.inventory[i].itemDurability / (float)nowToolData.toolDurability;
                    }
                    else
                    {
                        slider.gameObject.SetActive(false);
                    }
                }
                else
                {
                    iconImage.sprite = null;
                    amount.gameObject.SetActive(false);
                    slider.gameObject.SetActive(false);
                }
            }
        }
    }
    
}
