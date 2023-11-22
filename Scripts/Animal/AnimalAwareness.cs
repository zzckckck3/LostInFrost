using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAwareness : MonoBehaviour
{
    public PhotonAnimalAI animalAI;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pv.IsMine && animalAI.target == null && animalAI.CheckTargetType(other.gameObject))
        {
            pv.RPC("RPCSetTarget", RpcTarget.All, other.gameObject.GetComponent<PhotonView>().ViewID);
        }
    }
}
