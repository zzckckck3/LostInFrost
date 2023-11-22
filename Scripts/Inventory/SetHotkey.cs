using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetHotkey : MonoBehaviour
{
    [SerializeField]
    private GameObject[] hotkeyList;

    void Update()
    {
        for (int i = 0; i < hotkeyList.Length; i++)
        {
            Transform icon = hotkeyList[i].transform.Find("Icon");
            Slider slider = hotkeyList[i].transform.Find("Slider").GetComponent<Slider>();
            if (icon != null)
            {
                Transform amount = icon.transform.Find("Amount");
                Image iconImage = icon.GetComponent<Image>();
                if (InventoryManager.Instance.inventory[i] != null && InventoryManager.Instance.inventory[i].itemData != null)
                {
                    iconImage.color = new Color32(255, 255, 255, 255);
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
                }
                else
                {
                    iconImage.sprite = null;
                    iconImage.color = new Color32(50, 70, 101, 255);
                    amount.gameObject.SetActive(false);
                    slider.gameObject.SetActive(false);
                }
            }
        }
    }
}
