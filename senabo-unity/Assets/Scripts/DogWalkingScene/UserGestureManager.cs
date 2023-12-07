using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGestureManager : MonoBehaviour
{

    [SerializeField]
    private StrollEventManager strollEventManager;

    private const float shakeThreshold = 2.0f; // 흔들림 감도
    private const float shakeInterval = 0.5f; // 두 번째 흔들림 간격
    private float lastShakeTime = 0f;
    private int shakeCount = 0;

    void Update()
    {
        // 스마트폰의 가속도계 데이터 가져오기
        Vector3 acceleration = Input.acceleration;

        // 흔들림을 X, Y, Z 축 각각에 대한 합으로 정의
        float shakeMagnitude = acceleration.sqrMagnitude;

        // 흔들림 감지
        if (shakeMagnitude > shakeThreshold && Time.time - lastShakeTime > shakeInterval)
        {
            shakeCount++;
            lastShakeTime = Time.time;

            // 두 번째 흔들림 감지 시 이벤트 실행
            if (shakeCount == 2)
            {
                // 두 번 흔들림에 대한 이벤트 호출
                strollEventManager.updateGestureEventTrigger();

                // 초기화
                shakeCount = 0;
                lastShakeTime = 0;
            }
        }
    }
}
