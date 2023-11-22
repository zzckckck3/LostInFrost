using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowGoodsInShop : MonoBehaviour
{
    private static ShowGoodsInShop instance = null;
    public static ShowGoodsInShop Instance
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

    // 샵 내 코인 텍스트
    public TextMeshProUGUI myCoin;
    // 샵 내 크리스탈 텍스트
    public TextMeshProUGUI myCrystal;


    void Start()
    {
        UpdateGoodsInShop();
    }

    // 구매시 변경사항 수정
    public void UpdateGoodsInShop()
    {
        myCoin.text = AuthManager.Instance.userData.coin.ToString("#,0"); ;
        myCrystal.text = AuthManager.Instance.userData.crystal.ToString("#,0");
    }
}
