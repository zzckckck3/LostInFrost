using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static ApiResponse;
using static ApiRequest;
using System.Runtime.CompilerServices;

public class AuthSystem : MonoBehaviour
{
    private static AuthSystem instance = null;
    public static AuthSystem Instance
    {
        get { return instance; }
    }

    // api 주소
    private string apiURL = "https://k9c101.p.ssafy.io";// API 엔드포인트 URL

    // 내 정보
    public AuthManager.UserData userData;
    // 내 코스튬 리스트
    public List<AuthManager.MyCostume> myCostumes;

    

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // 비동기 통신 호출-----------------------------------------------------------------------

    // 버튼 클릭시 로그인 api 호출
    public void OnclickLogin(string userEmail, string password)
    {
        StartCoroutine(Login(apiURL, userEmail, password));
    }

    // 버튼 클릭시 회원가입 api 호출
    public void OnclickSignUp(string userEmail, string password, string nickName)
    {
        StartCoroutine(SignUp(apiURL, userEmail, password, nickName));
    }

    // 내정보 요청 api 호출
    public void CallMyInfo(string myAccessToken, string type)
    {
        StartCoroutine(MyInfo(apiURL, myAccessToken, type));
    }

    // 내 코스튬 리스트 요청 api 호출
    public void CallMyCostumeList(string myAccessToken)
    {
        StartCoroutine(MyCostumes(apiURL, myAccessToken));
    }

    // 변경된 장착 코스튬 정보 업데이트 api 호출
    public void CallUpdateCostumeInfo(string myAccessToken, int myCostumeSequence)
    {
        StartCoroutine(UpdateNowCostume(apiURL, myAccessToken, myCostumeSequence));
    }

    // 유저 재화 조회(업데이트) api 호출
    public void CallUpdateUserGoods(string myAccessToken)
    {
        StartCoroutine(UpdateUserGoods(apiURL, myAccessToken));
    }

    // 유저 상태 메세지 업데이트 api 호출
    public void CallUpdateUserMessage(string myAccessToken, string message)
    {
        StartCoroutine(UpdateUserMessage(apiURL, myAccessToken, message));
    }

    // 유저 상태 닉네임 업데이트 api 호출
    public void CallUpdateUserNickname(string myAccessToken, string nickname, string password)
    {
        StartCoroutine(UpdateUserNickname(apiURL, myAccessToken, nickname, password));
    }

    // 비밀번호 검증 호출
    public void CallCheckUserPassword(string myAccessToken, string password)
    {
        StartCoroutine(CheckUserPassword(apiURL, myAccessToken, password));
    }

