using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueChange : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI levelText;

    void Start()
    {
        // 슬라이더 최소, 최댓값 설정
        // minValue와 maxValue를 float 값으로 설정합니다.
        slider.minValue = 0.5f;
        slider.maxValue = 1.5f;
    }


    public void SliderValueChanged()
    {
        float value = slider.GetComponent<Slider>().value;
        levelText.text = Math.Round(value, 1) + "";
    }
}
