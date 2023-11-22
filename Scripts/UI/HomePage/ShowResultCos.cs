using ARPGFX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltimateClean;
using UnityEngine;
using UnityEngine.UI;

public class ShowResultCos : MonoBehaviour
{
    private static ShowResultCos instance = null;
    public static ShowResultCos Instance
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

    // 출력할 코스튬 리스트를 담은 오브젝트
    public GameObject costumeList;
    // 출력할 코스튬 이름 담을 오브젝트
    public TextMeshProUGUI costumeName;
    // 출력할 코스튬 등급 담을 오브젝트
    public TextMeshProUGUI costumeGrade;
    // 현재 출력중인 코스튬
    public GameObject resultCostume;
    // 현재 결재방식
    public string costType;
    // 보유 여부
    public GameObject isHave;
    // 재구매 버튼
    public GameObject retryBtn;
    // 확인 버튼
    public GameObject okBtn;
    // 나가기 버튼
    public GameObject exitBtn;
    // 버튼 비활성화 색상
    public Color offColor;
    // 버튼 활성화 색상
    public Color onColor;
    // 상자 프리팹
    public GameObject boxPrefab;
    // 상자 오브젝트
    public GameObject boxUi;
    // 중복 텍스트 컬러
    public Color nNewC;
    // 뉴 텍스트 컬러
    public Color newC;
    // 레전더리 뽑았을때 파티클
    public GameObject particleL;
    // 등급별 오로라 파티클 오브젝트
    public GameObject particleNormal;
    public GameObject particleEpic;
    public GameObject particleUnique;
    public GameObject particleLegendary;

    // 등급별 컬러
    public Color normalC = new Color(0.4313726f, 0.5473188f, 0.9058824f, 1.0f);
    public Color epicC = new Color(0.8374933f, 0.1398006f, 0.9622642f, 1.0f);
    public Color uniqueC = new Color(0.9622642f, 0.9341498f, 0.4302955f, 1.0f);
    public Color legendaryC = new Color(0.4313725f, 0.9607843f, 0.7756789f, 1.0f);

    // 현재 뽑기 결과
    public AuthManager.MyCostume nowResultCostume; // 뽑은 코스튬 
    public int isNew; // 중복여부
    public string goodsType; // 결재 재화

    // 뽑기 사운드
    AudioSource sparkling;
    AudioSource opening;
    AudioSource newOne;

    private void Start()
    {
        sparkling = GetComponentsInChildren<AudioSource>()[0];
        opening = GetComponentsInChildren<AudioSource>()[1];
        newOne = GetComponentsInChildren<AudioSource>()[2];
    }
    // 뽑기 결과 응답
    public void ResResult(AuthManager.MyCostume resultCostume, int result, string type)
    {
        nowResultCostume = resultCostume;
        isNew = result;
        goodsType = type;
    }

    // 첫 뽑기 시작 호출
    public void CallStart()//string type
    {
        // 딜레이 동안 버튼 무력화
        retryBtn.GetComponent<FadeButton>().enabled = false; // 버튼 클릭 비활성화
        okBtn.GetComponent<FadeButton>().enabled = false; // 버튼 클릭 비활성화
        exitBtn.GetComponent<FadeButton>().enabled = false; // 버튼 클릭 비활성화

        // 버튼 색 조정
        Image image = retryBtn.GetComponent<Image>();
        image.color = offColor;

        Image image1 = okBtn.GetComponent<Image>();
        Color color = image1.color;
        color.a = 0.3f;
        image1.color = color;
        // 결과 출력 호출 메서드 호출 3초뒤.
        StartCoroutine(ShowResult(3.0f));//, type

        // 효과음 출력
        OpenSoundStart();
    }
    // 결과 출력 호출 메서드
    private IEnumerator ShowResult(float delay)//, string type
    {
        yield return new WaitForSeconds(delay);
        ShowCostume(nowResultCostume, isNew, goodsType);
    }

