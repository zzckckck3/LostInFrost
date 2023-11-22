using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnClickChangeForm : MonoBehaviour
{
    private static OnClickChangeForm instance = null;
    public static OnClickChangeForm Instance
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

    // ui 오브젝트
    public GameObject LoginForm;
    public GameObject SignUpForm;

    // 로그인 입력 값
    public TMP_InputField userEmail;
    public TMP_InputField password;

    // 회원가입 입력 값
    public TMP_InputField signUpUserEmail;
    public TMP_InputField signUpPassword;
    public TMP_InputField signUpNickName;

    // 회원가입 ui 오픈
    public void ShowSignUpUi()
    {
        LoginForm.SetActive(false);
        SignUpForm.SetActive(true);
    }

    // 로그인 ui 오픈
    public void ShowLoginUi()
    {
        LoginForm.SetActive(true);
        SignUpForm.SetActive(false);
    }

    public void CallLogin()
    {
        AuthManager.Instance.Login(userEmail.text, password.text);
    }

    public void CallSignUp()
    {
        AuthManager.Instance.SignUp(signUpUserEmail.text, signUpPassword.text, signUpNickName.text);
    }
}
