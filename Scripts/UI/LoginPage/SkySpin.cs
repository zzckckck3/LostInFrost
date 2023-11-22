using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySpin : MonoBehaviour
{
    float degree = 0f;
    void Update()
    {
        degree += Time.deltaTime;
        if (degree >= 360)
            degree = 0;

        RenderSettings.skybox.SetFloat("_Rotation", degree);
    }
}
