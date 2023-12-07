using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARObjectController : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject UIModalManager;
    public GameObject gpsManager;
    public GameObject dogLeadSpawner;
    public GameObject walkTimer;
    public GameObject dogRotator;
    public GameObject strollEventManager;
    public GameObject dogAnimationManager;
    public GameObject dogManager;
    public GameObject itemSpawner;

    [SerializeField]
    private GameObject myDog;

    [SerializeField]
    private GameObject otherDog;

    [SerializeField]
    private GameObject poop;

    [SerializeField]
    private EventStatusManager eventStatusManager;

    private static List<ARRaycastHit> arHits = new List<ARRaycastHit>();
    private UIModalManager ummScript;


    // 화면에 다른 강아지가 등장하는 이벤트 발생을 확인하는 변수
    private bool dogEventTrigger;
    public void setDogEventTrigger()
    {
        dogEventTrigger = true;
    }
    // 배변 봉투 버튼을 클릭했는지 확인하는 변수
    private bool poopEventTrigger;
    public void setPoopEventTrigger()
    {
        poopEventTrigger = true;
    }

    private void Start()
    {
        ummScript = UIModalManager.GetComponent<UIModalManager>();
        // Gps가 꺼져 있을 경우
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS를 활성화해주세요.");
            // GPS 활성화 요청
        }
    }

    // Update is called once per frame
    void Update()
    {
        // myDog와 otherDog가 모두 생성된 경우
        if (myDog.activeInHierarchy && otherDog.activeInHierarchy)
        {
            // myDog와 otherDog 간의 거리를 계산
            float distance = Vector3.Distance(myDog.transform.position, otherDog.transform.position);

            // 거리가 3m 이상인 경우
            if (distance >= 2.5f)
            {
                Debug.Log("myDog와 otherDog 간의 거리가 2m 이상입니다.");
                otherDog.SetActive(false);
                StrollEventManager var = strollEventManager.GetComponent<StrollEventManager>();
                var.updateDistanceEventTrigger();
            }
        }

        // otherDog가 생성되는 이벤트가 발생한 경우
        if (dogEventTrigger)
        {
            // ======= otherDog를 생성하여 myDog 앞에 마주보도록 위치 설정 ========== //
            Vector3 offset = myDog.transform.forward * 2.0f; 

            otherDog.SetActive(true);
            otherDog.transform.position = myDog.transform.position + offset;
            float rotationSpeed = 180f;
            otherDog.transform.rotation = Quaternion.Euler(0, rotationSpeed += Time.deltaTime * 10, 0);
            dogEventTrigger = false;
        }

        // 터치가 없는 경우 아래 실행 x
        if (Input.touchCount==0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        Vector2 touchPosition = touch.position;

        // 배변 봉투를 클릭한 상태 + poop이 있는 상태인 경우
        if (poopEventTrigger && poop.activeInHierarchy)
        {
            if (touch.phase == TouchPhase.Began && arRaycaster.Raycast(touchPosition, arHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = arHits[0].pose;
                float distance = Vector3.Distance(poop.transform.position, hitPose.position);
                if (distance <= 0.3f)
                {
                    Debug.Log("배변 제거!");
                    poopEventTrigger = false;
                    poop.SetActive(false);
                    eventStatusManager.updatePoopEventResolveCheck();
                }
            }
        }
        // myDog가 비활성 상태인 경우
        if (!myDog.activeInHierarchy)
        {
            ummScript.CloseModal(ummScript.StartTip);

            // 처음 클릭한 상태일 때만 RayCast를 쏘도록 하고, 평면을 인식했을 경우
            if (touch.phase == TouchPhase.Began && arRaycaster.Raycast(touchPosition, arHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = arHits[0].pose;
                myDog.SetActive(true); 

                myDog.transform.position = hitPose.position;

               // dogRotator.SetActive(true);
                dogLeadSpawner.SetActive(true);
                walkTimer.SetActive(true);
                dogAnimationManager.SetActive(true);
                strollEventManager.SetActive(true);
                gpsManager.SetActive(true);
                dogManager.SetActive(true);
                itemSpawner.SetActive(true);
            }
        }
    }
}