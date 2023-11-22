using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class FillGauge : MonoBehaviour
{
    public static FillGauge instance = null;
    public static FillGauge Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public float maxGauge = 100f; // 최대 게이지
    private Image gaugeFillImage; // 게이지 이미지
    private SlicedFilledImage slicedImage;
    public TextMeshProUGUI guageValue; // 게이지 수치

    private void Start()
    {
        slicedImage = GetComponent<SlicedFilledImage>();
    }
    public void OnFillGuage(float gauge)
    {
        // 이미지 채우기
        if (slicedImage != null)
        {
            slicedImage.fillAmount = gauge / maxGauge;
            guageValue.text = (int)gauge + "%";
        }

    }
}
