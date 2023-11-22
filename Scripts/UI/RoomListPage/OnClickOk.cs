using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickOk : MonoBehaviour
{
    public GameObject failUi;
    public void ClickOk()
    {
        failUi.SetActive(false);
    }
}
