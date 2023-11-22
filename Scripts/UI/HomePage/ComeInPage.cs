using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComeInPage : MonoBehaviour
{
    private static ComeInPage instance = null;
    public static ComeInPage Instance
    {
        get { return instance; }
    }
    
    // 유저 데이터 출력 UI 오브젝트
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI exp;
    public TextMeshProUGUI coin;
    public TextMeshProUGUI crystal;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // 유저 데이터 불러오기
        AuthManager.Instance.ReqMyInfo("Home");
        Debug.Log("home씬"+AuthManager.Instance.userData.email);
    }

    public void ShowUserData()
    {
        nickName.text = AuthManager.Instance.userData.nickname;
        level.text = AuthManager.Instance.userData.level.ToString();
        exp.text = AuthManager.Instance.userData.experience.ToString()+" / 100";
        coin.text = AuthManager.Instance.userData.coin.ToString("#,0");
        crystal.text = AuthManager.Instance.userData.crystal.ToString("#,0");
        SetAvatarImg.Instance.SetImg();
    }

    public void UpdateNicknameUi(string nickname)
    {
        nickName.text = nickname;
    }

    // 유저 재화 업데이트
    public void UpdateUserGoods()
    {
        coin.text = AuthManager.Instance.userData.coin.ToString("#,0");
        crystal.text = AuthManager.Instance.userData.crystal.ToString("#,0");
    }
}
