using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGoods : MonoBehaviour
{
    public void OnClickUpdateGoods()
    {
        AuthManager.Instance.ReqUpdateUserGoods();
    }
}
