using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ready : MonoBehaviour
{
    void Update()
    {
        
    }

    public void OnclickReady()
    {
        PhotonManagerWaitingRoom.Instance.Ready();
    }
}
