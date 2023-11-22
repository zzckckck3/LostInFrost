using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowCoustumeSet : MonoBehaviour
{
    private static NowCoustumeSet instance = null;
    public static NowCoustumeSet Instance
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
    public GameObject nowCostume;

    void Start()
    {
        // 현재 장착중인 코스튬 정보 찾기
        nowCostume = gameObject.transform.Find(AuthManager.Instance.userData.costumeName).gameObject;
        // 해당 코스튬 내 캐릭터에 입혀서 on
        nowCostume.SetActive(true);
    }

    // 초기 캐릭터 코스튬 세팅
    public void SetFirstCostume()
    {
        // 현재 장착중인 코스튬 정보 찾기
        nowCostume = gameObject.transform.Find(AuthManager.Instance.userData.costumeName).gameObject;
        // 해당 코스튬 내 캐릭터에 입혀서 on
        nowCostume.SetActive(true);
    }

    // 캐릭터 코스튬 변경
    public void ChangeMyCharacterCostume(string cosName)
    {
        nowCostume.SetActive(false); // 현재 입은 코스튬 끄기.

        // 장착할 코스튬 찾기
        nowCostume = gameObject.transform.Find(cosName).gameObject;
        // 입기
        nowCostume.SetActive(true);
    }
}
