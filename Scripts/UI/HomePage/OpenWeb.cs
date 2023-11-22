using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWeb : MonoBehaviour
{
    public void OpenChrome()
    {
        string url = "https://k9c101.p.ssafy.io/shop/costume"; // 원하는 URL로 변경
        Application.OpenURL(url);
    }
    public void OpenSignUp()
    {
        string url = "https://k9c101.p.ssafy.io/user/login"; // 원하는 URL로 변경
        Application.OpenURL(url);
    }
}
