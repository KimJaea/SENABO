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
   // private static bool isDogPoopResolved; // ARObjectController���� ó��

    private static int currentStress = 0;

    // ���� �̺�Ʈ ó�� ���� Ȯ�� ����
    private bool encounterEventResolveCheck, eatEventResolveCheck, stopEventResolveCheck, poopEventResolveCheck;

    void Start()
    {
        isDogEventOn = false;
        isDogStopResolved = true;
     //   isDogPoopResolved = true;   
    }

    //���� ��Ȳ �߻� ������ Void Update�� isDogEventOn ��½� True�� ���� ��µ�.
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


    // ====== ��å�� ���� �̺�Ʈ ��ó�� �������� ��� ȣ���ϴ� �޼��� ��� ======
    public void updateEncounterEventResolveCheck()
    {
        Debug.Log("�̺�Ʈ ó�� �Ϸ�1");
        this.encounterEventResolveCheck = true;
    }
    public void updateEatEventResolveCheck()
    {
        Debug.Log("�̺�Ʈ ó�� �Ϸ�2");
        this.eatEventResolveCheck = true;
    }
    public void updateStopEventResolveCheck()
    {
        Debug.Log("�̺�Ʈ ó�� �Ϸ�3");
        this.stopEventResolveCheck = true;
    }
    public void updatePoopEventResolveCheck()
    {
        Debug.Log("�̺�Ʈ ó�� �Ϸ�4");
        this.poopEventResolveCheck = true;
    }

    // ===== ��å�� ���� �̺�Ʈ ��ó ���θ� ��ȯ�ϴ� �޼��� ===== 
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
