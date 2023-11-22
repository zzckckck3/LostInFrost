using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public Light directionalLight;
    public float rotationSpeed = 10.0f;
    public float dayNightCycleDuration = 180.0f; // 낮과 밤 주기의 기간 (초)

    private float timeOfDay = 0.0f; // 현재 시간 (초)

    private bool isNight = false; // 밤 판별 변수
    private bool wasNight = false; // 이전 밤 상태
    public bool IsNight
    {
        get { return isNight; }
        set { isNight = value; }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        RenderSettings.fog = true;
    }

    void Update()
    {
        timeOfDay += Time.deltaTime;
        
        if(timeOfDay > dayNightCycleDuration)
        {
            timeOfDay = 0.0f;
        }

        float angle = (timeOfDay / dayNightCycleDuration) * 360.0f;
        directionalLight.transform.rotation = Quaternion.Euler(angle, -30, 0);

        // 해가 지평선 아래로 떨어지면 밤이 된다.
        if (angle > 180.0f && angle < 360.0f)
        {
            isNight = true;
        }
        else
        {
            isNight = false;
        }

        if (isNight != wasNight)
        {
            if (!isNight) // 낮이 되었을 때
            {
                InGameManager.Instance.AvailableCards += 1;
                UserStatusManager.Instance.difficulty += 0.05f;
            }

            wasNight = isNight;
        }
    }

}
