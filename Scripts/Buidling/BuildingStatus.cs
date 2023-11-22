using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStatus : MonoBehaviour
{
    public BuildingData buildingData;
    public float hp;
    public bool isInteractive;
    private AudioSource audioSource;
    public AudioClip destroySound;
    private bool destroyCheck;
    private PhotonView pv;

    private void Start()
    {
        hp = buildingData.buildingHp;
        isInteractive = buildingData.isInteractive;
        audioSource = gameObject.GetComponent<AudioSource>();
        destroyCheck = true;
        pv = gameObject.GetComponent<PhotonView>();
    }

    [PunRPC]
    public void GetDamage(int damage)
    {
        Debug.Log("빌딩RPC");
        hp -= damage;
        if (hp <= 0 && destroyCheck)
        {
            destroyCheck = false;
            StartCoroutine(DestroyBuilding());
        }
    }

    private void EndInteraction()
    {
        isInteractive = false;
        // 여기서 상호작용을 종료하는 로직을 구현하세요.
        // 예를 들어, 플레이어를 집 밖으로 보내거나 상호작용 UI를 숨길 수 있습니다.
    }

    private IEnumerator DestroyBuilding()
    {
        PlaySound(30f, 0.4f); // 사운드 재생
        yield return new WaitForSeconds(destroySound.length); // 사운드 길이만큼 대기

        EndInteraction();
        if (pv.IsMine)
        {
            int viewID = pv.ViewID;
            NetworkManager.Instance.GetComponent<PhotonView>().RPC("ReturnBuildingPools", RpcTarget.All, viewID);
        }
    }

    // 
    public void PlaySound(float maxDistance, float volume = 1.0f) // volume 파라미터를 추가합니다.
    {
        Vector3 localPlayerPosition = Camera.main.transform.position;
        localPlayerPosition.y = this.transform.position.y;
        localPlayerPosition.z += 8f;
        // 최대거리보다 거리가 더 길면 소리 안들리게하는 조건문
        if (maxDistance > 0)
        {
            float distance = Vector3.Distance(localPlayerPosition, transform.position);

            if (distance > maxDistance)
            {
                return;
            }
        }

        // 볼륨을 설정합니다.
        audioSource.volume = volume;
        audioSource.PlayOneShot(destroySound);
    }
}
