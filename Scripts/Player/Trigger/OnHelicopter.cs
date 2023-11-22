using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class OnHelicopter : MonoBehaviour
{
    public string playerTag = "Helicopter"; // 헬기 태그
    public PhotonView PV; // 포톤뷰
    private float fillSpeed = 20f; // 게이지 차는 속도
    public float maxGauge = 100f; // 최대 게이지
    private float gauge = 0; // 현재 게이지
    public bool isFilling = false; // 채우기 유무
    AudioSource gaugeUpSound; // 게이지 사운드
    public bool isPaused = false; // 게이지 사운드 퍼즈 유무.
    public bool isStart = false; // 게이지 사운드 퍼즈 유무.
    public bool gameFinish = false; // 게임 종료 여부.
    private void Start()
    {
        gaugeUpSound = GetComponentsInChildren<AudioSource>()[0];
    }
    void Update()
    {
        if (isFilling && gauge < 100f)
        {
            DoFillGauge();
            //Debug.Log("현재 게이지: " + gauge);
        }
        // 게이지 완충시
        if(gauge >= 100f && !gameFinish)
        {
            gameFinish = true;
            Debug.Log("탈출 성공!");
            GameFinishCall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (PV.IsMine && PhotonNetwork.InRoom && UserStatusManager.Instance.HP > 0)
            {
                if (!isStart)
                {
                    gaugeUpSound.Play();
                    isStart = true;
                }
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        // 플레이어와의 접촉이 지속되는 동안 계속 게이지를 채우기
        if (other.CompareTag(playerTag))
        {
            if (PV.IsMine && PhotonNetwork.InRoom && UserStatusManager.Instance.HP > 0)
            {
                isFilling = true;
                // 이후 필요한 작업들
                if (isPaused)
                {
                    Debug.Log("퍼즈풀기");
                    isPaused = false;
                    gaugeUpSound.UnPause();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 플레이어와의 접촉이 끝나면 게이지 채우기 중단
        if (other.CompareTag(playerTag))
        {
            if (PV.IsMine && PhotonNetwork.InRoom && UserStatusManager.Instance.HP > 0)
            {
                isFilling = false;
                // 이후 필요한 작업들
                if (!isPaused)
                {
                    Debug.Log("퍼즈");
                    isPaused =  true;
                    gaugeUpSound.Pause();
                }
            }
        }
    }

    void DoFillGauge()
    {
        // 게이지를 시간에 따라 채우기
        gauge += fillSpeed * Time.deltaTime;

        // 게이지를 0에서 maxGauge 사이로 제한
        gauge = Mathf.Clamp(gauge, 0f, maxGauge);

        // 이미지 게이지 업데이트 호출\
        FillGauge.instance.OnFillGuage(gauge);

    }

    public void GameFinishCall()
    {
        GameFinshUi.instance.ClearFinish();
    }
}
