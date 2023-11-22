using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogOut : MonoBehaviour
{
    public static LogOut instance;
    public static LogOut Instance
    {
        get { return instance; }
    }


    // 일반 로그 아웃
    public void OnclickLogOut()
    {
        if (!SceneManager.GetActiveScene().name.Equals("SampleLogin")) // 포톤에 연결된 씬일 경우
        {
            OnClickLogOutWithPhoton();
            return;
        }
        AuthManager.Instance.LogOut();
        SceneManager.LoadScene("SampleLogin");
    }
    // 포톤 연결상태 로그 아웃
    public void OnClickLogOutWithPhoton()
    {
        AuthManager.Instance.LogOut();
        PhotonNetwork.Disconnect();
        try
        {
            Destroy(UserStatusManager.Instance.gameObject);
        }
        catch(Exception e)
        {
            Debug.Log("삭제할게 없음.");
        }
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        PhotonNetwork.LoadLevel("SampleLogin");
        //SceneManager.LoadScene("SampleLogin");
    }
}
