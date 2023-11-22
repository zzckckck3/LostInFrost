using ECM.Examples;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimatorLayer : MonoBehaviour
{
    // 캐릭터의 애니메이터
    private Animator animator;
    private PhotonView pv;
    private UserStatusManager userStatusManager;
    // Animator의 Layer
    public bool isBaseLayer { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        // 초기화
        animator = GetComponentInChildren<Animator>();
        isBaseLayer = true;
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            userStatusManager = FindObjectOfType<UserStatusManager>();
            if(userStatusManager != null)
            {
                userStatusManager.changeAnimator = this;
                userStatusManager.anim = gameObject.GetComponentInChildren<Animator>();
            }
        }
    }


    // BaseLayer(기본 이동모드)로 애니메이터를 바꾸는(가중치 조절로) 함수
    public void ToBaseLayer()
    {
        isBaseLayer = true;
        animator.SetLayerWeight(animator.GetLayerIndex("Base Layer"), 1);
        animator.SetLayerWeight(animator.GetLayerIndex("Weapon Layer"), 0);
    }

    // Weapon(공격 이동모드)로 애니메이터를 바꾸는(가중치 조절로) 함수
    public void ToWeaponLayer()
    {
        isBaseLayer = false;
        animator.SetFloat("Forward", 0f);
        animator.SetFloat("Turn", 0f);
        animator.SetLayerWeight(animator.GetLayerIndex("Base Layer"), 0);
        animator.SetLayerWeight(animator.GetLayerIndex("Weapon Layer"), 1);
    }
}
