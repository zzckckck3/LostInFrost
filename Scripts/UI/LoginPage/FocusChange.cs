using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FocusChange : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public Button button;

    void Update()
    {
        if(!inputEmail.isFocused && !inputPassword.isFocused)
        {
            inputEmail.ActivateInputField();
        }
        // 탭 키 입력 확인
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inputEmail.isFocused)
            {
                inputPassword.ActivateInputField();
            }
            else if(inputPassword.isFocused)
            {
                inputEmail.ActivateInputField();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            button.onClick.Invoke();
        }
    }


}
