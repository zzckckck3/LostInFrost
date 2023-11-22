using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBtnClick : MonoBehaviour
{
    public GameObject loginForm;
    public GameObject signUpForm;
    public GameObject startForm;

    // Update is called once per frame
    void Update()
    {
        if ((loginForm.activeSelf == true || loginForm.activeSelf == true) && Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickExit();
        }
    }

    public void OnClickExit()
    {
        loginForm.SetActive(false);
        signUpForm.SetActive(false);
        startForm.SetActive(true);
    }
}
