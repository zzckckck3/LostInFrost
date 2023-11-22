using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostumeInfo : MonoBehaviour
{
    private static CostumeInfo instance = null;
    public static CostumeInfo Instance
    {
        get { return instance; }
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // 현재 출력 캐릭터 코스튬 이름, 등급 출력 UI text
    public TextMeshProUGUI nowCosName;
    public TextMeshProUGUI nowCosGrade;
    public GameObject gradeForm;
    // 등급별 컬러
    public Color normalC = new Color(0.4306159f, 0.8494805f, 0.9056604f, 1.0f);
    public Color epicC = new Color(0.8374933f, 0.1398006f, 0.9622642f, 1.0f);
    public Color uniqueC = new Color(0.9622642f, 0.9341498f, 0.4302955f, 1.0f);
    public Color legendaryC = new Color(0.4313725f, 0.9607843f, 0.4354428f, 1.0f);

    public void UpdateCosInfo(string naem, string grade)
    {
        // 장착 코스튬 정보 출력
        nowCosName.text = naem; // 이름
        nowCosGrade.text = grade; // 등급


        // 등급에 맞는 색상 지정
        switch (grade)
        {
            case "normal":
                gradeForm.GetComponent<Image>().color = normalC;
                break;
            case "epic":
                gradeForm.GetComponent<Image>().color = epicC;
                break;
            case "unique":
                gradeForm.GetComponent<Image>().color = uniqueC;
                break;
            case "legendary":
                gradeForm.GetComponent<Image>().color = legendaryC;
                break;
        }

    }

}