    // 비동기 통신 : 코루틴--------------------------------------------------------------------
    // 로그인 코루틴
    IEnumerator Login(string url, string userEmail, string password)
    {
        LoginData loginData = new LoginData
        {
            email = userEmail,
            password = password
        };
        string loginInfo = JsonUtility.ToJson(loginData);

        string apiUrl = url + "/api/auth/login";

        // HTTP 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(loginInfo));
        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(apiResponse);

            if (responseData.success)
            {
                // AuthManager에 내 토큰 저장.
                AuthManager.Instance.ResLogin(true, responseData.response, null);
            }
            else
            {
                // 에러 메시지와 상태를 가져올 수 있음
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 로그인 실패
                AuthManager.Instance.ResLogin(false, " ", errorMessage);
                Debug.LogError("로그인 실패. 에러 메시지: " + errorMessage);
            }   
        }
    }

    // 회원가입 코루틴
    IEnumerator SignUp(string url, string userEmail, string password, string nickName)
    {
        SignUpData signUpData = new SignUpData
        {
            email = userEmail,
            password = password,
            nickname = nickName
        };

        string loginInfo = JsonUtility.ToJson(signUpData);

        string apiUrl = url + "/api/auth/join";

        // HTTP 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(loginInfo));
        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(apiResponse);

            if (responseData.success)
            {
                // 가입 성공
                Debug.Log("회원가입 성공.");
                AuthManager.Instance.ResSignUp(true, null);
            }
            else
            {
                // 에러 메시지와 상태를 가져올 수 있음
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                AuthManager.Instance.ResSignUp(false, errorMessage);
                // 회원가입 실패
                Debug.LogError("회원가입 실패. 에러 메시지: " + errorMessage);
            }
        }
    }

    // 내 정보 가져오기 코루틴
    IEnumerator MyInfo(string url, string accessToken, string type)
    {
        string apiUrl = url + "/api/user/info";
        // HTTP 요청 설정
        UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl);
        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseUserData responseData = JsonUtility.FromJson<ResponseUserData>(apiResponse);
            

            if (responseData.success)
            {
                // 유저 데이터 받아옴.
                userData = responseData.response;
                // AuthManager로 데이터 넘김.
                AuthManager.Instance.ResMyInfo(userData, type);
                StartCoroutine(MyCostumes(apiURL, AuthManager.Instance.accessToken));
            }
            else
            {
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.LogError("정보 요청실패. 에러 메시지: ");
            }
        }
    }

    // 내 보유 코스튬 리스트 가져오기 코루틴
    IEnumerator MyCostumes(string url, string accessToken)
    {
        string apiUrl = url + "/api/user/my-costume/all-list";

        // HTTP 요청 설정
        UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl);

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseMyCostumes responseData = JsonUtility.FromJson<ResponseMyCostumes>(apiResponse);


            if (responseData.success)
            {
                // 유저 데이터 받아옴.
                myCostumes = responseData.response;
                // AuthManager로 데이터 넘김.
                AuthManager.Instance.ResMyCostumes(myCostumes);
            }
            else
            {
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.LogError("정보 요청실패. 에러 메시지: ");
            }
        }
    }

    // 코스튬 장착 정보 업데이트.
    IEnumerator UpdateNowCostume(string url, string accessToken, int myCostumeSequence)
    {
        string apiUrl = url + "/api/user/my-costume/" + myCostumeSequence;

        // PUT 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "PUT");

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(apiResponse);


            if (responseData.success)
            {
                // 응답처리
                AuthManager.Instance.ResUpdateUserDataCostume(true, "변경 완료");
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                AuthManager.Instance.ResUpdateUserDataCostume(false, "변경 실패 : "+ errorMessage);
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.LogError("정보 요청실패. 에러 메시지: " + errorMessage);
            }
        }
    }

    // 유저 재화 정보 업데이트.
    IEnumerator UpdateUserGoods(string url, string accessToken)
    {
        string apiUrl = url + "/api/user/amount";

        // get 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "GET");

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            UpdateGoodsResponseData responseData = JsonUtility.FromJson<UpdateGoodsResponseData>(apiResponse);


            if (responseData.success)
            {
                Goods goods = responseData.response;
                // 응답처리
                AuthManager.Instance.ResUpdateUserGoods(goods.crystal, goods.coin);
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.LogError("정보 요청실패. 에러 메시지: " + errorMessage);
            }
        }
    }

    // 유저 상태 메세지 업데이트.
    IEnumerator UpdateUserMessage(string url, string accessToken, string message)
    {
        string apiUrl = url + "/api/user/message";
        // PUT 요청 설정
        UnityWebRequest webRequest = UnityWebRequest.Put(apiUrl, message);

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(apiResponse);


            if (responseData.success)
            {
                // 응답처리
                Debug.Log("메세지 저장 성공");
                AuthManager.Instance.ResUpdateUserMessage(true, message);
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.Log("메세지 저장 실패");
                AuthManager.Instance.ResUpdateUserMessage(false, message);

                Debug.LogError("정보 요청실패. 에러 메시지: " + errorMessage);
            }
        }
    }

    // 유저 닉네임 업데이트.
    IEnumerator UpdateUserNickname(string url, string accessToken, string nickname, string password)
    {
        UpdateData updateData = new UpdateData
        {
            nickname = nickname,
            password = password
        };
        string updateInfo = JsonUtility.ToJson(updateData);

        string apiUrl = url + "/api/user/info";

        // Post 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "PUT");

        // json 데이터 담기 : 유저정보.
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(updateInfo));

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(apiResponse);


            if (responseData.success)
            {
                // 응답처리
                Debug.Log("닉네임 변경 성공");
                AuthManager.Instance.ResUpdateUserNickname(true, nickname);
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.Log("닉네임 변경 실패");
                AuthManager.Instance.ResUpdateUserNickname(false, nickname);
                Debug.LogError("정보 요청실패. 에러 메시지: " + errorMessage);
            }
        }
    }

    // 비밀번호 검증
    IEnumerator CheckUserPassword(string url, string accessToken, string password)
    {
        string apiUrl = url + "/api/user/validate-password";
        // Post 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");

        //  데이터 담기 : 비밀번호.
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(password));

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);

        //UnityWebRequest에 버퍼설정
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        // Content-Type 헤더를 application/json으로 설정
        webRequest.SetRequestHeader("Content-Type", "application/json");

        //URL로 접속해서 결과를 가져올때까지 대기
        yield return webRequest.SendWebRequest();

        //에러발생 체크
        if (webRequest.isNetworkError)
        {
            //호출실패
            Debug.Log(webRequest.error);
        }
        else
        {
            // API 응답을 문자열로 받음
            string apiResponse = webRequest.downloadHandler.text;

            // JSON 파싱을 위해 직렬화 클래스 사용
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(apiResponse);


            if (responseData.success)
            {
                // 응답처리
                Debug.Log("비밀번호 인증 성공");
                AuthManager.Instance.ResCheckUserPassword(true, password);
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 에러 메시지와 상태를 가져올 수 있음
                Debug.Log("비밀번호 인증 실패");
                AuthManager.Instance.ResCheckUserPassword(false, password);
                Debug.LogError("정보 요청실패. 에러 메시지: " + errorMessage);
            }
        }
    }
}
