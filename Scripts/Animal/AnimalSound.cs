using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;

public class AnimalSound : MonoBehaviourPunCallbacks
{
    // 동물의 특별 사운드
    public AudioClip specialSound;
    // 동물의 걷는 사운드
    public AudioClip walkSound;
    // 동물의 뛰는 사운드
    public AudioClip runSound;
    // 동물의 공격 사운드
    public AudioClip attackSound;
    // 동물의 죽는 사운드
    public AudioClip deadSound;
    // 동물의 오디오 소스
    public AudioSource audioSource;
    public float someMaxDistance = 20f;
    public PhotonView pv;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
    }
    
    // 애니메이션 이벤트에 사용할 메서드
    public void PlaySoundByName(string clipName)
    {
        if (pv.IsMine)
        {
            PlaySound(GetAudioClipByName(clipName));
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (pv.IsMine && clip != null)
        {
            pv.RPC("PlayAnimalSoundRPC", RpcTarget.All, pv.ViewID, clip.name);
        }
    }
    
    [PunRPC]
    void PlayAnimalSoundRPC(int viewID, string clipName)
    {
        Vector3 localPlayerPosition = Camera.main.transform.position;
        localPlayerPosition.y = this.transform.position.y;
        localPlayerPosition.z += 8f;
        //GameObject player = PhotonView.Find(viewID).gameObject;
        float distance = Vector3.Distance(localPlayerPosition, this.transform.position);

        if (distance < someMaxDistance) // 'someMaxDistance'는 최대 들리는 거리를 나타냄
        {
            float volume = CalculateVolumeBasedOnDistance(distance);
            AudioClip clip = GetAudioClipByName(clipName+"Sound");
            if (clip != null)
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }
    }
    
    private AudioClip GetAudioClipByName(string clipName)
    {
        //PlaySoundByName
        switch (clipName)
        {
            case "specialSound":
                return specialSound;
            case "walkSound":
                return walkSound;
            case "runSound":
                return runSound;
            case "attackSound":
                return attackSound;
            case "deadSound":
                return deadSound;
            default:
                return null;
        }
    }
    
    private float CalculateVolumeBasedOnDistance(float distance)
    {
        // 예: 거리에 따라 0에서 1 사이의 볼륨을 반환
        return Mathf.Clamp(1 - distance / someMaxDistance, 0, 1);
    }
}
