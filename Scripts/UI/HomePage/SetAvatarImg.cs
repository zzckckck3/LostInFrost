using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAvatarImg : MonoBehaviour
{

    private static SetAvatarImg instance = null;
    public static SetAvatarImg Instance
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

    private void Start()
    {
        SetImg();
    }

    public GameObject Avatar;

    public void SetImg()
    {
        // 아바타 이미지 변경.
        // 코스튬에 해당하는 이미지 불러와서 할당
        Sprite cosImage = Resources.Load<Sprite>("Images/" + AuthManager.Instance.userData.costumeName);
        Debug.Log("이미지 있나?" + cosImage);
        // 이미지 변경
        Avatar.GetComponent<Image>().sprite = cosImage;
    }
}
