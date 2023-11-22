using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TMPro;
using UltimateClean;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    private static AuthManager instance = null;
    public static AuthManager Instance
    {
        get { return instance; }
    }

    // 유저 에세스 토큰
    public string accessToken = null;

    [System.Serializable]
    public class UserData
    {
        public string email;
        public string nickname;
        public int level;
        public int experience;
        public long crystal;
        public long coin;
        public long costumeSeq;
        public string costumeName;
        public string costumeImage;
        public string costumeGrade;
        public string message;

        // 기본 생성자
        public UserData() {}
        public UserData(string email, string nickname, int level, int experience,
                        long crystal, long coin, long costumeSeq,
                        string costumeName, string costumeImage, string costumeGrade, string message)
        {
            this.email = email;
            this.nickname = nickname;
            this.level = level;
            this.experience = experience;
            this.crystal = crystal;
            this.coin = coin;
            this.costumeSeq = costumeSeq;
            this.costumeName = costumeName;
            this.costumeImage = costumeImage;
            this.costumeGrade = costumeGrade;
            this.message = message;
        }
    }

    [System.Serializable]
    public class MyCostume
    {
        public int myCostumeSeq;
        public int costumeSeq;
        public string costumeName;
        public string costumeImage;
        public string costumeGrade;

        public MyCostume(int costumeSeq, string costumeName, string costumeImage, string costumeGrade, int myCostumeSeq)
        {
            this.costumeSeq = costumeSeq;
            this.costumeName = costumeName;
            this.costumeImage = costumeImage;
            this.costumeGrade = costumeGrade;
            this.myCostumeSeq = myCostumeSeq;
        }
    }

    // 내 정보
    public UserData userData;
    // 내 보유 코스튬 리스트
    public List<MyCostume> myCostumes;
    // 로그인 경험 여부
    public int isLogOut = 0;
    // 강퇴 유무
    public int isKicked = 0;
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

    }

    // api 요청 호출--------------------------------------------------------------------

    // 로그인
    public void Login(string userEmail, string password)
    {
        AuthSystem.Instance.OnclickLogin(userEmail, password);
    }

    // 회원가입
    public void SignUp(string signUpUserEmail, string signUpPassword, string signUpNickName)
    {
        // 닉네임 입력값 검증
        if (signUpNickName.Length < 2)
        {
            ResultInfo.Instance.InputWrong("닉네임은 2자리 이상 입력하세요.");
            return;
        }

        // 이메일 입력값 검증
        string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"; // 이메일 주소를 검증할 정규표현식

        if (!Regex.IsMatch(signUpUserEmail, pattern)) // 정규표현식을 사용하여 이메일 주소를 검증
        {
            ResultInfo.Instance.InputWrong("올바른 이메일 형식이 아닙니다.");
            return;
        }

        // 비밀번호 입력값 검증
        if (signUpPassword.Length < 6)
        {
            ResultInfo.Instance.InputWrong("비밀번호는 6자리 이상 입력하세요.");
            return;
        }

        AuthSystem.Instance.OnclickSignUp(signUpUserEmail, signUpPassword, signUpNickName);
    }

    // 로그인 유저 정보 요청
    public void ReqMyInfo(string type)
    {
        AuthSystem.Instance.CallMyInfo(accessToken, type);
    }

    // 내 코스튬 정보 요청
    public void ReqMyCostume()
    {
        AuthSystem.Instance.CallMyCostumeList(accessToken);
    }

    // 유저 정보 수정 : 코스튬
    public void ReqUpdateUserDataCostume(string cosName, string cosGrade, int cosSeq)
    {
        userData.costumeName = cosName;
        userData.costumeGrade = cosGrade;
        userData.costumeSeq = cosSeq;
        userData.costumeImage = cosName;

        // 출력창 업데이트
        CostumeInfo.Instance.UpdateCosInfo(userData.costumeName, userData.costumeGrade);

        // ui가 아닌 실제 캐릭터 코스튬 변경
        NowCoustumeSet.Instance.ChangeMyCharacterCostume(userData.costumeName);

        // 아바타 이미지 변경
        SetAvatarImg.Instance.SetImg();

        // 병경된 코스튬 정보 서버에 저장.
        AuthSystem.Instance.CallUpdateCostumeInfo(accessToken, cosSeq);
    }

    // 유저 재화정보 업데이트 요청
    public void ReqUpdateUserGoods()
    {
        AuthSystem.Instance.CallUpdateUserGoods(accessToken);
    }

    // 유저 상태 메세지 업데이트 요청
    public void ReqUpdateUserMessage(string message)
    {
        AuthSystem.Instance.CallUpdateUserMessage(accessToken, message);
    }

    // 유제 닉네임 수정 요청
    public void ReqUpdateUserNickname(string nickname, string password)
    {
        AuthSystem.Instance.CallUpdateUserNickname(accessToken, nickname, password);
    }

    // 비밀번호 검증 요청
    public void ReqCheckUserPassword(string password)
    {
        AuthSystem.Instance.CallCheckUserPassword(accessToken, password);
    }

    // api 콜백---------------------------------------------------------------
    // 회원가입 응답처리
    public void ResSignUp(bool isSuccess, string errorMessage)
    {
        // 성공시 처리
        if (isSuccess) // 로그인 성공
        {
            ResultInfo.Instance.LaunchNotification(isSuccess, "가입 성공! 환영합니다.");
            OnClickChangeForm.Instance.ShowLoginUi();
        }
        // 실패시 처리
        else // 로그인 실패
        {
            ResultInfo.Instance.LaunchNotification(isSuccess, errorMessage);
        }
    }

    // 로그인 응답처리
    public void ResLogin(bool isSuccess, string myAcToken, string errorMessage)
    {
        if(isSuccess) // 로그인 성공
        {
            ResultInfo.Instance.LaunchNotification(isSuccess, "로그인 성공! 환영합니다.");
            accessToken = myAcToken;
            StartBtnClick.Instance.AfterLogin();
        }
        else // 로그인 실패
        {
            ResultInfo.Instance.LaunchNotification(isSuccess, "비밀번호 오류 또는 "+errorMessage);
        }
    }

    // 유저데이터 응답처리
    public void ResMyInfo(UserData myData, string type)
    {
        userData = myData;
        if (type.Equals("Home")) // Home 씬에서 요청한 경우
        {
            ComeInPage.Instance.ShowUserData();
            NowCoustumeSet.Instance.SetFirstCostume();
        }
        else if (type.Equals("Lobby")) // Lobby 씬에서 요청한 경우
        {
            ComeInLobby.Instance.ShowUserData();
        }
        else if (type.Equals("WaitingRoom")) // Lobby 씬에서 요청한 경우
        {
            ComInWaitingRoom.Instance.ShowUserData();
        }
    }

    // 유저코스튬 응답처리
    public void ResMyCostumes(List<MyCostume> myCostumeList)
    {
        myCostumes = myCostumeList;
    }

    // userData 코스튬 정보 업데이트 응답처리
    public void ResUpdateUserDataCostume(bool isChange, string message)
    {
        ResultInfo.Instance.LaunchNotification(isChange, message);
    }

    // 유저 재화 업데이트 응답처리
    public void ResUpdateUserGoods(int crystal, int coin)
    {
        // 변경사항 저장.
        userData.crystal = crystal;
        userData.coin = coin;

        // 변경 후 작업 처리
        ComeInPage.Instance.UpdateUserGoods(); // 메인 홈

        // 상점부분과 다시뽑기 버튼 부분은 안열려 있을 수 있으니 try catch
        try
        {
            ShowGoodsInShop.Instance.UpdateGoodsInShop(); // 상점
        }
        catch (Exception ex)
        {
            Debug.Log("상점 안열려있음");
        }

        try
        {
            ShowResultCos.Instance.SetOnBtn(); // 버튼 on
        }
        catch (Exception ex)
        {
            Debug.Log("다시뽑기 안열려있음");

        }
    }

    // 유저 메세지 업데이트 응답처리
    public void ResUpdateUserMessage(bool isSuccess, string message)
    {
        if (isSuccess)
        {
            // 변경사항 저장.
            userData.message = message;
            // 성공 안내 팝업
            ResultInfo.Instance.LaunchNotification(isSuccess, "업데이트 완료.");
            // 변경 메세지 출력
            Profile.Instance.UpdateMessage(message);
        }
        else
        {
            // 실패 안내 팝업
            ResultInfo.Instance.LaunchNotification(isSuccess, "업데이트 실패.");

            // 메세지 인풋값 초기화
            Profile.Instance.UpdateMessage("상태 메세지 입력..");
        }

    }

    // 유저 닉네임 업데이트 응답처리
    public void ResUpdateUserNickname(bool isSuccess, string nickname)
    {
        if (isSuccess)
        {
            // 변경사항 저장.
            userData.nickname = nickname;
            // 변경 메세지 출력
            Profile.Instance.UpdateNickname(nickname);
            //포톤 닉네임 변경
            PhotonNetwork.NickName = nickname;
            // ui 변경
            ComeInPage.Instance.UpdateNicknameUi(nickname);
            // 성공 안내 팝업
            ResultInfo.Instance.LaunchNotification(isSuccess, "변경 완료!");
        }
        else
        {
            // 실패 안내 팝업
            ResultInfo.Instance.LaunchNotification(isSuccess, "이미 사용중인 닉네임 입니다.");

            // 메세지 인풋값 초기화
            Profile.Instance.UpdateNickname(userData.nickname);
        }

    }
    // 비밀번호 검증 응답처리
    public void ResCheckUserPassword(bool isSuccess, string password)
    {
        if (isSuccess)
        {
            // 입력된 닉네임 가져오기
            string nickname = Profile.Instance.GetNickname();
            // 닉네임 변경 요청 호출
            ReqUpdateUserNickname(nickname, password);
        }
        else
        {
            // 실패 안내 팝업
            ResultInfo.Instance.LaunchNotification(isSuccess, "비밀번호가 틀렸습니다.");

        }
        // 비밀번호 인풋값 초기화
        Profile.Instance.AfterCheck(isSuccess);

    }
    // 로그아웃
    public void LogOut()
    {
        isLogOut = 1;
        accessToken = "";
        userData = new UserData();
        myCostumes.Clear();
    }
}
