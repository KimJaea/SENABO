using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGestureManager : MonoBehaviour
{

    [SerializeField]
    private StrollEventManager strollEventManager;

    private const float shakeThreshold = 2.0f; // ��鸲 ����
    private const float shakeInterval = 0.5f; // �� ��° ��鸲 ����
    private float lastShakeTime = 0f;
    private int shakeCount = 0;

    void Update()
    {
        // ����Ʈ���� ���ӵ��� ������ ��������
        Vector3 acceleration = Input.acceleration;

        // ��鸲�� X, Y, Z �� ������ ���� ������ ����
        float shakeMagnitude = acceleration.sqrMagnitude;

        // ��鸲 ����
        if (shakeMagnitude > shakeThreshold && Time.time - lastShakeTime > shakeInterval)
        {
            shakeCount++;
            lastShakeTime = Time.time;

            // �� ��° ��鸲 ���� �� �̺�Ʈ ����
            if (shakeCount == 2)
            {
                // �� �� ��鸲�� ���� �̺�Ʈ ȣ��
                strollEventManager.updateGestureEventTrigger();

                // �ʱ�ȭ
                shakeCount = 0;
                lastShakeTime = 0;
            }
        }
    }
}
