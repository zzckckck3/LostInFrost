using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableStatus : MonoBehaviour
{
    public ConsumableData consumableData;
    public string consumableName;
    public string consumableExplanation;
    public ItemType itemType;
    public int consumableQuantity;
    public float rotationSpeed = 50f; // 회전 속도

    // Start is called before the first frame update
    void Start()
    {
        consumableName = consumableData.itemName;
        consumableExplanation = consumableData.itemDescription;
        itemType = consumableData.itemType;
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); // Y축을 중심으로 회전
    }
}
