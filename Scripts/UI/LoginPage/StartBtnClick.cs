using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartBtnClick : MonoBehaviour
{
    private static StartBtnClick instance = null;
    public static StartBtnClick Instance
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
    public TextMeshProUGUI StartBtnText;
    public GameObject startFrom;
    public GameObject loginForm;
    // 로그아웃 버튼'
    public GameObject logOutBtn;
    // Start is called before the first frame update
    void Start()
    {
        string token = GetToken.Instance.ReadValue();

        if (token.Length == 0)
        {
            StartBtnText.text = "L O G I N";
            logOutBtn.SetActive(false);
            return;
        }
        AuthManager.Instance.accessToken = token;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnclickStart();
        }
    }

    // 시작페이지 start버튼 클릭
    public void OnclickStart()
    {
        if(StartBtnText.text.Equals("S T A R T"))
        {
            SceneManager.LoadScene("Home");
            return;
        }
        startFrom.SetActive(false);
        loginForm.SetActive(true);
    }

    // 로그인 성공시
    public void AfterLogin()
    {
        loginForm.SetActive(false);
        startFrom.SetActive(true);
        StartBtnText.text = "S T A R T";
        logOutBtn.SetActive(true);
    }
}
