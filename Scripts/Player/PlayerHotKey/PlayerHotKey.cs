using Photon.Pun;
using UnityEngine;

public class PlayerHotKey : MonoBehaviour
{
    public GameObject hand;
    public PlayerAudio playerAudio;
    public Animator anim;
    private PhotonView pv;
    public int hotkeyIndex;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            InventoryManager.Instance.hotkey = this;
        }
        InventoryManager.Instance.hotkey = this;
        hotkeyIndex = -1;
    }

    void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        if(hotkeyIndex != -1 && InventoryManager.Instance.equippedItem != null 
            && (InventoryManager.Instance.inventory[hotkeyIndex]==null || InventoryManager.Instance.inventory[hotkeyIndex].itemData ==null))
        {
            hotkeyIndex = -1;
            InventoryManager.Instance.equippedItem = null;
            UnequipTool();
        }

        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ProcessHotKey(i);
                break;
            }
        }
    }

    void ProcessHotKey(int slotIndex)
    {
        if (!pv.IsMine) return;
        InventoryItem hotkeyItem = InventoryManager.Instance.inventory[slotIndex];
        if (hotkeyItem != null && hotkeyItem.itemData != null && hotkeyItem.itemData.itemType == ItemType.Tool)
        {
            EquipTool(hotkeyItem);
            InventoryManager.Instance.EquipItem(slotIndex);
            hotkeyIndex = slotIndex;
        }
        else if (hotkeyItem != null && hotkeyItem.itemData != null && hotkeyItem.itemData.itemType == ItemType.Consumable && hotkeyItem.itemData.itemName == "MeatGrilled")
        {
            InventoryItem usingItem = new InventoryItem(hotkeyItem.itemData, 1);
            if (InventoryManager.Instance.UseConsumableItem(usingItem))
            {
                UserStatusManager.Instance.Hungry -= 20.0f;
                playerAudio.PlaySoundByName("eatSound");
            }
        }
        else
        {
            UnequipTool();
            hotkeyIndex = -1;
            InventoryManager.Instance.UnequipItem();
        }
    }

    public void UnequipTool()
    {
        pv.RPC("RpcUnequipTool", RpcTarget.All);
    }

    void EquipTool(InventoryItem toolItem)
    {
        UnequipTool();
        pv.RPC("RpcEquipTool", RpcTarget.All, toolItem.itemData.itemName);
    }

    [PunRPC]
    void RpcEquipTool(string toolName)
    {
        GameObject toolToEquip = hand.transform.Find(toolName)?.gameObject;
        if (toolToEquip != null)
        {
            toolToEquip.SetActive(true);
            playerAudio.PlaySoundByName("equipSound");
            anim.SetTrigger("Belt");
        }
        else
        {
            Debug.Log("프리펩 없음");
        }
    }

    [PunRPC]
    void RpcUnequipTool()
    {
        foreach (Transform child in hand.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
