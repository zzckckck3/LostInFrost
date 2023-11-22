using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStatus : MonoBehaviour
{
    public IngredientData ingredientData;
    public string ingredientName;
    public string ingredientExplanation;
    public ItemType itemType;
    public int ingredientQuantity;
    public float rotationSpeed = 50f; // 회전 속도

    // Start is called before the first frame update
    void Start()
    {
        ingredientName = ingredientData.itemName;
        ingredientExplanation = ingredientData.itemDescription;
        itemType = ingredientData.itemType;
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); // Y축을 중심으로 회전
    }
}
