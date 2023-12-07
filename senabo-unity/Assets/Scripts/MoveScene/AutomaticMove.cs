using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomaticMove : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float movementSpeed = 2.0f;

    private float journeyLength;
    private float startTime;
    private readonly float limitTime = 3.0f;

    void Start()
    {
        StartCoroutine(LoadScene());

        journeyLength = Vector3.Distance(startPoint.position, endPoint.position);
        startTime = Time.time;
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync("ReceiptScene");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            yield return null;
            DogMove();

            if(Time.time - startTime > limitTime && operation.progress >= 0.9f) {
                operation.allowSceneActivation = true;
            }
        }

    }

    void DogMove()
    {
        float distanceCovered = (Time.time - startTime) * movementSpeed;

        if (distanceCovered < journeyLength)
        {
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPoint.position, endPoint.position, fractionOfJourney);
        }
        else
        {
            transform.position = startPoint.position;
            // startTime = Time.time;
        }
    }
}