    public void ShowCostume(AuthManager.MyCostume myCos, int flag, string type)
    {
        // 중복 여부 확인
        // 보유중
        isHave.SetActive(true); 
        if (flag == 0)
        {
            isHave.GetComponent<TextMeshProUGUI>().text = "보유중";
            isHave.GetComponent<TextMeshProUGUI>().color = nNewC;
        }
        // 뉴페이스
        else
        {
            isHave.GetComponent<TextMeshProUGUI>().text = "N E W";
            isHave.GetComponent<TextMeshProUGUI>().color = newC;
            
            particleL.SetActive(false);
            particleL.SetActive(true);

            // 새로운 코스튬일때 사운드
            newOne.Play();
        }
        // 현재 결재 타입 저장
        costType = type;

        // 뽑은 코스튬 오브젝트 찾아서 출력
        resultCostume = costumeList.transform.Find(myCos.costumeName).gameObject;
        resultCostume.SetActive(true);

        // 뽑은 코스튬 이름
        costumeName.text = myCos.costumeName;

        // 뽑은 코스튬 등급
        costumeGrade.text = myCos.costumeGrade;

        // 등급에 맞는 색상 지정
        switch (myCos.costumeGrade)
        {
            case "normal":
                costumeGrade.color = normalC;
                particleNormal.SetActive(true);
                break;
            case "epic":
                costumeGrade.color = epicC;
                particleEpic.SetActive(true);
                break;
            case "unique":
                costumeGrade.color = uniqueC;
                particleUnique.SetActive(true);
                break;
            case "legendary":
                costumeGrade.color = legendaryC;
                particleLegendary.SetActive(true);
                break;
        }

        // 결과 출력 후 재화상태 업데이트
        AuthManager.Instance.ReqUpdateUserGoods();

        
    }

    public void OnClickRetry()
    {
        // 코인 결재일때 있는지 확인
        if (goodsType.Equals("coin")){
            if (AuthManager.Instance.userData.coin < ShopClickBuy.Instance.CostCoin)
            {
                ResultInfo.Instance.LaunchNotification(false, "코인이 부족합니다.");
                return;
            }
        }
        // 크리스탈 결재일떄 있는지 확인
        else
        {
            if (AuthManager.Instance.userData.crystal < ShopClickBuy.Instance.CostCrystal)
            {
                ResultInfo.Instance.LaunchNotification(false, "크리스탈이 부족합니다.");
                return;
            }
        }
        // 딜레이 동안 버튼 무력화
        retryBtn.GetComponent<FadeButton>().enabled = false; // 버튼 클릭 비활성화
        okBtn.GetComponent<FadeButton>().enabled = false; // 버튼 클릭 비활성화
        exitBtn.GetComponent<FadeButton>().enabled = false; // 버튼 클릭 비활성화

        // 버튼 색 조정
        Image image = retryBtn.GetComponent<Image>();
        image.color = offColor;

        Image image1 = okBtn.GetComponent<Image>();
        Color color = image1.color;
        color.a = 0.3f;
        image1.color = color;

        resultCostume.SetActive(false);
        // 기존 파티클 효과 off
        particleNormal.SetActive(false);
        particleEpic.SetActive(false);
        particleUnique.SetActive(false);
        particleLegendary.SetActive(false);

        // 뽑은 코스튬 이름
        costumeName.text = "";

        // 뽑은 코스튬 등급
        costumeGrade.text = "";

        isHave.SetActive(false);
        // 뽑기 api호출
        CostumeManager.Instance.ReqPurchaseCostume(costType);
    }

    // 버튼 활성화 메서드
    public void SetOnBtn()
    {
        // 버튼 활성화
        Image image = retryBtn.GetComponent<Image>();
        image.color = onColor;

        Image image1 = okBtn.GetComponent<Image>();
        Color color = image1.color;
        color.a = 1.0f;
        image1.color = color;
        // 스크립트를 활성화
        retryBtn.GetComponent<FadeButton>().enabled = true;
        okBtn.GetComponent<FadeButton>().enabled = true;
        exitBtn.GetComponent<FadeButton>().enabled = true;
    }

    public void OpenSoundStart()
    {
        sparkling.Play();
        Invoke("OpenSoundEnd", 1.5f);
    }

    public void OpenSoundEnd()
    {
        opening.Play();
    }
}
/*
 * 1. 시작시 바로 api 호출 및 결과 받으면
 * 2. 상자 오픈 이팩트 시작(3초) : 결과에 따른 상자 이팩트 변경
 * 2. 상자 이팩트가 끝나기 전까지 버튼 비활성화
 * 3. 3초가 지나면(상자 이팩트가 끝나는 시점) 버튼 활성화 및 결과 출력
 * 4. 다시뽑기.
 * 
 */