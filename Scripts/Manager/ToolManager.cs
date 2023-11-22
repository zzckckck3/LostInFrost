using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    public List<ToolData> toolDataList;
    private Dictionary<string, ToolData> toolDictionary;
    private List<InventoryItem> ingredients;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        toolDictionary = new Dictionary<string, ToolData>();
        foreach(var tool in toolDataList)
        {
            toolDictionary[tool.itemName] = tool;
        }
    }

    // 도구이름을 받고 그 도구를 만들수 있는지 체크하는 함수
    public bool CanMake(string toolName)
    {
        ingredients = GetToolData(toolName).ingredients;
        return InventoryManager.Instance.CheckItem(ingredients); ;
    }

    // 도구를 만드는 함수
    public bool NowMake(string toolName)
    {
        if (!CanMake(toolName)) return false;
        InventoryManager.Instance.UseItem(ingredients);
        return true;
    }

    public ToolData GetToolData(string toolName)
    {
        if(toolDictionary.TryGetValue(toolName, out ToolData toolData)) { return toolData; }

        return null;
    }

}
