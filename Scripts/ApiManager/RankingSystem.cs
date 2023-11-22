using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ApiResponse;
using UnityEngine.Networking;

public class RankingSystem : MonoBehaviour
{
    private static RankingSystem instance = null;
    public static RankingSystem Instance
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

    // 랭킹 리스트 호출
    public void CallRankingList()
    {
        StartCoroutine(RankingList(apiURL));
    }


    // 비동기 통신 : 코루틴--------------------------------------------------------------------

    // 랭킹 리스트 업데이트.
    IEnumerator RankingList(string url)
    {
        string apiUrl = url + "/api/rank/list?page=0&size=30";

        // PUT 요청 설정
        UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "GET");

        // 토큰정보 헤더에 담기.
        webRequest.SetRequestHeader("Authorization", "Bearer ");

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
            ResultRankingList responseData = JsonUtility.FromJson<ResultRankingList>(apiResponse);
            
            if (responseData.success)
            {
                Debug.Log(responseData.response.content.Count);
                // 응답처리
                // 랭킹 리스트 저장
                RankingManager.Instance.rankList = responseData.response.content;
                // 이후 작업 요청 호출.
                RankingManager.Instance.ResRankingList();
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
}
