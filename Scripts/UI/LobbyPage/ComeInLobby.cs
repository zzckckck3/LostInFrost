using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComeInLobby : MonoBehaviour
{
    private static ComeInLobby instance = null;
    public static ComeInLobby Instance
    {
        get { return instance; }
    }

    // 유저 데이터 출력 UI 오브젝트
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI exp;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // 유저 데이터 불러오기
        AuthManager.Instance.ReqMyInfo("Lobby");
    }

    public void ShowUserData()
    {
        nickName.text = AuthManager.Instance.userData.nickname;
        level.text = AuthManager.Instance.userData.level.ToString();
        exp.text = AuthManager.Instance.userData.experience.ToString() + " / 100";
    }

}
