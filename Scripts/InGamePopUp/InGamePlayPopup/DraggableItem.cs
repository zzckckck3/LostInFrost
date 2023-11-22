using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    private Transform buttonDrag;

    public bool isDragging;
    private Vector2 offset;


    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 활성화
        isDragging = true;
        // inventory의 ButtonDrag라는 게임 오브젝트를 할당
        buttonDrag = transform.parent.Find("ButtonDrag");
        // 활성화 상태로 만듬
        buttonDrag.gameObject.SetActive(true);
        // 인벤토리의 정보를 활용하기 위해 누른 버튼을 인벤토리 매니저에 할당 (맨 밐의 OnDrop에서 사용하기 위함)
        InventoryManager.Instance.draggedItem = this.transform;
        InventoryManager.Instance.draggingItem = buttonDrag;

        // 드래그버튼에 누른 버튼의 아이콘 이미지 할당
        buttonDrag.Find("Icon").GetComponent<Image>().sprite = this.transform.Find("Icon").GetComponent<Image>().sprite;

        // 누른 버튼의 Amount가 활성화 되어 있을때만 드래그버튼의 Amount를 활성화
        if(InventoryManager.Instance.draggedItem.Find("Icon").Find("Amount").gameObject.activeSelf)
        {
            buttonDrag.Find("Icon").Find("Amount").gameObject.SetActive(true);
            buttonDrag.Find("Icon").Find("Amount").Find("Text").GetComponent<TextMeshProUGUI>().text = this.transform.Find("Icon").Find("Amount").Find("Text").GetComponent<TextMeshProUGUI>().text;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그중일 때 buttonDrag의 위치를 마우스에 맞게 이동
        if (isDragging)
        {
            buttonDrag.position = eventData.position + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 상태를 종료하고
        isDragging = false;
        // 드래그버튼의 Amount를 비활성화
        buttonDrag.Find("Icon").Find("Amount").gameObject.SetActive(false);
        // 드래그버튼 비활성화
        buttonDrag.gameObject.SetActive(false);
    }

    // 이놈이 OnEndDrag보다 먼저 실행된다
    public void OnDrop(PointerEventData eventData)
    {
        // TakeWhile : 조건이 True인동안 시퀀스에서 요소를 반환
        // 버튼 이름이 Button XX 이기 때문에 뒤집어서 isDigit만 추출, 추출이 완료되면 다시 뒤집어서 번호를 추출한다.
        string dragStartNumberString = new string(InventoryManager.Instance.draggedItem.name.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
        string dragEndNumberString = new string(eventData.pointerCurrentRaycast.gameObject.name.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
        // String으로 된 번호를 Int형으로 변환. 한줄에 쓰기 너무 길어서 따로 뺐다
        int dragStartNumber;
        int dragEndNumber;
        Int32.TryParse(dragStartNumberString, out dragStartNumber);
        Int32.TryParse(dragEndNumberString, out dragEndNumber);

        if (dragStartNumber >= 0 && dragEndNumber >= 0)
        {
            // 아이템 위치 교환
            InventoryItem temp = InventoryManager.Instance.inventory[dragStartNumber];
            InventoryManager.Instance.inventory[dragStartNumber] = InventoryManager.Instance.inventory[dragEndNumber];
            InventoryManager.Instance.inventory[dragEndNumber] = temp;
        }
    }
}