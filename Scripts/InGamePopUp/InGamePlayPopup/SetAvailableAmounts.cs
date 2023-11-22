using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetAvailableAmounts : MonoBehaviour
{
    [SerializeField]
    private GameObject amountsPlus;

    [SerializeField]
    private TextMeshProUGUI amounts;

    // Update is called once per frame
    void Update()
    {
        if(InGameManager.Instance.AvailableCards <= 0)
        {
            amountsPlus.SetActive(false);
        }
        else
        {
            amountsPlus.SetActive(true);
        }

        amounts.text = InGameManager.Instance.AvailableCards.ToString();
    }
}
