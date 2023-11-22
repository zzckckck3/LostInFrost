using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ApiResponse;
using UnityEngine.Networking;

public class GameApiSystem : MonoBehaviour
{
    private static GameApiSystem instance = null;
    public static GameApiSystem Instance
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

    // api 주소
    private string apiURL = "https://k9c101.p.ssafy.io";// API 엔드포인트 URL


    // 비동기 통신 호출-----------------------------------------------------------------------
    // 게임 시작했을때 데이터 저장
    public void CallSetGameStartData(GameApiManager.GameResult gameResult)
    {
        StartCoroutine(SetGameStartData(apiURL, gameResult));
    }
    // 게임 끝났을때 결과 업데이트
    public void CallSetGameResult (GameApiManager.GameResult gameResult, int type)
    {
        StartCoroutine(SetGameResult(apiURL, gameResult, type));
    }


    // 비동기 통신 : 코루틴--------------------------------------------------------------------

    // 게임 결과 업데이트
    IEnumerator SetGameResult(string url, GameApiManager.GameResult gameResult, int type)
    {
        string apiUrl = url + "/api/game/record/finish";

        // 데이터 인코딩
        string gameResultInfo = JsonUtility.ToJson(gameResult);

        // PUT 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "PUT");

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + AuthManager.Instance.accessToken);

        // json 데이터 담기 : 게임결과.
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(gameResultInfo));

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
            ResultGameResponseData responseData = JsonUtility.FromJson<ResultGameResponseData>(apiResponse);


            if (responseData.success)
            {
                // 응답처리
                Debug.Log("끝 데이터 저장 성공: " + AuthManager.Instance.userData.nickname);
                int level = responseData.response.level;
                int experience = responseData.response.experience;
                int coin = (int) responseData.response.coin;
                GameApiManager.Instance.ResGameResult(level, experience, coin, type);
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                //에러처리

                // 에러 메시지와 상태를 가져올 수 있음
                Debug.LogError("끝정보 요청실패. 에러 메시지: " + errorMessage + "- " + AuthManager.Instance.userData.nickname);
            }
        }
    }

    // 게임 시작시 정보 저장.
    IEnumerator SetGameStartData(string url, GameApiManager.GameResult gameResult)
    {
        string apiUrl = url + "/api/game/record/start";

        // 데이터 인코딩
        string gameResultInfo = JsonUtility.ToJson(gameResult);

        // Post 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer " + AuthManager.Instance.accessToken);

        // json 데이터 담기 : 게임결과.
        webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(gameResultInfo));

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
            ResultGameResponseData responseData = JsonUtility.FromJson<ResultGameResponseData>(apiResponse);


            if (responseData.success)
            {
                // 응답처리
                Debug.Log("시작 데이터 저장 성공: "+ AuthManager.Instance.userData.nickname);
                GameApiManager.Instance.ResGameStart();
            }
            else
            {
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 에러 메시지와 상태를 가져올 수 있음
                GameApiManager.Instance.ResGameStart();
                Debug.LogError("시작정보 요청실패. 에러 메시지: " + errorMessage +"- " + AuthManager.Instance.userData.nickname);
            }
        }
    }
}
