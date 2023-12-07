using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalkTimer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Text totalWalkTime;
    
    public GameObject dogObject;

    private int minuteInNumber = 0;
    private float elapsedTime = 0f;
    public int getTotalStrollMinute()
    {
        return minuteInNumber;
    }

    IEnumerator Start()
    {
        totalWalkTime.text = "출발 전";

        yield return new WaitUntil(() => dogObject.activeInHierarchy);

        StartCoroutine(UpdateElapsedTime());
    }

    IEnumerator UpdateElapsedTime()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;
            minuteInNumber++;

            //totalWalkTime.text = String.Join("", elapsedTime.ToString("F2"),"분");
            totalWalkTime.text = String.Join("", minuteInNumber.ToString("D2"), "분");

            yield return new WaitForSeconds(60);
        }
    }
}
