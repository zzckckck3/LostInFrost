using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatInput : MonoBehaviour
{
    public GameObject uiInputChat; // 특정 UI 요소를 여기에 연결합니다.
    public TMP_InputField inputField; // 입력 필드를 여기에 연결합니다.
    public GameObject uiOutputChat;
    public TextMeshProUGUI outputField; // 입력 필드를 여기에 연결합니다.
    public PhotonView PV;

    [SerializeField]
    private bool isChat;

    int flag = 0;
    private void Start()
    {
        isChat = false;
    }
    void Update()
    {
        if (PV.IsMine && Input.GetKeyDown(KeyCode.Return))
        {

            // 엔터 키를 눌렀을 때 : 입력창 on
            if (uiInputChat.activeSelf) // 현재 채팅 input창 온일때 : 채팅 보냄
            {
                if(inputField.text.Length < 1)
                {
                    uiInputChat.SetActive(false);
                    return;
                }
                string chat = inputField.text;
                // 현재 text 초기화
                inputField.text = "";
                // 내 input창 끄기
                uiInputChat.SetActive(false);
                // 내 채팅 동기화
                PV.RPC("OnChat", RpcTarget.AllBuffered, chat);
            }
            else // 현재 채팅 input창 꺼져있을때
            {
                OnInput();
            }
        }
    }
    public void KeyDownEnter()
    {
        
    }
    public void OnInput()
    {
        Debug.Log("채팅치는중");
        // UI 요소를 활성화
        uiInputChat.SetActive(true);
        uiOutputChat.SetActive(false);
        // 입력 필드에 포커스 설정
        inputField.Select();
        inputField.ActivateInputField();
    }
    [PunRPC]
    public void OnChat(string chat)
    {
        uiOutputChat.SetActive(true);
        outputField.text = chat;
        StartCoroutine(WaitForThreeSeconds(chat));
    }

    IEnumerator WaitForThreeSeconds(string chat)
    {
        // 3초 동안 대기
        yield return new WaitForSeconds(3.0f);

        // 3초가 지난 후에 실행되는 코드
        if (outputField.text.Equals(chat))
        {
            uiOutputChat.SetActive(false);
        }
    }
}
