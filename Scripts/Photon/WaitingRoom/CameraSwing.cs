using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwing : MonoBehaviour
{
    public static CameraSwing instance = null;
    public static CameraSwing Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int flag = 0;
    public float shakeMagnitude = 0.5f;

    private Transform originalTransform;
    void Start()
    {
        originalTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(flag == 1)
        {
            float x = originalTransform.position.x + Random.Range(-1f, 1f) * shakeMagnitude;
            float y = originalTransform.position.y + Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(x, y, originalTransform.position.z);
        }
    }
}
