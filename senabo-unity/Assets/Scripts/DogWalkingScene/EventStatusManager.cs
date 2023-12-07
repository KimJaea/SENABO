using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventStatusManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static bool isDogEventOn;

    private static bool isDogStopResolved;
   // private static bool isDogPoopResolved; // ARObjectController에서 처리

    private static int currentStress = 0;

    // 돌발 이벤트 처리 여부 확인 변수
    private bool encounterEventResolveCheck, eatEventResolveCheck, stopEventResolveCheck, poopEventResolveCheck;

    void Start()
    {
        isDogEventOn = false;
        isDogStopResolved = true;
     //   isDogPoopResolved = true;   
    }

    //돌발 상황 발생 시점에 Void Update로 isDogEventOn 출력시 True로 정상 출력됨.
    public static void SwitchDogEvent(bool eventStatus)
    {
        isDogEventOn = eventStatus;
    }
    public static bool GetDogEvent()
    {
        return isDogEventOn;
    }

    public static void IncreaseStress()
    {
        currentStress++;
        Debug.Log("stress : " + currentStress);
    }


    public static void SwitchDogStopResolved(bool eventStatus)
    {
        isDogStopResolved = eventStatus;
    }

    //public static void SwitchDogPoopResolved(bool eventStatus)
    //{
        //isDogPoopResolved = eventStatus;
    //}


    public static bool GetDogStopResolved()
    {
        return isDogStopResolved;
    }

    //public static bool GetDogPoopResolved()
    //{
    //  return isDogPoopResolved;
    //}


    // ====== 산책중 돌발 이벤트 대처에 성공했을 경우 호출하는 메서드 목록 ======
    public void updateEncounterEventResolveCheck()
    {
        Debug.Log("이벤트 처리 완료1");
        this.encounterEventResolveCheck = true;
    }
    public void updateEatEventResolveCheck()
    {
        Debug.Log("이벤트 처리 완료2");
        this.eatEventResolveCheck = true;
    }
    public void updateStopEventResolveCheck()
    {
        Debug.Log("이벤트 처리 완료3");
        this.stopEventResolveCheck = true;
    }
    public void updatePoopEventResolveCheck()
    {
        Debug.Log("이벤트 처리 완료4");
        this.poopEventResolveCheck = true;
    }

    // ===== 산책중 돌발 이벤트 대처 여부를 반환하는 메서드 ===== 
    public bool getEncounterEventResolveCheck()
    {
        return this.encounterEventResolveCheck;
    }
    public bool getEatEventResolveCheck()
    {
        return this.eatEventResolveCheck;
    }
    public bool getStopEventResolveCheck()
    {
        return this.stopEventResolveCheck;
    }
    public bool getPoopEventResolveCheck()
    {
        return this.poopEventResolveCheck;
    }


}
