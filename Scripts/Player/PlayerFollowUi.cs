using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowUi : MonoBehaviour
{

    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
