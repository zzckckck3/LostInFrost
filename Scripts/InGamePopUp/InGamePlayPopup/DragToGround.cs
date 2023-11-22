using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragToGround : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private Camera mainCamera;
    private int draggedNumber;

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return; // 카메라가 여전히 없으면 나머지 코드는 실행하지 않음
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject.name == "CanvasBackground")
        {
            string draggedNumberString = new string(InventoryManager.Instance.draggedItem.name.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
            Int32.TryParse(draggedNumberString, out draggedNumber);
            InventoryItem dropItem = InventoryManager.Instance.inventory[draggedNumber];
            // 떨군 아이템 종류가 '재료' 라면
            if (dropItem.itemData.itemType == ItemType.Ingredient)
            {
                DropInventoryIngredient((IngredientData)dropItem.itemData, dropItem.quantity);
            }
            else if (dropItem.itemData.itemType == ItemType.Tool)
            {
                DropInventoryTool(dropItem.itemData, dropItem.quantity, dropItem.itemDurability);
            }
            else if(dropItem.itemData.itemType == ItemType.Consumable)
            {
                DropInventoryConsumable(dropItem.itemData, dropItem.quantity);
            }
        } 
        else
        {
            return;
        }
    }

    public void DropInventoryIngredient(IngredientData ingredientData, int quantity)
    {
        // Ground에만 부딛힐 수 있도록 레이어마스크 지정
        int groundLayerMask = LayerMask.GetMask("Ground");

        // 카메라 중앙 계산 (왜 캐릭터로 안했냐 >> 캐릭터가 여러명일 수 있어서 PlayerECM으로 대상을 지정하는건 좀 아닌것 같다)
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            InventoryItem temp = new InventoryItem(ingredientData, quantity);
            // Raycast로 감지한 지면의 위치를 얻음
            Vector3 spawnPosition = hit.point;
            NetworkManager.Instance.DropIngredient(temp, spawnPosition);
            InventoryManager.Instance.inventory[draggedNumber] = null;
        }
        else
        {
            return;
        }
        
    }

    public void DropInventoryTool(ItemData toolData, int quantity, int itemDurability)
    {

        // Ground에만 부딛힐 수 있도록 레이어마스크 지정
        int groundLayerMask = LayerMask.GetMask("Ground");

        // 카메라 중앙 계산 (왜 캐릭터로 안했냐 >> 캐릭터가 여러명일 수 있어서 PlayerECM으로 대상을 지정하는건 좀 아닌것 같다)
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            InventoryItem temp = new InventoryItem(toolData, quantity, itemDurability);

            // Raycast로 감지한 지면의 위치를 얻음
            Vector3 spawnPosition = hit.point;
            NetworkManager.Instance.DropTool(temp, spawnPosition);
            InventoryManager.Instance.inventory[draggedNumber] = null;
        }
        else
        {
            return;
        }

    }

    public void DropInventoryConsumable(ItemData consumableData, int quantity)
    {
        // Ground에만 부딛힐 수 있도록 레이어마스크 지정
        int groundLayerMask = LayerMask.GetMask("Ground");

        // 카메라 중앙 계산 (왜 캐릭터로 안했냐 >> 캐릭터가 여러명일 수 있어서 PlayerECM으로 대상을 지정하는건 좀 아닌것 같다)
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            InventoryItem temp = new InventoryItem(consumableData, quantity);
            // Raycast로 감지한 지면의 위치를 얻음
            Vector3 spawnPosition = hit.point;
            NetworkManager.Instance.DropConsumable(temp, spawnPosition);
            InventoryManager.Instance.inventory[draggedNumber] = null;
        }
        else
        {
            return;
        }

    }
}
