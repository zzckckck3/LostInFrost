using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GetToken : MonoBehaviour
{
    private static GetToken instance = null;
    public static GetToken Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        ReadValue();
        if (instance == null)
        {
            instance = this;
        }
    }
    public TextMeshProUGUI text;

    public string ReadValue()
    {
        string[] arrguments = Environment.GetCommandLineArgs();
        string token = "";

        for(int i = 0; i<arrguments.Length; i++)
        {
            if (arrguments[i].Substring(0, 7).Equals("/token="))
            {
                try
                {
                    token = arrguments[i];
                    // 여기에서 token을 사용하거나 처리
                    token = token.Substring(7);
                }
                catch (Exception e)
                {
                    token = "";
                }
                break;
            }
        }

        if (AuthManager.Instance.isLogOut == 0)
            return token;
        else
            return "";
    }
}
