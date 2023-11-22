using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CostumeManager;

public class CostumeCharacterSet : MonoBehaviour
{
    // 코스튬 캐릭터 출력 ui form
    public GameObject Abilities;
    // 모든 코스튬 캐릭터 리스트
    public GameObject costumeCharacterList;
    // 현재 출력 코스튬 캐릭터 
    public GameObject nowCosChar;
    // 현재 출력 코스튬 이름
    public static string cosName;
    // 현재 출력 코스튬 등급
    public static string cosGrade;
    // 현재 출력 코스튬 seq
    public static string cosSeq;

    // 현재 출력 코스튬 장착 여부 출력 오브젝트
    public static GameObject IsOn;

    void Start()
    {
        IsOn = Abilities.transform.Find("IsOn").gameObject;
        // 좌측 코스튬 캐릭터 출력 정보 저장.
        string nowCosName = AuthManager.Instance.userData.costumeName;
        cosName = nowCosName;
        string nowCosGrade = AuthManager.Instance.userData.costumeGrade;
        cosGrade = nowCosGrade;
        string nowCosSeq = AuthManager.Instance. userData.costumeSeq.ToString();
        cosSeq = nowCosSeq;

        // 부모 오브젝트 내에서 자식 오브젝트를 찾기
        nowCosChar = costumeCharacterList.transform.Find(nowCosName).gameObject;
        // 출력
        nowCosChar.SetActive(true);
        IsOn.SetActive(true);

        // 코스튬 정보 업데이트
        CostumeInfo.Instance.UpdateCosInfo(cosName, cosGrade);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
