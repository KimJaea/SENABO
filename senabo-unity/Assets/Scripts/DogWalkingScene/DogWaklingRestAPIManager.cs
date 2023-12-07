using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DogWalkingRestAPIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gpsManager;
    private double distance;

    static public WalkEndRequestDtoClass walkEndRequestDto;

    public void HandleWalkEnd()
    {
        if (!gpsManager.activeInHierarchy)
        {
            Debug.Log("��å ���۵��� ����!");
            return;
        }
        StartCoroutine(SendWalkEndInfo());
    }

    IEnumerator SendWalkEndInfo()
    {

        GpsManager gpsManagerScript = gpsManager.GetComponent<GpsManager>();
        walkEndRequestDto = new WalkEndRequestDtoClass();
        walkEndRequestDto.distance = gpsManagerScript.getUserMovementDistance();
        string jsonFile = JsonUtility.ToJson(walkEndRequestDto);
        string url = $"{ServerSettings.SERVER_URL}/api/walk/end";

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";

            request.SetRequestHeader("Authorization", jwtToken);

            yield return request.SendWebRequest();

            if (request.error == null)
            {
                Debug.Log(request.downloadHandler.text);
                SceneManager.LoadScene("WalkResultScene");
                yield break;
            }
            else
            {
                Debug.Log("��å ���� ���� ���� ����");
                Debug.LogError(request.error.ToString());
            }
        }
    }

}
