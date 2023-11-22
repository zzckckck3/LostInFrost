using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostumeInventory : MonoBehaviour
{

    // UI 오브젝트
    // 보유 코스튬 리스트 ui
    public GameObject costumeListUi;
    private List<GameObject> costumeNodes = new List<GameObject>();


    // 등급별 컬러
    public Color normalC = new Color(0.4306159f, 0.8494805f, 0.9056604f, 1.0f);
    public Color epicC = new Color(0.8374933f, 0.1398006f, 0.9622642f, 1.0f);
    public Color uniqueC = new Color(0.9622642f, 0.9341498f, 0.4302955f, 1.0f);
    public Color legendaryC = new Color(0.4313725f, 0.9607843f, 0.4354428f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        // 인벤토리 리스트의 자식 노드들(코스튬 이미지 출력 노드) 불러오기
        for (int i = 0; i < costumeListUi.transform.childCount; i++)
        {
            GameObject child = costumeListUi.transform.GetChild(i).gameObject;
            // 원하는 조건에 따라 필요한 자식 오브젝트를 orderedChildren 리스트에 추가
            costumeNodes.Add(child);
        }

        // 내가 보유한 코스튬만 코스튬 이미지 등록
        for (int i = 0; i < AuthManager.Instance.myCostumes.Count; i++)
        {
            string grade = AuthManager.Instance.myCostumes[i].costumeGrade;
            string imgName = AuthManager.Instance.myCostumes[i].costumeImage;
            string cosName = AuthManager.Instance.myCostumes[i].costumeName;
            int cosSeq = AuthManager.Instance.myCostumes[i].myCostumeSeq;

            // 등급에 맞는 색상 지정
            switch (grade)
            {
                case "normal":
                    costumeNodes[i].GetComponent<Image>().color = normalC;
                    break;
                case "epic":
                    costumeNodes[i].GetComponent<Image>().color = epicC;
                    break;
                case "unique":
                    costumeNodes[i].GetComponent<Image>().color = uniqueC;
                    break;
                case "legendary":
                    costumeNodes[i].GetComponent<Image>().color = legendaryC;
                    break;
            }

            // 코스튬에 해당하는 이미지 불러와서 할당
            Sprite cosImage = Resources.Load<Sprite>("Images/"+ imgName);
            // 코스튬 이미지는 손자 오브젝트에서 할당
            GameObject childImg = costumeNodes[i].transform.GetChild(0).gameObject;
            // 이미지 변경
            childImg.GetComponent<Image>().sprite = cosImage;

            // 해당 버튼이 무슨 코스튬 버튼인지 저장.
            TextMeshProUGUI[] CostumeText =
                costumeNodes[i].GetComponentsInChildren<TextMeshProUGUI>();
            CostumeText[0].text = cosName;
            CostumeText[1].text = grade;
            CostumeText[2].text = cosSeq.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
