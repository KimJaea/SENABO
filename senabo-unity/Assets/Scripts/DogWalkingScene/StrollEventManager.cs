using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static ItemSpawner;

public class StrollEventManager : MonoBehaviour
{
    private int[] randomTimes;

    [SerializeField]
    private ARObjectController arObjectController;

    [SerializeField]
    private DogManager dogManager;

    [SerializeField]
    private EventStatusManager eventStatusManager;

    [SerializeField]
    private GameObject dogObject;

    [SerializeField]
    GameObject dogAnimationManager;

    [SerializeField]
    GameObject itemSpawner;

    DogAnimationManager dogAnimator;

    [SerializeField]
    private GameObject userGestureManager;

    ItemSpawner itemSpawnerScript;


    private bool gestureEventTrigger;   // 사용자의 당김 동작 수행 체크
    private bool distanceEventTrigger;  // myDog와 otherDog의 거리 체크(멀리 떨어졌을 경우 true)
    public void updateGestureEventTrigger(){
        this.gestureEventTrigger = true;
    }
    public void updateDistanceEventTrigger()

    {
        this.distanceEventTrigger = true;
    }

    // Start is called before the first frame update
    void Start()
    {

        dogAnimator = dogAnimationManager.GetComponent<DogAnimationManager>(); 
        if (dogAnimator == null)
        {
            Debug.LogError("DogAnimationManager component not found on dogAnimationManager GameObject!");
        }

        randomTimes = new int[4];

        // 1분부터 60분까지 랜덤한 값 4개를 randomTimes 배열에 입력
        for (int i = 0; i < randomTimes.Length; i++)
        {
            randomTimes[i] = UnityEngine.Random.Range(1, 31); // 1부터 60까지의 랜덤한 값 생성
        }

        StartCoroutine(SuddenEat(16));
    }


    // ================돌발 이벤트 발생 함수================
    // 1. 다른 강아지를 만났을 때
    IEnumerator SuddenEncounter(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        dogManager.updateStrollEventCheck(true);    // 강아지 움직임 제어
        EventStatusManager.SwitchDogEvent(true);    // 애니메이션 고정
        
        arObjectController.setDogEventTrigger();    // otherDog가 나오게 설정
        userGestureManager.SetActive(true);         // 손동작 인식 on

        // 진동 알림
        for (int i = 0; i < 40; i++)
        {
            // 일정 거리 이상 떨어진 경우
            if (distanceEventTrigger)
            {
                distanceEventTrigger = false;
                eventStatusManager.updateEncounterEventResolveCheck();  // 돌발행동 대처 성공 데이터 처리
                break;
            }

            // 사용자가 핸드폰을 n번 당겼을 경우
            if (gestureEventTrigger)
            {
                EventStatusManager.SwitchDogEvent(false); // 애니메이션이 변경될 수 있게 설정
                dogManager.updateStrollEventCheck(false); // 강아지가 움직일 수 있게 설정
                userGestureManager.SetActive(false);      // 사용자 손동작 인식 off
                gestureEventTrigger = false;
            }

            dogAnimator.handleDogSuddenEvent("WelshBark");
            Handheld.Vibrate(); // 0.5초간 진동이 울림
            yield return new WaitForSeconds(1); 
        }

        yield return StartCoroutine(SuddenStop(6)); ;

        gestureEventTrigger = false;
        dogManager.updateStrollEventCheck(false);
        EventStatusManager.SwitchDogEvent(false);
        userGestureManager.SetActive(false);
        // 돌발행동 대처 실패 데이터 처리
        EventStatusManager.IncreaseStress();
    }


    // 2, 땅에 떨어진 이물질을 주워 먹으려 할 때
    IEnumerator SuddenEat(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        dogManager.updateStrollEventCheck(true);        // 강아지 움직임 제어
        EventStatusManager.SwitchDogEvent(true);        // 애니메이션 고정
        userGestureManager.SetActive(true);             // 손동작 인식 on

        // 진동 알림
        for (int i = 0; i < 30; i++)
        {
            // 사용자가 핸드폰을 n번 당겼을 경우
            if (gestureEventTrigger)
            {
                gestureEventTrigger = false;
                dogManager.updateStrollEventCheck(false);  // 강아지가 움직일 수 있게 설정
                EventStatusManager.SwitchDogEvent(false);  // 애니메이션이 변경될 수 있게 설정
                userGestureManager.SetActive(false);       // 사용자 손동작 인식 off
                eventStatusManager.updateEatEventResolveCheck(); // 돌발행동 대처 성공 데이터 처리
                break;
            }
            dogAnimator.handleDogSuddenEvent("WelshEat");
            Handheld.Vibrate(); // 0.5초간 진동이 울림
            yield return new WaitForSeconds(1);
        }

        yield return StartCoroutine(SuddenPoop(8));

        gestureEventTrigger = false;
        dogManager.updateStrollEventCheck(false);
        EventStatusManager.SwitchDogEvent(false);
        userGestureManager.SetActive(false);
        // 돌발행동 대처 실패 데이터 처리
        EventStatusManager.IncreaseStress();
    }


    // 3. 주저 앉아서 움직이지 않으려 할 때
    IEnumerator SuddenStop(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        dogManager.updateStrollEventCheck(true);
        EventStatusManager.SwitchDogEvent(true);
        EventStatusManager.SwitchDogStopResolved(false);
        if (itemSpawnerScript == null) itemSpawnerScript = itemSpawner.GetComponent<ItemSpawner>();

        // 진동 알림
        int cnt = 0;
        for (int i = 0; i < 30; i++)
        {
            // 간식을 주고 4초 됬을 때
            if (cnt == 4)
            {
                dogManager.updateStrollEventCheck(false);
                EventStatusManager.SwitchDogEvent(false);
                itemSpawnerScript.HandleRemoveAction(ItemType.Snack);
                eventStatusManager.updateStopEventResolveCheck(); // 돌발행동 대처 성공 데이터 처리
                break;
            }

            // 간식을 준 상태일 때
            if (EventStatusManager.GetDogStopResolved())
            {
                if (cnt == 0)
                {
                    dogAnimator.handleDogSuddenEvent("WelshIdle");
                }
                else
                {
                    dogAnimator.handleDogSuddenEvent("WelshEat");
                }
                cnt++;
            }
            else
            {
                dogAnimator.handleDogSuddenEvent("WelshSit");
            }

            Handheld.Vibrate();
            yield return new WaitForSeconds(1);
        }

        dogManager.updateStrollEventCheck(false);
        EventStatusManager.SwitchDogEvent(false);
        EventStatusManager.SwitchDogStopResolved(true);
        // 실패 처리
        EventStatusManager.IncreaseStress();
        yield break;
    }

    // 4. 배변 활동
    IEnumerator SuddenPoop(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        if(itemSpawnerScript == null) itemSpawnerScript = itemSpawner.GetComponent<ItemSpawner>();

        dogManager.updateStrollEventCheck(true);
        EventStatusManager.SwitchDogEvent(true);

        // 진동 알림
        for (int i = 0; i < 5; i++)
        {
            dogAnimator.handleDogSuddenEvent("WelshPoop");
            Handheld.Vibrate();
            yield return new WaitForSeconds(1);
        }

        itemSpawnerScript.HandleSpawnAction(ItemType.Poop); // Poop 생성
        dogManager.updateStrollEventCheck(false);
        EventStatusManager.SwitchDogEvent(false);
        yield return StartCoroutine(SuddenEncounter(16));
    }
}
