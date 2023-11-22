using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireScript : MonoBehaviour
{
    [SerializeField]
    private float destroyTime;
    [SerializeField]
    private float coldChangeValue;
    private PhotonView pv;
    public string playerTag = "Player"; // 플레이어 태그

    void Start()
    {
        pv = this.GetComponent<PhotonView>();
        StartCoroutine(AutoDestroyAfterDelay(destroyTime)); // 예: 60초 후 파괴
    }

    private void OnTriggerStay(Collider other)
    {
        PhotonView collsionPv = other.gameObject.GetComponent<PhotonView>();
        if (collsionPv != null && collsionPv.IsMine && other.CompareTag(playerTag))
        {
            UserStatusManager.Instance.Cold -= coldChangeValue * Time.deltaTime;
        }

            
    }

    private IEnumerator AutoDestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 오브젝트가 여전히 존재하는지 확인
        if (gameObject != null && pv.IsMine)
        {
            int viewID = pv.ViewID;
            NetworkManager.Instance.GetComponent<PhotonView>().RPC("ReturnBuildingPools", RpcTarget.All, viewID);
        }
    }
}
