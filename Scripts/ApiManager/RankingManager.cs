using System;
using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    private static RankingManager instance = null;
    public static RankingManager Instance
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

    [System.Serializable]
    public class RankData
    {
        public int rankSeq;
        public string memberId;
        public string memberNickname;
        public string memberCostume;
        public int memberLevel;
        public int memberExperience;
        public string memberMessage;
        public int memberGamePlayCount;

        // 기본 생성자
        public RankData() { }
        public RankData(int rankSeq, string memberId, string memberNickname, string memberCostume,
                        int memberLevel, int memberExperience, string memberMessage, int memberGamePlayCount)
        {
            this.rankSeq = rankSeq;
            this.memberId = memberId;
            this.memberNickname = memberNickname;
            this.memberCostume = memberCostume;
            this.memberLevel = memberLevel;
            this.memberExperience = memberExperience;
            this.memberMessage = memberMessage;
            this.memberGamePlayCount = memberGamePlayCount;
        }
    }

    // 랭킹 리스트
    public List<RankData> rankList;

    // api 요청 호출--------------------------------------------------------------------
    // 랭킹 리스트 요청 호출
    public void ReqRankingList()
    {
        RankingSystem.Instance.CallRankingList();
    }

    // api 콜백---------------------------------------------------------------
    // 회원가입 응답처리
    public void ResRankingList()
    {
        // 이후 작업.
        RankingShow.Instance.ShowRankingList();
    }
}
