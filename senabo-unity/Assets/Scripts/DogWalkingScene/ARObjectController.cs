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


    // ȭ�鿡 �ٸ� �������� �����ϴ� �̺�Ʈ �߻��� Ȯ���ϴ� ����
    private bool dogEventTrigger;
    public void setDogEventTrigger()
    {
        dogEventTrigger = true;
    }
    // �躯 ���� ��ư�� Ŭ���ߴ��� Ȯ���ϴ� ����
    private bool poopEventTrigger;
    public void setPoopEventTrigger()
    {
        poopEventTrigger = true;
    }

    private void Start()
    {
        ummScript = UIModalManager.GetComponent<UIModalManager>();
        // Gps�� ���� ���� ���
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS�� Ȱ��ȭ���ּ���.");
            // GPS Ȱ��ȭ ��û
        }
    }

    // Update is called once per frame
    void Update()
    {
        // myDog�� otherDog�� ��� ������ ���
        if (myDog.activeInHierarchy && otherDog.activeInHierarchy)
        {
            // myDog�� otherDog ���� �Ÿ��� ���
            float distance = Vector3.Distance(myDog.transform.position, otherDog.transform.position);

            // �Ÿ��� 3m �̻��� ���
            if (distance >= 2.5f)
            {
                Debug.Log("myDog�� otherDog ���� �Ÿ��� 2m �̻��Դϴ�.");
                otherDog.SetActive(false);
                StrollEventManager var = strollEventManager.GetComponent<StrollEventManager>();
                var.updateDistanceEventTrigger();
            }
        }

        // otherDog�� �����Ǵ� �̺�Ʈ�� �߻��� ���
        if (dogEventTrigger)
        {
            // ======= otherDog�� �����Ͽ� myDog �տ� ���ֺ����� ��ġ ���� ========== //
            Vector3 offset = myDog.transform.forward * 2.0f; 

            otherDog.SetActive(true);
            otherDog.transform.position = myDog.transform.position + offset;
            float rotationSpeed = 180f;
            otherDog.transform.rotation = Quaternion.Euler(0, rotationSpeed += Time.deltaTime * 10, 0);
            dogEventTrigger = false;
        }

        // ��ġ�� ���� ��� �Ʒ� ���� x
        if (Input.touchCount==0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        Vector2 touchPosition = touch.position;

        // �躯 ������ Ŭ���� ���� + poop�� �ִ� ������ ���
        if (poopEventTrigger && poop.activeInHierarchy)
        {
            if (touch.phase == TouchPhase.Began && arRaycaster.Raycast(touchPosition, arHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = arHits[0].pose;
                float distance = Vector3.Distance(poop.transform.position, hitPose.position);
                if (distance <= 0.3f)
                {
                    Debug.Log("�躯 ����!");
                    poopEventTrigger = false;
                    poop.SetActive(false);
                    eventStatusManager.updatePoopEventResolveCheck();
                }
            }
        }
        // myDog�� ��Ȱ�� ������ ���
        if (!myDog.activeInHierarchy)
        {
            ummScript.CloseModal(ummScript.StartTip);

            // ó�� Ŭ���� ������ ���� RayCast�� ��� �ϰ�, ����� �ν����� ���
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