using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public RawImage mapRawImage;

    public string baseURL = "https://maps.googleapis.com/maps/api/staticmap?";
    public int zoom = 20;
   
    public int mapWidth = 400;
    public int mapHeight = 400;
    public string APIKey = "";

    [SerializeField]
    private GameObject LocationManagerObject;
    private LocationManager locationManager;

    private double latitude, longitude;

    private void Start()
    {
        mapRawImage = GetComponent<RawImage>();
        locationManager = LocationManagerObject.GetComponent<LocationManager>();

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
        FirebaseAuthManager.signUpRequestDto.houseLatitude = latitude;
        FirebaseAuthManager.signUpRequestDto.houseLongitude = longitude;

        SceneManager.LoadScene("ProfileSettingScene");
    }
}
