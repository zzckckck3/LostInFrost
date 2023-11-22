using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Net;

public class ApiRequest : MonoBehaviour
{
    // 로그인 요청 데이타 폼
    [System.Serializable]
    public class LoginData
    {
        public string email;
        public string password;
    }

    // 회원가입 요청 데이타 폼
    [System.Serializable]
    public class SignUpData
    {
        public string email;
        public string password;
        public string nickname;
    }

    // 회원 정보 변경 폼
    public class UpdateData
    {
        public string nickname;
        public string password;
    }
}
