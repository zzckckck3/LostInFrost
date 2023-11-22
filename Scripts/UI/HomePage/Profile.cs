using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;

public class Profile : MonoBehaviour
{
    private static Profile instance = null;
    public static Profile Instance
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

    public TextMeshProUGUI nowNickname; // 현재 닉네임 플레이스홀더
    public TextMeshProUGUI nowStatusMessage; // 현재 메세지 플레이스홀더
    public TextMeshProUGUI nowLevel; // 현재 레벨
    public TextMeshProUGUI nowExp; // 현재 경험치
    public TextMeshProUGUI nowCoin; // 현재 코인
    public TextMeshProUGUI nowCrystal; // 현재 크리스탈
    public TMP_InputField inputNickname; // 입력된 닉네임
    public TMP_InputField inputMessage; // 입력된 메세지
    public GameObject passwordCheckUi; // 비밀번호 확인 ui
    public TMP_InputField inputPassword; // 입력된 비밀번호


    private void Start()
    {
        nowNickname.text = AuthManager.Instance.userData.nickname;
        if (AuthManager.Instance.userData.message.Length < 1)
            nowStatusMessage.text = "상태 메세지 입력..";
        else
            nowStatusMessage.text = AuthManager.Instance.userData.message;

        nowLevel.text = AuthManager.Instance.userData.level + "";

        nowExp.text = AuthManager.Instance.userData.experience + "";
        nowCoin.text = AuthManager.Instance.userData.coin.ToString("#,0"); ;
        nowCrystal.text = AuthManager.Instance.userData.crystal.ToString("#,0"); ;
    }

    // 비밀번호 확인창 열기
    public void OnClickOpenPasswordCheck()
    {
        // 입력된 닉네임이 2자리 이상일때만 on
        if (inputNickname.text.Length > 2)
        {
            passwordCheckUi.SetActive(true);
        }
        else
        {
            // 실패 안내 팝업
            ResultInfo.Instance.LaunchNotification(false, "닉네임을 2자리 이상 입력하세요!");
        }
    }

    //닉네임 가져오기
    public string GetNickname()
    {
        return inputNickname.text;
    }
    // 메세지 업데이트 호출
    public void OnClickUpdateMessage()
    {
        string message = inputMessage.text;
        AuthManager.Instance.ReqUpdateUserMessage(message);
    }

    // 업데이트 후 ui 수정
    public void UpdateMessage(string message)
    {
        inputMessage.text = "";
        nowStatusMessage.text = message;
    }

    // 닉네임 업데이트 후 ui 수정.
    public void UpdateNickname(string nickname)
    {
        inputNickname.text = "";
        nowNickname.text = nickname;
    }
    // 비밀번호 검증 호출
    public void OnClickCheckPassword()
    {
        if (inputPassword.text.Length < 1)
        {
            ResultInfo.Instance.LaunchNotification(false, "비밀번호를 입력하세요!");
        }
        else
        {
            AuthManager.Instance.ReqCheckUserPassword(inputPassword.text);
        }
    }

    // 비밀번호 검증 후 ui 수정.
    public void AfterCheck(bool isScuccess)
    {
        inputPassword.text = "";
        if (isScuccess)
        {
            passwordCheckUi.SetActive(false);
        }
    }
}
