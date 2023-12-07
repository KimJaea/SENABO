using System;
using System.Collections;
using UnityEngine;

using UnityEngine.Android;
using UnityEngine.UI;

public class GpsManager : MonoBehaviour
{
    // ����, �浵(�̵� ��, �̵� ��)
    private double pastLat, pastLon, curLat, curLon;

    // ������� �̵� �Ÿ�
    public Text userMovementDistanceText;
    private double userMovementDistance;
    public double getUserMovementDistance()
    {
        return this.userMovementDistance;
    }

    IEnumerator Start()
    {

        // Location Permission�� ��û�ϴ� �ڵ�
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
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

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // �� �ʱ�ȭ
            userMovementDistance = 0;
            pastLat = Input.location.lastData.latitude;
            pastLon = Input.location.lastData.longitude;

            while (true)
            {
                // 3�� ���
                yield return new WaitForSeconds(3);
                Debug.Log("Gps �ݺ� ��");
                // GPS ���� �������� �Լ� ȣ��
                ContinuousGPSUpdates();
            }
        }
    }


    // ���� ������Ʈ�� ������� ����
    private void OnDestroy()
    {
        StopAllCoroutines();
        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }

    // 1�ʸ��� GPS �����͸� �޾Ƽ� ����� �̵� �Ÿ�, �ӵ� ��ȯ
    void ContinuousGPSUpdates()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            curLat = Input.location.lastData.latitude;
            curLon = Input.location.lastData.longitude;

            // �̵� ��, �� ��ǥ�� �Ÿ� ���
            double dist = distance(pastLat, pastLon, curLat, curLon);
            if (dist < 0) return;

            // ���� Ƣ�� ��� ����, ����� 1�ʿ� �̵� ������ �Ÿ� �� ���
            if (dist < 10)
            {
                userMovementDistance += dist;
                pastLat = curLat;
                pastLon = curLon;
            }
        }
        userMovementDistanceText.text = String.Join("", userMovementDistance.ToString("F2"), "km");
    }


    // �Ϲ����� ����(��ǥ�� �Ÿ���� ����)
    // ���� ��ġ�� ����, �浵 -> �������� ���� �浵
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
            dist = dist * 1609.344 / 1000; // ���� ��ȯ
            return dist;
        }
        catch (Exception e)
        {
            // �и� 0�� ��� ����
            Debug.Log("GPS ���� �߻�");
            return -1; // ������ �߻��� ��� -1 �Ǵ� �ٸ� ������ �� ��ȯ
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
}
