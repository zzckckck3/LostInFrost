using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetSliderStatus : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    TextMeshProUGUI text;

    private void Update()
    {
        var status = float.Parse(text.text); 
        slider.value = status / 100.0f;
    }

}
