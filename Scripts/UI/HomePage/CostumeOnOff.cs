using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CostumeManager;

public class CostumeOnOff : MonoBehaviour
{
    // 모든 캐릭터 코스튬 리스트
    public GameObject costumeCharacterList;
    // 현재 출력 캐릭터 코스튬
    public GameObject nowCosChar;
    // 현재 버튼의 캐릭터
    public string name;
    // 현재 버튼의 캐릭터 넘버(myCosSeq)
    public int seq;
    // 장착 버튼
    public Button myButton;

    public void OnClickOtherCostume()
    {
        TextMeshProUGUI[] CostumeTexts =
                gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        // 보유중인 코스튬이 아닐때.
        if (CostumeTexts[0].text.Equals("Empty")) {
            return;
        }
        // 현재 보여지고 있는 코스튬 off
        // 부모 오브젝트 내에서 자식 오브젝트를 찾기
        nowCosChar = costumeCharacterList.transform.Find(CostumeCharacterSet.cosName).gameObject;
        nowCosChar.SetActive(false);

        // 클릭한 다른 코스튬 on
        // 클릭한 다른 코스튬 이름
        CostumeCharacterSet.cosName = CostumeTexts[0].text; // 현재 코스튬 정보 업데이트 : 이름
        CostumeCharacterSet.cosGrade = CostumeTexts[1].text; // 현재 코스튬 정보 업데이트 : 등급
        CostumeCharacterSet.cosSeq = CostumeTexts[2].text; // 현재 코스튬 정보 업데이트 : 시퀀스

        nowCosChar = costumeCharacterList.transform.Find(CostumeCharacterSet.cosName).gameObject;
        nowCosChar.SetActive(true);
        
        // 장착 여부 검증
        IsOn();

        // 코스튬 정보 업데이트
        CostumeInfo.Instance.UpdateCosInfo(CostumeTexts[0].text, CostumeTexts[1].text);

    }

    // 장착 여부 출력 및 버튼 변경.
    public void IsOn()
    {
        // 보여지는 코스튬이 내가 장착중인 코스튬일때 장착여부 출력
        if (CostumeCharacterSet.cosName.Equals(AuthManager.Instance.userData.costumeName))
        {
            CostumeCharacterSet.IsOn.SetActive(true);
            // 장착 중일때 버튼 색
            Image image = myButton.GetComponent<Image>();
            image.color = new Color(0.3181163f, 0.8301887f, 0.198932f, 1.0f);
            // 장착 중일때 텍스트
            TextMeshProUGUI textComponent = myButton.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = "장착중";
        }
        else
        {
            CostumeCharacterSet.IsOn.SetActive(false);

            // 장착중이지 않을때 버튼 색
            Image image = myButton.GetComponent<Image>();
            image.color = new Color(0.682353f, 0.2431373f, 0.7882353f, 1.0f);
            // 작착중이지 않을때 버튼 텍스트
            TextMeshProUGUI textComponent = myButton.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = "장 착";
        }
    }

}
