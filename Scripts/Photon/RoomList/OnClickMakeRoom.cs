using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickMakeRoom : MonoBehaviour
{
    public void ClickMakeRoom()
    {
        PhotonManagerList.instance.MakeRoom();
    }
}
