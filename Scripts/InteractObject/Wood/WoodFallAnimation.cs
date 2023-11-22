using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodFallAnimation : MonoBehaviour
{
    public bool isFalling = false;
    private float timeSinceFall = 0.0f;
    public float respawnWoodTime = 5.0f;
    private int woodFallQuadrant;

    //private int firstWoodHp;

    private RespawnAnimation respawnAnimation;

    void Start()
    {
        respawnAnimation = GetComponent<RespawnAnimation>();
        // HP 초기값 저장 
        //firstWoodHp = this.GetComponent<InteractStatus>().interactHp;
    }

    void Update()
    {
        if(isFalling)
        {
            // 캐릭터 입력 각도에 따른 나무 쓰러짐 방향 조정
            // 유니티 엔진에서 transform은 -112.5이런식으로 쓰여지지만, C#에선 247.5 이런식으로 인식한다..
            // 초기 각도가 0이기 때문에 음수로 진행될 때에는 0을 넣어줘야함. 그래서 switch case문으로 진행 불가능
            float thisZ = this.transform.rotation.eulerAngles.z;
            float thisY = this.transform.rotation.eulerAngles.y;
            if (woodFallQuadrant == 1)
            {
                if(thisZ > 247.5f || thisZ == 0.0f)
                {
                    transform.eulerAngles += new Vector3(0f, -45f, -112.5f) * Time.deltaTime;
                }
                else if (thisY > -5.0f)
                {
                    transform.position += new Vector3(0f, -1.0f, 0) * Time.deltaTime;
                }
            }
            else if (woodFallQuadrant == 2)
            {
                if(thisZ < 112.5f)
                {
                    transform.eulerAngles += new Vector3(0f, 45f, 112.5f) * Time.deltaTime;
                }
                else if (thisY > -5.0f)
                {
                    transform.position += new Vector3(0f, -1.0f, 0) * Time.deltaTime;
                }
            }
            else if (woodFallQuadrant == 3)
            {
                if (thisZ < 112.5f)
                {
                    transform.eulerAngles += new Vector3(0f, -45f, 112.5f) * Time.deltaTime;
                }
                else if (thisY > -5.0f)
                {
                    transform.position += new Vector3(0f, -1.0f, 0) * Time.deltaTime;
                }
            }
            else if (woodFallQuadrant == 4)
            {
                if (thisZ > 247.5f || thisZ == 0.0f)
                {
                    transform.eulerAngles += new Vector3(0f, 45f, -112.5f) * Time.deltaTime;
                }
                else if (thisY > -5.0f)
                {
                    transform.position += new Vector3(0f, -1.0f, 0) * Time.deltaTime;
                }
            }

            // 시간 경과 측정
            timeSinceFall += Time.deltaTime;

            // respawnWoodTime 후에 초기 상태로 돌아가기
            if (timeSinceFall >= respawnWoodTime)
            {
                isFalling = false;
                timeSinceFall = 0.0f; // 시간 초기화

                respawnAnimation.StartRespawnAnimation();
            }
        }
    }
    public void StartFallingAnimation(float characterRotation)
    {
        isFalling = true;
        timeSinceFall = 0.0f; // 시간 초기화
        // 나무의 체럭도 초기값으로 돌리는건 쓰러지자마자 해야함
        //this.GetComponent<InteractStatus>().interactHp = firstWoodHp;
        // 사람이 1시 방향을 바라볼 때 나무는 1사분면으로 쓰러져야함
        if (0.0f <= characterRotation && characterRotation < 90.0f)
        {
            woodFallQuadrant = 1;
        }
        // 사람이 11시 방향을 바라볼 때 나무는 2사분면으로 쓰러져야함
        else if(270.0f <= characterRotation && characterRotation < 360.0f)
        {
            woodFallQuadrant = 2;
        }
        // 사람이 7시 방향을 바라볼 때 나무는 3사분면으로 쓰러져야함
        else if (180.0f <= characterRotation && characterRotation < 270.0f)
        {
            woodFallQuadrant = 3;
        }
        // 사람이 5시 방향을 바라볼 때 나무는 4사분면으로 쓰러져야함
        else if (90.0f <= characterRotation && characterRotation < 180.0f)
        {
            woodFallQuadrant = 4;
        }
    }
}
