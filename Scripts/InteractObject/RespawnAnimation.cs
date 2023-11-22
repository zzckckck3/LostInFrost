using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAnimation : MonoBehaviour
{
    public bool isRespawn = false;
    public float changeSpeed;// 스케일 변경 속도 > 초기는 0으로 해서 점차 증가하게 
    private float firstChangeSpeed; // 증가한 changeSpeed를 초기화해주기 위한 변수
    private Vector3 firstPosition;
    private int firstHp;
    void Start()
    {
        // 초기 변경속도 저장 
        firstChangeSpeed = changeSpeed;
        firstPosition = transform.position;
        firstHp = this.GetComponent<InteractStatus>().interactHp;
    }

    void Update()
    {
        // 리스폰 판별
        if (isRespawn)
        {
            // Scale값 확인 후 1.0보다 작다면
            if (transform.localScale.x < 1.0f && transform.localScale.y < 1.0f && transform.localScale.z < 1.0f)
            {
                // 변경값만큼 변경
                transform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed) * Time.deltaTime;
                // 서서히 느리게 커지도록 조정
                changeSpeed -= 0.01f;
                // changeSpeed가 0아래로 내려가면 gameobject가 없어지기에 예외처리
                if(changeSpeed < 0.0f)
                {
                    isRespawn = false;
                }
            }
            else
            {
                isRespawn = false;
            }
        }
    }

    // Respawn을 시작하고, 상태를 초기화
    public void StartRespawnAnimation()
    {
        // 나무의 체럭도 초기값으로 돌리는건 쓰러지자마자 해야함
        this.GetComponent<InteractStatus>().interactHp = firstHp;
        // 리스폰 상태를 활성화
        isRespawn = true;
        // 스케일을 0부터 시작하여 자라나도록 보이게
        transform.localScale = new Vector3(0, 0, 0);
        // 회전을 초기화시켜 똑바로 서게 한다
        transform.rotation = Quaternion.identity;
        // 높이도 초기값으로 맞춰줌
        transform.position = firstPosition;
        // changeSpeed를 초기값으로 돌림
        changeSpeed = firstChangeSpeed;
        // 나무의 체럭도 초기값으로 돌리는건 쓰러지자마자 해야함
        //this.GetComponent<Wood>().woodPhysicalStrength = firstWoodPhysicalStrength;
    }
}
