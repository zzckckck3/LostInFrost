using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractHpShow : MonoBehaviour
{
    [SerializeField]
    Slider hp;

    private bool respawn;
    private bool woodFall;

    void Update()
    {
        hp.value = (float)this.GetComponent<InteractStatus>().interactHp / (float)this.GetComponent<InteractStatus>().interactData.InteractHp;
        if(this.GetComponent<RespawnAnimation>() == null)
        {
            respawn = false;
        }
        else
        {
            respawn = this.GetComponent<RespawnAnimation>().isRespawn;
        }

        if (this.GetComponent<WoodFallAnimation>() == null)
        {
            woodFall = false;
        }
        else
        {
            woodFall = this.GetComponent<WoodFallAnimation>().isFalling;
        }

        if (hp.value == 1 || respawn || woodFall)
        {
            hp.gameObject.SetActive(false);
        }
        else
        {
            hp.gameObject.SetActive(true);
        }
    }

    // 모든 update가 호출된 뒤 호출..
    void LateUpdate()
    {
        hp.transform.forward = Camera.main.transform.forward;
    }
}
