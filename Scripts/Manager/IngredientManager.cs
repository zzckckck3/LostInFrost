using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static IngredientManager Instance { get; private set; }

    public List<IngredientData> ingredientDataList; // Unity Editor에서 할당
    private Dictionary<string, IngredientData> ingredientDictionary;

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ingredientDictionary = new Dictionary<string, IngredientData>();
        foreach (var ingredient in ingredientDataList)
        {
            ingredientDictionary[ingredient.itemName] = ingredient;
        }
    }

    // 외부에서 건물 데이터에 접근하기 위한 메서드
    public IngredientData GetIngredientgData(string ingredientName)
    {
        if (ingredientDictionary.TryGetValue(ingredientName, out IngredientData ingredientData))
        {
            return ingredientData;
        }

        return null;
    }
}
