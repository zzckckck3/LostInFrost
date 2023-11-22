using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UltimateClean;
using UnityEngine;

public class ShopClickBuy : MonoBehaviour
{
    private static ShopClickBuy instance = null;
    public static ShopClickBuy Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private int costCoin = 2000;
    private int costCrystal = 20;

    protected Canvas m_canvas;
    protected GameObject m_popup;

    public GameObject resultCostume;
    public GameObject characterUi;

    //  구매 확인창
    public GameObject checkUi;

    protected void Start()
    {
        m_canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // 코인으로 구매 클릭
    public void OnclickBuyCoin()
    {
        // 코인이 부족할때
        if (AuthManager.Instance.userData.coin < costCoin)
        {
            ResultInfo.Instance.LaunchNotification(false, "코인이 부족합니다.");
        }
        else
        {
            // 등급별 확룔에 따른 뽑힐 등급 지정.
            // 구매 api 호출
            OpenPopupResultCostume("coin");
            Debug.Log("구매완료");
        }
    }

    // 크리스탈로 구매 클릭
    public void OnclickBuyCrystal()
    {
        // 크리스탈이 부족할때
        if (AuthManager.Instance.userData.crystal < costCrystal)
        {
            ResultInfo.Instance.LaunchNotification(false, "크리스탈이 부족합니다.");
        }
        else
        {
            // 등급별 확룔에 따른 뽑힐 등급 지정.
            // 구매 api 호출
            OpenPopupResultCostume("crystal");
            Debug.Log("구매완료");
        }
    }

    // 뽑기 결과처리 : 뽑은 코스튬 출력
    public virtual void OpenPopupResultCostume(string type)
    {
        // 뽑기 결과창 출력.
        m_popup = Instantiate(resultCostume, m_canvas.transform, false);
        m_popup.SetActive(true);
        m_popup.transform.localScale = Vector3.zero;
        m_popup.GetComponent<Popup>().Open();


        // 뽑기 시작 : api 호출
        CostumeManager.Instance.ReqPurchaseCostume(type);

    }

    public int CostCoin
    {
        get { return costCoin; }
    }

    public int CostCrystal
    {
        get { return costCrystal; }
    }
}
