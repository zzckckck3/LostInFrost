using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public void DifficultyPlus()
    {
        UserStatusManager.Instance.difficulty += 0.1f;
    }

    public void DifficultyMinus()
    {
        UserStatusManager.Instance.difficulty -= 0.1f;
        if (UserStatusManager.Instance.difficulty < 0.1f)
        {
            UserStatusManager.Instance.difficulty = 0.1f;
        }
    }

    public void StatusReset()
    {
        UserStatusManager.Instance.HP = 100.0f;
        UserStatusManager.Instance.Stamina = 0.0f;
        UserStatusManager.Instance.Hungry = 0.0f;
        UserStatusManager.Instance.Cold = 0.0f;
    }
}
