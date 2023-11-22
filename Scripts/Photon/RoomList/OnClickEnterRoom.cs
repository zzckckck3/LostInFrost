using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnClickEnterRoom : MonoBehaviour
{
    public TextMeshProUGUI roomTitle;

    public void TryEnterRoom()
    {
        PhotonManagerList.instance.EnterRoom(roomTitle.text);
    }
}
