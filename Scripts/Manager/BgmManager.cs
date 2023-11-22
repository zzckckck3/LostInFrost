using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    private static BgmManager instance = null;
    public static BgmManager Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        if (instance == null)
        {
            // 이 GameManager 오브젝트가 다른 씬으로 전환될 때 파괴되지 않도록 함
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // 이미 존재하는 GameManager 오브젝트가 있으므로 이 인스턴스를 파괴
            Destroy(gameObject);
        }

    }

    AudioSource bgm1;
    AudioSource bgm2;
    AudioSource bgm3;
    private bool isPlayBgm1 = true;
    private bool isPlayBgm2 = false;
    private bool isPlayBgm3 = false;
    private void Start()
    {
        bgm1 = GetComponentsInChildren<AudioSource>()[0];
        bgm2 = GetComponentsInChildren<AudioSource>()[1];
        bgm3 = GetComponentsInChildren<AudioSource>()[2];
    }
    public void StopBgm1()
    {
        bgm1.Stop();
        isPlayBgm1 = false;
    }
    public void PlayBgm1()
    {
        bgm1.Play();
        isPlayBgm1 = true;
    }

    public bool IsPlayBgm1
    {
        get { return isPlayBgm1; }
    }

    public void StopBgm2()
    {
        bgm2.Stop();
        bgm3.Stop();
        isPlayBgm2 = false;
        isPlayBgm3 = false;
    }
    public void PlayBgm2()
    {
        bgm2.Play();
        bgm3.Play();
        isPlayBgm2 = true;
        isPlayBgm3 = true;
    }

    public bool IsPlayBgm2
    {
        get { return isPlayBgm2; }
    }
    public bool Bgm3
    {
        get { return bgm3; }
    }
}
