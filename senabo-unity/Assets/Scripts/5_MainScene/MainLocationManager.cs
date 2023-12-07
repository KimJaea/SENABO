using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class MainLocationManager : MonoBehaviour
{
    private double curlatitude, curlongitude;
    private double homeLatitude, homeLongitude;

    // 집에 있는지 여부
    // true일 경우 집, false일 경우 집 x
    private bool isAtHome;
    public bool getIsAtHome()
    {
        return this.isAtHome;
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
        homeLatitude = PlayerPrefs.GetFloat("houseLatitude");
        homeLongitude = PlayerPrefs.GetFloat("houseLongitude");
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

            Debug.Log("집과의 거리: " + distance(curlatitude, curlongitude, homeLatitude, homeLongitude));

            // 집에 있는 경우
            if (distance(curlatitude, curlongitude, homeLatitude, homeLongitude) < 50f)
            {
                isAtHome = true;
                isLocationManagerFinished = true;
            }
            // 밖에 있는 경우
            else
            {
                isAtHome = false;
                isLocationManagerFinished = true;
            }
        }
    }

    // 하버싸인 공식(지표면 거리계산 공식)
    // 현재 위치의 위도, 경도 -> 목적지의 위도 경도
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        try
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2))
            + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2))
            * Math.Cos(Deg2Rad(theta));

            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1609.344; // 미터 변환
            return dist;
        }
        catch (Exception e)
        {
            // 분모가 0일 경우 방지
            Debug.Log("GPS 예외 발생");
            return -1; // 에러가 발생한 경우 -1 또는 다른 적절한 값 반환
        }

    }
    private double Deg2Rad(double deg)
    {
        return (deg * Math.PI / 180.0f);
    }
    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }


    private void OnDisable()
    {
        isAtHome = false;
        isLocationManagerFinished = false;
        Input.location.Stop();
    }
}
