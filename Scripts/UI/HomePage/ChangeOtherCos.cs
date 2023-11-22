using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOtherCos : MonoBehaviour
{

    public GameObject IsOn;
    public Button btn;
    public void OnclickPutOn()
    {
        AuthManager.Instance.ReqUpdateUserDataCostume(CostumeCharacterSet.cosName, 
            CostumeCharacterSet.cosGrade, int.Parse(CostumeCharacterSet.cosSeq));

        IsOn.SetActive(true);

        // 장착 중일때 버튼 색
        Image image = btn.GetComponent<Image>();
        image.color = new Color(0.3181163f, 0.8301887f, 0.198932f, 1.0f);
        // 장착 중일때 텍스트
        TextMeshProUGUI textComponent = btn.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = "장착중";
    }
}
