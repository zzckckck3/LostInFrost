using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatus : MonoBehaviour
{
    public ItemData itemData;
    public string itemName;
    public string itemDescription;
    public ItemType itemType;
    public int itemQuantity;
    private PhotonView pv;
    public Transform parent;
    public float destroyTime;
    // Start is called before the first frame update
    void Awake()
    {
        itemName = itemData.itemName;
        itemDescription = itemData.itemDescription;
        itemType = itemData.itemType;
        itemQuantity = itemData.itemQuantity;
        pv = GetComponent<PhotonView>();
        destroyTime = 60f;
    }

    private void OnEnable()
    {
        StartCoroutine(AutoDestroyAfterDelay(destroyTime)); // 예: 60초 후 파괴
    }

    private IEnumerator AutoDestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 오브젝트가 여전히 존재하는지 확인
        RequestDestroyItem();
    }

    // 다른 스크립트에서 호출할 수 있는 퍼블릭 메서드
    public void RequestDestroyItem()
    {
        int viewID = pv.ViewID;
        NetworkManager.Instance.GetComponent<PhotonView>().RPC("ReturnInventoryPools", RpcTarget.All, viewID);
    }
}
