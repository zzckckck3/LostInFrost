using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static BuildingManager Instance { get; private set; }

    public List<BuildingData> buildingDataList; // Unity Editor에서 할당
    private Dictionary<string, BuildingData> buildingDictionary;
    List<InventoryItem> ingredients;

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

        buildingDictionary = new Dictionary<string, BuildingData>();
        foreach (var building in buildingDataList)
        {
            buildingDictionary[building.buildingName] = building;
        }
        nowBuilding = "";
    }

    private void Start()
    {
        ingredients = new List<InventoryItem>();
    }

    // 외부에서 건물 데이터에 접근하기 위한 메서드
    public BuildingData GetBuildingData(string buildingName)
    {
        if (buildingDictionary.TryGetValue(buildingName, out BuildingData buildingData))
        {
            return buildingData;
        }

        return null;
    }



    // 건물이름을 받고 그 건물을 지을 수 있는지 체크하는 함수
    public bool CanBuild(string buildingName)
    {
        ingredients = GetBuildingData(buildingName).ingredients;
        return InventoryManager.Instance.CheckItem(ingredients); ;
    }

    // 건물 이름을 받고 그 건물을 지으려고 준비하는 함수
    public bool PreBuild(string buildingName)
    {
        if (!CanBuild(buildingName)) return false;
        nowBuilding = buildingName;
        return true;
    }

    // 건물을 짓는 함수
    public bool NowBuild(string buildingName)
    {
        if (!CanBuild(buildingName)) return false;
        InventoryManager.Instance.UseItem(ingredients);
        return true;
    }


    private string nowBuilding;
    public string NowBuilding
    {
        get { return nowBuilding;}
        set { nowBuilding = value;}
    }
}