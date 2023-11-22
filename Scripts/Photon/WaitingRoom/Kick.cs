using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Kick : MonoBehaviour
{
    public TextMeshProUGUI playerNickname;
    public void OnclickKick()
    {
        PhotonManagerWaitingRoom.Instance.KickPlayer(playerNickname.text);
    }
}
