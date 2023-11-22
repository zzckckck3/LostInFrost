using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodAxe : MonoBehaviour
{
    public ToolData toolData;
    public string toolName;
    public string explanation;
    public ToolType type;
    public int durability;
    public int attackPower;
    public int interactivePower;

    private void Start()
    {
        toolName = toolData.itemName;
        explanation = toolData.itemDescription;
        type = toolData.toolType;
        durability = toolData.toolDurability;
        attackPower = toolData.toolAttackPower;
        interactivePower = toolData.toolInteractivePower;
    }
}
