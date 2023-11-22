using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeRPC : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public GameObject costumeList;
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            FindMyCostume();
        }
    }

    public void FindMyCostume()
    {
        string myCostumeName = AuthManager.Instance.userData.costumeName;

        if (PV.IsMine)
        {
            PV.RPC("myCostume", RpcTarget.AllBuffered, myCostumeName);
        }
    }

    [PunRPC]
    public void myCostume(string myCostumeName)
    {
        int numOfChild = costumeList.transform.childCount;
        for(int i = 0; i < numOfChild; i++)
        {
            if (!costumeList.transform.GetChild(i).gameObject.name.Equals(myCostumeName))
            {
                costumeList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        GameObject activeNowCostume = costumeList.transform.Find(myCostumeName).gameObject;
        activeNowCostume.SetActive(true);
    }
}
