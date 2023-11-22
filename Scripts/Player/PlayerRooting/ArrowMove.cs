using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    public float bobbingSpeed = 1f; // 진자운동 속도
    public float bobbingAmount = 0.5f; // 위아래로 움직이는 양

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float verticalMovement = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = initialPosition + new Vector3(0, verticalMovement, 0);
    }
}
