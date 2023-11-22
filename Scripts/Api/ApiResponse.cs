using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiResponse : MonoBehaviour
{
    // 에러 메세지 res
    [System.Serializable]
    public class ErrorData
    {
        public string message;
        public int status;
    }

    // 기본 res
    public class ResponseData
    {
        public bool success;
        public string response;
        public ErrorData error;
    }

    // 유저데이타 res
    public class ResponseUserData
    {
        public bool success;
        public AuthManager.UserData response;
        public ErrorData error;
    }
    
    // 마이코스튬리스트 res
    public class ResponseMyCostumes
    {
        public bool success;
        public List<AuthManager.MyCostume> response;
        public ErrorData error;
    }

    // 코스튬리스트 res
    public class ResponseDataCostume
    {
        public bool success;
        public List<CostumeManager.Costume> response;
        public ErrorData error;
    }

    // 코스튬 뽑기 res
    // 코스튬리스트 res
    public class ResponseCostumePurchase
    {
        public bool success;
        public AuthManager.MyCostume response;
        public ErrorData error;
    }

    // 중복 코스튬 res
    public class DrawResDto
    {
        public int costumeSeq { get; set; }
        public string costumeName { get; set; }
        public string costumeImage { get; set; }
        public string costumeGrade { get; set; }
        public int myCostumeSeq { get; set; }
    }

    // 재화 class
    [System.Serializable]
    public class Goods
    {
        public int crystal;
        public int coin;
    }
    // 유저 재화 업데이트 res
    public class UpdateGoodsResponseData
    {
        public bool success;
        public Goods response;
        public ErrorData error;
    }

    // 게임결과 class
    [System.Serializable]
    public class ResultGame
    {
        public int level;
        public int experience;
        public long coin;
    }
    // 게임결과 res
    public class ResultGameResponseData
    {
        public bool success;
        public ResultGame response;
        public ErrorData error;
    }

    // 랭킹리스트 res
    public class ResultRankingList
    {
        public bool success;
        public RankinResponse response;
        public ErrorData error;
    }

    [System.Serializable]
    public class RankinResponse
    {
        public List<RankingManager.RankData> content;
        public Object pageable;
        public int totalPage;
        public int totalElements;
        public bool last;
        public int size;
        public int number;
        public Object sort;
        public int numberOfElements;
        public bool first;
        public bool empty;

    }
}
