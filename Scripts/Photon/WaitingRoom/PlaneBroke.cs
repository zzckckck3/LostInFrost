using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaneBroke : MonoBehaviour
{
    public static PlaneBroke instance = null;
    public static PlaneBroke Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public float rotationSpeed = 10.0f; // 회전 속도
    public float targetRotationAngleZ = -70.0f; // Z 축 목표 회전 각도
    public float targetRotationAngleY = -70.0f; // Y 축 목표 회전 각도
    private float currentRotationAngleZ = 0.0f; // 현재 Z 축 회전 각도
    private float currentRotationAngleY = 0.0f; // 현재 Y 축 회전 각도
    public int flag = 0;

    public float animationSpeed = 0.5f; // 애니메이션 속도
    private bool increasing = true; // 알파 값 증가 여부
    public GameObject backGround; // 백그라운드 오브젝트.
    private Image image; // Canvas의 Image 컴포넌트

    AudioSource infoSound; // 시작시 오디오 : 기장 안내
    AudioSource bombSound; // 시작시 오디오 : 폭발음
    AudioSource sirenSound; // 시작시 오디오 : 폭발음

    public GameObject fireAndSmoge; // 불꽃, 연기 이팩트
    public GameObject fireAndSmoge2; // 불꽃, 연기 이팩트

    public GameObject beforeInGame; // 인게임 직전 ui
    private Image imageInfo; // 인게임 직전 ui 이미지
    public float fadeInDuration = 2.0f; // 서서히 fadeIn이 완료되는 데 걸리는 시간
    public TextMeshProUGUI infoTitle; 
    public TextMeshProUGUI infoContent;

    void Start()
    {
        infoSound = GetComponentsInChildren<AudioSource>()[0];
        bombSound = GetComponentsInChildren<AudioSource>()[1];
        sirenSound = GetComponentsInChildren<AudioSource>()[2];
        // Canvas의 Image 컴포넌트 가져오기
        image = backGround.GetComponent<Image>();
        imageInfo = beforeInGame.GetComponent<Image>();
        // 애니메이션을 시작합니다.
        StartAlphaAnimation();
    }

    void Update()
    {
        // 비행기 조난 신호 : 게임 시작 신호
        if(flag == 1)
        {
            // Z 축 목표 회전 각도에 도달하지 않았다면 Z 축 회전

            if (currentRotationAngleZ > targetRotationAngleZ)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.forward * rotationStep*-1);
                currentRotationAngleZ -= rotationStep;
            }

            // Y 축 목표 회전 각도에 도달하지 않았다면 Y 축 회전
            if (currentRotationAngleY > targetRotationAngleY)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up * rotationStep);
                currentRotationAngleY -= rotationStep;
            }

            // 알파 값을 조절하여 효과를 만듭니다.
            AdjustAlpha();

            // 애니메이션이 끝나면 다시 반복합니다.
            if (image.color.a <= 0 || image.color.a >= 0.5f)
            {
                increasing = !increasing;
            }
        }

        
    }
    void AdjustAlpha()
    {
        // increasing이 true이면 알파 값을 증가시키고, false이면 감소시킵니다.
        float alphaChange = increasing ? Time.deltaTime * animationSpeed : -Time.deltaTime * animationSpeed;

        // 현재 알파 값을 가져와서 변경된 값으로 설정합니다.
        Color currentColor = image.color;
        currentColor.a += alphaChange;
        image.color = currentColor;
    }

    void StartAlphaAnimation()
    {
        // 애니메이션을 시작하기 전 초기 알파 값을 설정합니다.
        Color initialColor = image.color;
        initialColor.a = 0f;
        image.color = initialColor;
    }

    // 게임 시작시
    public void StartInfoSound()
    {
        // bgm1제거
        BgmManager.Instance.StopBgm1();

        beforeInGame.SetActive(true);
        infoSound.Play();
        Invoke("FinishInfoSound", 3.7f);
    }
    public void FinishInfoSound()
    {
        infoSound.Stop();
        bombSound.Play();
        fireAndSmoge.SetActive(true);
        fireAndSmoge2.SetActive(true);
        flag = 1;
        CameraSwing.instance.flag = 1;
        Invoke("SirenOn", 1.5f);
    }
    public void SirenOn()
    {
        sirenSound.Play();
        Invoke("InfoUiOn", 7.0f);
    }
    public void InfoUiOn()
    {
        StartCoroutine(FadeOutSoundVolume());
        StartCoroutine(FadeInImage());
    }
    IEnumerator FadeInImage()
    {
        float elapsed = 0.0f;
        Color originalColor = imageInfo.color;

        while (elapsed < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            imageInfo.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator FadeOutSoundVolume()
    {
        float elapsed = 0.0f;
        float originalVolume = sirenSound.volume;

        while (elapsed < fadeInDuration)
        {
            float targetVolume = Mathf.Lerp(originalVolume, 0f, elapsed / fadeInDuration);
            sirenSound.volume = targetVolume;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // fadeOut 완료 후 필요한 작업 수행
        StartCoroutine(FadeInTextColorTtile());
    }

    IEnumerator FadeInTextColorTtile()
    {
        float elapsed = 0.0f;
        Color originalColor = infoTitle.color;

        while (elapsed < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            infoTitle.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // fadeIn 완료 후 필요한 작업 수행
        StartCoroutine(FadeInTextColorContent());
    }
    IEnumerator FadeInTextColorContent()
    {
        float elapsed = 0.0f;
        Color originalColor = infoContent.color;

        while (elapsed < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            infoContent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // fadeIn 완료 후 필요한 작업 수행
        Invoke("TextOff", 2.0f);

    }

    public void TextOff()
    {
        StartCoroutine(FadeOutTextColorContent());
    }
    IEnumerator FadeOutTextColorContent()
    {
        float elapsed = 1.0f;
        Color originalColor = infoContent.color;

        while (elapsed > 0)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            infoContent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            infoTitle.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed -= Time.deltaTime;
            yield return null;
        }

        // 게임 시작 데이터 저장
        string startTime = PhotonNetwork.CurrentRoom.CustomProperties["startTime"] + "";
        string roomSeq = PhotonNetwork.CurrentRoom.CustomProperties["uuid"] + "";
        string gameLevelString = PhotonNetwork.CurrentRoom.CustomProperties["gameLevel"].ToString();
        float gameLevel;
        if (float.TryParse(gameLevelString, out gameLevel)) { }// 변환 성공
        else
        {
            // 변환 실패
            gameLevel = 0.8f;
        }
        GameApiManager.Instance.ReqGameStart(0f, false, roomSeq, startTime, gameLevel);
    }

    // 씬 이동 호출
    public void changeIntoInGameScene()
    {
        //방장이 이동 시도
        if (PhotonNetwork.LocalPlayer.NickName.Equals(PhotonNetwork.CurrentRoom.CustomProperties["captain"]))
        {
            PhotonNetwork.LoadLevel("Frost");
            //PhotonNetwork.LoadLevel("SuminDemoScene");
        }
    }
}
