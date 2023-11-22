using ARPGFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ApiResponse;

public class CostumeManager : MonoBehaviour
{
    private static CostumeManager instance = null;
    public static CostumeManager Instance
    {
        get { return instance; }
    }

    [System.Serializable]
    public class Costume 
    {
        public int costumeSeq;
        public string costumeName;
        public string costumeImage;
        public string costumeGrade;
        public int costumeCoinPrice;
        public int costumeCrystalPrice;

        public Costume(int costumeSeq, string costumeName, string costumeImage, string costumeGrade,
                        int costumeCoinPrice, int costumeCrystalPrice)
        {
            this.costumeSeq = costumeSeq;
            this.costumeName = costumeName;
            this.costumeImage = costumeImage;
            this.costumeGrade = costumeGrade;
            this.costumeCoinPrice = costumeCoinPrice;
            this.costumeCrystalPrice = costumeCrystalPrice;
        }
    }

    
    // 코스튬 리스트
    public List<Costume> costumeList = new List<Costume>();


    public void Awake()
    {
        if (instance == null)
        {
            // 이 GameManager 오브젝트가 다른 씬으로 전환될 때 파괴되지 않도록 함
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // 이미 존재하는 GameManager 오브젝트가 있으므로 이 인스턴스를 파괴
            Destroy(gameObject);
        }
        ReqCostumeList();
    }

    // api--------------------------------------------------------------------
    // 코스튬 리스트 요청
    public void ReqCostumeList()
    {
        CostumeSystem.Instance.CallReqCostumList();
    }

    // 코스튬 뽑기 진행 요청
    public void ReqPurchaseCostume(string type)
    {
        CostumeSystem.Instance.CallReqPurchaseCostume(AuthManager.Instance.accessToken, type);
    }


    // api 콜백---------------------------------------------------------------
    // 코스튬 데이터 응답
    public void ResCostumeList(List<Costume> costumes)
    {
        costumeList = costumes;
        Debug.Log("코스튬갯수: " + costumes.Count);
        Debug.Log("0번째 코스튬이름: "+ costumeList[0].costumeName);
    }

    // 코스튬 뽑기 응답처리
    public void ResPurchaseCostume(AuthManager.MyCostume resultCostume, int result, string type)
    {

        // 중복 뽑음
        if (result == 0)
        {
            // 뽑기 결과 처리 : 중복
            BoxOpen.Instance.Open(resultCostume.costumeGrade);// 상자 오픈 이팩트 시작 : 3초
            ShowResultCos.Instance.ResResult(resultCostume, result, type); // 뽑기 결과 전달
        }

        // 뽑기 성공
        else if(result == 1)
        {
            // 뽑은 코스튬 저장.
            AuthManager.Instance.myCostumes.Add(resultCostume);

            // 뽑기 결과 처리 : 새로운 코스튬
            BoxOpen.Instance.Open(resultCostume.costumeGrade);// 상자 오픈 이팩트 시작 : 3초
            ShowResultCos.Instance.ResResult(resultCostume, result, type); // 뽑기 결과 전달
        }

        // 에러 발생
        else
        {

        }
    }


}
