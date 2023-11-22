using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterTrigger : MonoBehaviour
{
    public string playerTag = "Player"; // 플레이어 태그

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(playerTag))
        {
            InGameManager.Instance.isTriggeredWithHelicopter = true;
            Debug.Log(InGameManager.Instance.isTriggeredWithHelicopter);
            Debug.Log(other);
        }
    }
}
