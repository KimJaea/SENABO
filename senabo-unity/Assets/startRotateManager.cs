using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startRotateManager : MonoBehaviour
{

    public GameObject myDog;
    private Gyroscope gyro;

    void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (myDog.activeInHierarchy)
        {
            Debug.Log("gyro2 종료");
            gyro.enabled = false;
            // 만약 myDog가 활성화되어 있으면 해당 스크립트를 비활성화
            this.enabled = false;
            return;
        }
        Quaternion gyroRotation = gyro.attitude;
        Debug.Log("gyro2: "+gyro.attitude);
        // y축 회전 확인
        float yRotation = gyroRotation.eulerAngles.y;
        myDog.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
