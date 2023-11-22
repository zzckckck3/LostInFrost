using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCosChar : MonoBehaviour
{
    public GameObject characterUi;
    public GameObject itemBoxUi;
    public void SetOffCharacter()
    {
        characterUi.SetActive(false);
    }
    public void SetOffItemBox()
    {
        itemBoxUi.SetActive(false);
    }
    public void SetOnItemBox()
    {
        itemBoxUi.SetActive(true);
    }
}
