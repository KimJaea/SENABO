using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DogManager : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject myDog;
    List<ARRaycastHit> hits;
    float stoppingDistance = 0.6f;

    [SerializeField]
    GameObject dogAnimationManager;

    DogAnimationManager dogAnimator;

    Vector3 screenCenter; // 카메라 중앙 위치

    private bool strollEventCheck;
    public void updateStrollEventCheck(bool check)
    {
        strollEventCheck = check;
    }

    // Start is called before the first frame update
    void Start()
    {
        dogAnimator = dogAnimationManager.GetComponent<DogAnimationManager>();
        hits = new List<ARRaycastHit>();
        screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));   // 카메라의 중앙으로 ray를 쏜다
    }

    // Update is called once per frame
    void Update()
    {
        // 돌발 이벤트 발생 중일 때(아래 실행 x)
        if (strollEventCheck)
        {
            myDog.GetComponent<Rigidbody>().velocity = Vector3.zero; // 속도 중지
            myDog.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // 회전 중지
            return;
        }

        arRaycaster.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon); // 평면을 인식하여 hits에 값을 입력

        // 평면을 인식 했을 때
        if (hits.Count > 0)
        {
            Pose hitPose = hits[0].pose;

            // 이동 방향 설정
            Vector3 moveDirection = hitPose.position - myDog.transform.position;
            //moveDirection.y = 0; // 원하는 축으로 이동하도록 y 값을 0으로 설정

            // 이동하는 경우(이동 거리가 정한 값보다 클 경우)
            if (moveDirection.magnitude > stoppingDistance) // magnitude는 3차원 백터의 크기/길이
            {
                // 이동 방향으로 회전
                Quaternion newRotation = Quaternion.LookRotation(moveDirection);
                myDog.GetComponent<Rigidbody>().MoveRotation(newRotation);

                // 거리가 멀 때
                if (moveDirection.magnitude > 1.4f)
                {
                    myDog.GetComponent<Rigidbody>().velocity = Vector3.zero; // 속도 중지
                    if (!strollEventCheck)
                    {
                        dogAnimator.handleDogMovement("WelshRun");
                        myDog.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 100f);
                    }
                }
                // 거리가 짧을 때
                else
                {
                    if (!strollEventCheck)
                    {
                        dogAnimator.handleDogMovement("WelshWalk");
                        myDog.GetComponent<Rigidbody>().AddForce(moveDirection.normalized * 2f);
                    }
                }
            }
            // 이동안하는 경우(너무 짧은 거리는 이동 X)
            else
            {
                if (!strollEventCheck)
                {
                    // 멈출 거리에 도달한 경우
                    dogAnimator.handleDogMovement("WelshIdle");
                    myDog.GetComponent<Rigidbody>().velocity = Vector3.zero; // 속도 중지
                    myDog.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // 회전 중지
                }
            }
        }
        // 평면을 인식 못 했을 때
        else
        {
            if (!strollEventCheck)
            {
                dogAnimator.handleDogMovement("WelshIdle");
                myDog.GetComponent<Rigidbody>().velocity = Vector3.zero; // 속도 중지
                myDog.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // 회전 중지
            }
        }

    }
}
