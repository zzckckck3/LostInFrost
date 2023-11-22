using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class GameApiManager : MonoBehaviour
{
    private static GameApiManager instance = null;
    public static GameApiManager Instance
    {
        get { return instance; }
    }
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
    [System.Serializable]
    public class GameResult
    {
        public int finishTime;
        public bool isClear;
        public string roomId;
        public string startAt;
        public float difficulty;

        public GameResult(int finishTime, bool isClear, string roomId, string startAt, float difficulty)
        {
            this.finishTime = finishTime;
            this.isClear = isClear;
            this.roomId = roomId;
            this.startAt = startAt;
            this.difficulty = difficulty;
        }
    }
    // api 요청 호출--------------------------------------------------------------------
    public void ReqGameStart(float finishTime, bool isClear, string roomId, string startAt, float gameLevel)
    {
        /*
         * type = 0 -> 죽음
         * type = 1 -> 시간초과
         * type = 2 -> 클리어
         */
        GameResult gameResult = new GameResult((int)finishTime, isClear, roomId, startAt, gameLevel);
        GameApiSystem.Instance.CallSetGameStartData(gameResult);

    }
    public void ReqGameFinsh(float finishTime, bool isClear, string roomId, string startAt, int type, float gameLevel)
    {
        /*
         * type = 0 -> 죽음
         * type = 1 -> 시간초과
         * type = 2 -> 클리어
         */
        GameResult gameResult = new GameResult((int)finishTime, isClear, roomId, startAt, gameLevel);
        GameApiSystem.Instance.CallSetGameResult(gameResult, type);

    }

    // api 콜백---------------------------------------------------------------
    // 게임 결과 저장 응답처리
    public void ResGameResult(int level, int experience, int coin, int type)
    {
        // 결과 저장.
        AuthManager.Instance.userData.level = level;
        AuthManager.Instance.userData.experience = experience;
        AuthManager.Instance.userData.coin = coin;
        
        // 죽었을때 결과처리
        if(type == 0)
        {
            FailDeadUi.Instance.AfterSaving(); // 저장완료 후 나가기 버튼 활성화.
        }
        // 타임오버 결과처리
        else if (type == 1)
        {
            TimeOverUi.Instance.AfterSaving(); // 저장완료 후 나가기 버튼 활성화.
        }
        // 클리어 결과처리
        else if(type == 2)
        {
            ClearInfoUi.Instance.AfterSaving(); // 저장완료 후 나가기 버튼 활성화.
        }
    }

    // 게임 시작 데이터 저장 응답처리
    public void ResGameStart()
    {
        PlaneBroke.instance.changeIntoInGameScene();
    }
}
