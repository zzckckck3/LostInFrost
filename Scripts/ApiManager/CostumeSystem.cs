using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ApiResponse;
using UnityEngine.Networking;
using static AuthManager;
using System;

public class CostumeSystem : MonoBehaviour
{
    private static CostumeSystem instance = null;
    public static CostumeSystem Instance
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
    // 코스튬 리스트
    public List<CostumeManager.Costume> costumeList = new List<CostumeManager.Costume>();


    // 비동기 통신 호출-----------------------------------------------------------------------

    // 코스튬 리스트 요청 호출 
    public void CallReqCostumList()
    {
        StartCoroutine(CostumeList(apiURL));
    }

    // 코스튬 뽑기 요청 호출
    public void CallReqPurchaseCostume(string accessToken, string type)
    {
        StartCoroutine(PurchaseCostume(apiURL, accessToken, type));
    }


    // 비동기 통신 : 코루틴--------------------------------------------------------------------

    // 코스튬 리스트 요청
    IEnumerator CostumeList(string url)
    {
        string apiUrl = url + "/api/game/costume/all-list";
        // HTTP 요청 설정
        UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl);
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
            ResponseDataCostume responseData = JsonUtility.FromJson<ResponseDataCostume>(apiResponse);

            if (responseData.success)
            {
                // 응답 데이터 처리 : 코스튬 리스트
                costumeList = responseData.response;

                CostumeManager.Instance.ResCostumeList(costumeList);
            }
            else
            {
                // 에러 메시지와 상태를 가져올 수 있음
                string errorMessage = responseData.error.message;
                int errorStatus = responseData.error.status;
                // 로그인 실패
                Debug.LogError("코스튬 불러오기 실패. 에러 메시지: " + errorMessage);
            }
        }
    }

    // 코스튬 뽑기 요청
    IEnumerator PurchaseCostume(string url, string accessToken, string type)
    {
        string apiUrl = url + "/api/game/draw/"+ type;
        // PUT 요청 설정
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
            ResponseCostumePurchase responseData = JsonUtility.FromJson<ResponseCostumePurchase>(apiResponse);

            if (responseData.success)
            {
                // 응답 데이터 처리 : 가챠 결과
                // 구매가 가능했고, 뽑은 코스튬이 중복이 아니라면 성공 
                CostumeManager.Instance.ResPurchaseCostume(responseData.response, 1, type);
                
            }
            else
            {
                // 에러 메시지와 상태를 가져올 수 있음
                string errorMessage = responseData.error.message;

                // 문자열에서 필요한 데이터 추출
                int myCostumeSeq = int.Parse(ExtractValueFromMessage(errorMessage, "myCostumeSeq"));
                int costumeSeq = int.Parse(ExtractValueFromMessage(errorMessage, "costumeSeq"));
                string costumeName = ExtractValueFromMessage(errorMessage, "costumeName");
                string costumeImage = ExtractValueFromMessage(errorMessage, "costumeImage");
                string costumeGrade = ExtractValueFromMessage(errorMessage, "costumeGrade");

                // MyCostume 객체 생성
                MyCostume myCostume = new MyCostume(costumeSeq, costumeName, costumeImage, costumeGrade, myCostumeSeq);

                // 중복 코스튬 출력
                CostumeManager.Instance.ResPurchaseCostume(myCostume, 0, type);

                int errorStatus = responseData.error.status;
            }
        }
    }

    // 문자열에서 필요한 데이터를 추출하는 함수
    string ExtractValueFromMessage(string message, string key)
    {
        string keyPattern = key + "=";
        int keyStartIndex = message.IndexOf(keyPattern);
        if (keyStartIndex < 0)
        {
            throw new ArgumentException("Key not found in message: " + key);
        }

        keyStartIndex += keyPattern.Length;

        int keyEndIndex = message.IndexOf(",", keyStartIndex);
        if (keyEndIndex < 0)
        {
            keyEndIndex = message.Length - 1;
        }

        string valueString = message.Substring(keyStartIndex, keyEndIndex - keyStartIndex);

        return valueString;
    }
}
