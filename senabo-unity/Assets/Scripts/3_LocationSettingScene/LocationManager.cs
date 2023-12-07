using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class LocationManager : MonoBehaviour
{
    private double curlatitude,curlongitude;

    public double GetLatitude()
    {
        return curlatitude;
    }

    public double GetLongitude()
    {
        return curlongitude;
    }

    // 해당 오브젝트의 작업 완료 여부
    // true일 경우 작업 완료, false 일 경우 작업 완료 x
    private bool isLocationManagerFinished;
    public bool getIsLocationManagerFinished()
    {
        return this.isLocationManagerFinished;
    }

    public void OnEnable()
    {
        curlatitude = 0; curlongitude = 0;
        StartCoroutine(GetLocation());
    }

    IEnumerator GetLocation()
    {
        Debug.Log("LocationManager start");
        // Location Permission 요청
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }

        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
            yield break;
        }

        // Starts the location service.
        Input.location.Start(1, 1); // 정확도, 업데이트 거리

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Check for service initialization timeout
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Check for service connection failure
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, retrieves the device's current location and displays it in the Console window.
            curlatitude = Input.location.lastData.latitude;
            curlongitude = Input.location.lastData.longitude;
            Debug.Log("집 GPSPosition1: " + curlatitude + ", " + curlongitude);

            while (curlatitude == 0 || curlongitude == 0)
            {
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    curlatitude = Input.location.lastData.latitude;
                    curlongitude = Input.location.lastData.longitude;
                }
                Debug.Log("집 GPSPosition2: " + curlongitude + ", " + curlatitude);
                yield return new WaitForSeconds(1);
            }

            isLocationManagerFinished = true;
        }
    }
}
