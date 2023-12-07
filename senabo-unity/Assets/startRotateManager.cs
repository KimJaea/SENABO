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
            Debug.Log("gyro2 ����");
            gyro.enabled = false;
            // ���� myDog�� Ȱ��ȭ�Ǿ� ������ �ش� ��ũ��Ʈ�� ��Ȱ��ȭ
            this.enabled = false;
            return;
        }
        Quaternion gyroRotation = gyro.attitude;
        Debug.Log("gyro2: "+gyro.attitude);
        // y�� ȸ�� Ȯ��
        float yRotation = gyroRotation.eulerAngles.y;
        myDog.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
