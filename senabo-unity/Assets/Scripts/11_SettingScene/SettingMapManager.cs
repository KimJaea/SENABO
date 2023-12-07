using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingMapManager : MonoBehaviour
{
    public GameObject ChangeLocationModalPanel;
    public GameObject SuccessChangeLocationAlertPanel;
    public GameObject FailChangeLocationAlertPanel;
    public RawImage mapRawImage;

    public string baseURL = "https://maps.googleapis.com/maps/api/staticmap?";
    public int zoom = 20;
   
    public int mapWidth = 400;
    public int mapHeight = 400;
    public string APIKey = "";

    [SerializeField]
    private GameObject LocationManagerObject;
    private SettingLocationManager locationManager;

    private double latitude, longitude;

    private void Start()
    {
        mapRawImage = GetComponent<RawImage>();
        locationManager = LocationManagerObject.GetComponent<SettingLocationManager>();

        StartCoroutine(UpdatePosition());
    }

    IEnumerator UpdatePosition()
    {
        LocationManagerObject.SetActive(true);

        while (true)
        {
            if (locationManager.getIsLocationManagerFinished())
            {
                latitude = locationManager.GetLatitude();
                longitude = locationManager.GetLongitude();

                Debug.Log("위도:" + latitude + "\n경도:" + longitude);

                StartCoroutine(LoadMap());

                LocationManagerObject.SetActive(false);
                yield break;
            }

            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator LoadMap()
    {
        string url = baseURL 
            + "center=" + latitude + "," + longitude 
            + "&zoom=" + zoom.ToString()
            + "&size=" + mapWidth.ToString() + "x" + mapHeight.ToString()
            + "&markers=color:blue%7Clabel:S%7C" + latitude + "," + longitude
            + "&key=" + APIKey;

        Debug.Log("URL=" + url);

        url = UnityWebRequest.UnEscapeURL(url);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        mapRawImage.texture = DownloadHandlerTexture.GetContent(request);
    }

    public void SetHouseLocation()
    {
        Debug.Log("새로운 위치 설정");

        StartCoroutine(PutLocation());
    }

    private void CloseSuccessChangeLocationAlertPanel()
    {
        SuccessChangeLocationAlertPanel.SetActive(false);
        ChangeLocationModalPanel.SetActive(false);
    }

    private void CloseFailChangeLocationAlertPanel()
    {
        FailChangeLocationAlertPanel.SetActive(false);
    }

    IEnumerator PutLocation()
    {
        UpdateLocationRequestDtoClass updateLocationRequestDto = new UpdateLocationRequestDtoClass();
        updateLocationRequestDto.houseLatitude = latitude;
        updateLocationRequestDto.houseLongitude = longitude;

        string jsonFile = JsonUtility.ToJson(updateLocationRequestDto);
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/locate";

        using (UnityWebRequest request = UnityWebRequest.Put(api_url, jsonFile))
        {
            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";
            request.SetRequestHeader("Authorization", jwtToken);

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.error == null)
            {
                Debug.Log(request.downloadHandler.text);

                MemberGetResponseClass memberGetResponseClass =
                    JsonUtility.FromJson<APIResponse<MemberGetResponseClass>>(request.downloadHandler.text).data;

                PlayerPrefs.SetFloat("houseLatitude", (float)memberGetResponseClass.houseLatitude);
                PlayerPrefs.SetFloat("houseLongitude", (float)memberGetResponseClass.houseLongitude);

                SuccessChangeLocationAlertPanel.SetActive(true);
                Invoke("CloseSuccessChangeLocationAlertPanel", 2.0f);
            }
            else
            {
                Debug.Log("사용자 주소 갱신 실패");

                if (request.responseCode == 403)
                {
                    RefreshTokenManager.Instance.ReIssueRefreshToken();

                    StartCoroutine(PutLocation());
                }
                else
                {
                    FailChangeLocationAlertPanel.SetActive(true);
                    Invoke("CloseFailChangeLocationAlertPanel", 2.0f);
                    SceneManager.LoadScene("MainScene");
                }
            }
        }
    }
}
