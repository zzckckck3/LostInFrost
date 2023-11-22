using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolStatus : MonoBehaviour
{
    public ToolData toolData;
    public string toolName;
    public string toolExplanation;
    public ItemType itemType;
    public int toolDurability;
    public float rotationSpeed = 50f; // 회전 속도

    // Start is called before the first frame update
    void Start()
    {
        toolName = toolData.itemName;
        toolExplanation = toolData.itemDescription;
        itemType = toolData.itemType;
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); // Y축을 중심으로 회전
    }
}
