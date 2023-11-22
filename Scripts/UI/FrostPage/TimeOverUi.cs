using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;

public class TimeOverUi : MonoBehaviour
{
    private static TimeOverUi instance = null;
    public static TimeOverUi Instance
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

    private float maxGauge; // 최대 게이지
    private SlicedFilledImage slicedImage;
    public TextMeshProUGUI guageValue; // 게이지 수치
    public TextMeshProUGUI scoreValue; // 스코어 수치
    public TextMeshProUGUI clearTimeValue; // 클리어 타임 수치
    private float nowGauge = 0f;
    private int score = 0;
    private float fillSpeed = 20f; // 게이지 차는 속도
    public GameObject exitBtn; // 방 나가는 버튼
    public GameObject savingInfo; // 데이터 저장중 안내멘트

    // Start is called before the first frame update
    void Start()
    {
        float clearTime = InGameManager.Instance.PlayTime;
        slicedImage = GetComponent<SlicedFilledImage>();
        maxGauge = (clearTime / InGameManager.Instance.endTime) * 100.0f;
        maxGauge = Mathf.Clamp(maxGauge, 0f, 100.0f);

        // 스코어 정산
        score = (int)InGameManager.Instance.endTime * 2;

        // 스코어 ui출력
        scoreValue.text = score + "";

        // 클리어타임
        int minutes = Mathf.FloorToInt(clearTime / 60);
        int seconds = Mathf.FloorToInt(clearTime % 60);
        clearTimeValue.text = "생존시간: " + minutes.ToString("00") + ":" + seconds.ToString("00");

        // 데이터 저장 api 호출
        string startTime = PhotonNetwork.CurrentRoom.CustomProperties["startTime"] + "";
        string roomSeq = PhotonNetwork.CurrentRoom.CustomProperties["uuid"] + "";
        string gameLevelString = PhotonNetwork.CurrentRoom.CustomProperties["gameLevel"].ToString();
        float gameLevel;
        if (float.TryParse(gameLevelString, out gameLevel)) { }// 변환 성공
        else
        {
            // 변환 실패
            gameLevel = 0.8f;
        }
        GameApiManager.Instance.ReqGameFinsh(clearTime, false, roomSeq, startTime, 1, gameLevel);
    }

    void Update()
    {

        // 이미지 채우기
        if (nowGauge <= maxGauge && slicedImage != null)
        {
            // 게이지를 시간에 따라 채우기
            nowGauge += fillSpeed * Time.deltaTime;
            nowGauge = Mathf.Clamp(nowGauge, 0f, maxGauge);
            slicedImage.fillAmount = nowGauge * 0.01f;
            guageValue.text = (int)nowGauge + "%";
        }
    }
    // 저장 후 호출
    public void AfterSaving()
    {
        exitBtn.SetActive(true);
        savingInfo.SetActive(false);
        Invoke("ClearExit", 15.0f);

    }
    public void ClearExit()
    {
        string myNickname = AuthManager.Instance.userData.nickname;
        if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.NickName.Equals(myNickname))
        {
            Debug.Log("룸 리스트로 이동 시도");
            PhotonNetwork.LeaveRoom();
            // 게임방 리스트로
            PhotonNetwork.LoadLevel("RoomList");
        }
    }
}
