using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingShow : MonoBehaviour
{
    private static RankingShow instance = null;
    public static RankingShow Instance
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

    // 랭킹정보 ui item
    public GameObject rankInfo;
    // 랭킹 정보 리스트 담을 ui 오브젝트
    public GameObject rankInfos;
    public void ShowRankingList()
    {
        foreach (RankingManager.RankData rankData in RankingManager.Instance.rankList)
        {
            // 프리팹을 인스턴스화
            GameObject instance = Instantiate(rankInfo);

            // 원하는 부모 오브젝트를 찾거나 직접 지정
            Transform parent = rankInfos.transform;

            // 부모 아래에 추가
            instance.transform.SetParent(parent);
            instance.transform.localScale = new Vector3(0.92f, 0.92f, 0.92f);
            // 추가된 랭킹 정보 form에 데이터 출력
            TextMeshProUGUI[] textObjects = instance.GetComponentsInChildren<TextMeshProUGUI>(true);

            textObjects[0].text = rankData.rankSeq+""; // 순위
            textObjects[1].text = rankData.memberNickname; // 닉네임
            textObjects[2].text = rankData.memberLevel + ""; // 레벨
            textObjects[3].text = rankData.memberExperience + ""; // 경험치
            textObjects[4].text = rankData.memberGamePlayCount.ToString("#,0"); ; // 플레이 횟수
            textObjects[5].text = rankData.memberMessage.Length > 0 ? rankData.memberMessage : ".  .  ."; // 상태메세지

            if (rankData.memberNickname.Equals(PhotonNetwork.LocalPlayer.NickName))
            {
                textObjects[1].color = Color.yellow;
            }

            // 순위 텍스트 처리
            if(rankData.rankSeq == 1)
            {
                textObjects[0].text = "#<size=50>1</size>";
                textObjects[0].color = new Color(1.0f, 0.8313726f, 0.2313726f, 1.0f);
            }
            else if (rankData.rankSeq == 2)
            {
                textObjects[0].text = "#<size=50>2</size>";
                textObjects[0].color = new Color(0.7882353f, 0.827451f, 0.8745098f, 1.0f);
            }
            else if (rankData.rankSeq == 3)
            {
                textObjects[0].text = "#<size=50>3</size>";
                textObjects[0].color = new Color(1.0f, 0.6f, 0.2784314f, 1.0f);
            }


            // 이미지 처리
            // 코스튬에 해당하는 이미지 불러와서 할당
            Sprite cosImage = Resources.Load<Sprite>("Images/" + rankData.memberCostume);
            Image[] imgObjects = instance.GetComponentsInChildren<Image>(true);
            // 이미지 변경
            imgObjects[2].sprite = cosImage;
        }
    }
}
