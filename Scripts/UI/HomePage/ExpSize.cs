using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpSize : MonoBehaviour
{
    void Start()
    {
        float rate = AuthManager.Instance.userData.experience * 0.01f;
        float expLen = 250 * rate;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        // 현재 RectTransform의 크기를 가져와 너비만 변경
        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.x = expLen;

        // 크기를 설정한 값으로 업데이트
        rectTransform.sizeDelta = sizeDelta;
    }
}
