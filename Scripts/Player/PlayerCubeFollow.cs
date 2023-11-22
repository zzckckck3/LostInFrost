using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCubeFollow : MonoBehaviour
{
    [SerializeField]
    private PhotonView pv;
    [SerializeField]
    private Material isMine;
    
    private void Start()
    {
        if (pv.IsMine)
        {
            this.GetComponent<Renderer>().material = isMine;
        }
    }
    void Update()
    {
        this.transform.position = new Vector3(this.transform.parent.position.x, this.transform.position.y, this.transform.parent.position.z);
    }
}
