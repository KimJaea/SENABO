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

    // �ش� ������Ʈ�� �۾� �Ϸ� ����
    // true�� ��� �۾� �Ϸ�, false �� ��� �۾� �Ϸ� x
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
        // Location Permission ��û
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
        Input.location.Start(1, 1); // ��Ȯ��, ������Ʈ �Ÿ�

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
            Debug.Log("�� GPSPosition1: " + curlatitude + ", " + curlongitude);

            while (curlatitude == 0 || curlongitude == 0)
            {
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    curlatitude = Input.location.lastData.latitude;
                    curlongitude = Input.location.lastData.longitude;
                }
                Debug.Log("�� GPSPosition2: " + curlongitude + ", " + curlatitude);
                yield return new WaitForSeconds(1);
            }

            isLocationManagerFinished = true;
        }
    }
}
