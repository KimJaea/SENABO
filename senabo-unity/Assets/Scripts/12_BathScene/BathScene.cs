using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
public class BathScene : MonoBehaviour
{
    public Text titleBarText;
    public GameObject bubblePrefab;
    public Transform bubbleParent;
    public GameObject[] bubbleArray;
    public GameObject titleBar, dogBodyObject, dogCleanFace, dogDirtyFace, BathTub, YellowShine, WhiteShine;
    public GameObject CantBathAlertPanel, HowToBathPanel1, HowToBathPanel2, HowToBathPanel3, CantTeethAlertPanel, BrushTeethPanel1, BrushTeethPanel2;
    public Image background;
    public Sprite wetImage, dogClean, dogDirty, teethClean, teethDirty;
    private bool bathPossible, brushTeethPossible; // checking status
    private int selectType; // 1: Bath, 2: Teeth
    private int dogCount = 0, dogLimit = 10, dogState = 0;
    private Button dogBody, dogFace;

    void Start()
    {
        titleBar.SetActive(false);
        dogBodyObject.SetActive(false); dogCleanFace.SetActive(false); dogDirtyFace.SetActive(false);
        YellowShine.SetActive(false); WhiteShine.SetActive(false);
        CloseAllAlert();
        bathPossible = false; brushTeethPossible = false;
        selectType = 0; dogCount = 0; dogState = 0;

        bubbleArray = new GameObject[dogLimit];

        StartCoroutine(CheckBathTime());
        StartCoroutine(CheckTeethPossible());
    }

    public void setSelectType(int type)
    {
        selectType = type;

        if (type == 1)
        {
            titleBarText.text = "목욕";
            dogBodyObject.SetActive(true);

            if (bathPossible)
            {
                dogBody = BathTub.GetComponent<Button>();
                dogBody.onClick.AddListener(OnClickDogBody);

                Image dogBodyImage = dogBodyObject.GetComponent<Image>();
                dogBodyImage.sprite = dogDirty;
                BathTub.SetActive(true);

                HowToBathPanel1.SetActive(true);
                Invoke(nameof(CloseAllAlert), 2.0f);
            }
            else
            {
                YellowShine.SetActive(true);
                CantBathAlertPanel.SetActive(true);
                Invoke(nameof(CloseAllAlert), 2.0f);
            }
        }
        else
        {
            titleBarText.text = "양치";
            dogCleanFace.SetActive(true);

            if (brushTeethPossible)
            {
                dogDirtyFace.SetActive(true);

                dogFace = dogDirtyFace.GetComponent<Button>();
                dogFace.onClick.AddListener(OnClickDogFace);

                BrushTeethPanel1.SetActive(true);
                Invoke(nameof(CloseAllAlert), 2.0f);
            }
            else
            {
                WhiteShine.SetActive(true);
                CantTeethAlertPanel.SetActive(true);
                Invoke(nameof(CloseAllAlert), 2.0f);
            }
        }
        titleBar.SetActive(true);
    }

    void OnClickDogBody()
    {
        Debug.Log("씻기는 중");
        if (dogState == 0)
        { // 거품 내기
            if (dogCount < dogLimit)
            {
                Vector3 randomPosition = new(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-5f, 1f));
                Quaternion randomRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                bubbleArray[dogCount++] = Instantiate(bubblePrefab, randomPosition, randomRotation, bubbleParent);
            }
            else
            {
                dogState = 1;
                HowToBathPanel2.SetActive(true);
                Invoke(nameof(CloseAllAlert), 2.0f);

                dogBody = dogBodyObject.GetComponent<Button>();
                dogBody.onClick.AddListener(OnClickDogBody);

                Image dogBodyImage = dogBodyObject.GetComponent<Image>();
                dogBodyImage.sprite = dogClean;
                BathTub.SetActive(false);
            }
        }
        else if (dogState == 1)
        { // 물로 씻기
            if (dogCount > 0)
            {
                Destroy(bubbleArray[--dogCount]);
            }
            else
            {
                dogState = 2;
                background.sprite = wetImage;

                YellowShine.SetActive(true);
                HowToBathPanel3.SetActive(true);
                Invoke(nameof(CloseAllAlert), 2.0f);

                dogBody.onClick.RemoveListener(OnClickDogBody);
                StartCoroutine(DoBath());
            }
        }
    }

    void OnClickDogFace()
    {
        if (dogCount < dogLimit)
        {
            Image dogFaceImage = dogDirtyFace.GetComponent<Image>();
            Color color = dogFaceImage.color;
            color.a -= 0.1f;
            dogFaceImage.color = color;

            dogCount++;
        }
        else
        {
            dogDirtyFace.SetActive(false);

            WhiteShine.SetActive(true);
            BrushTeethPanel2.SetActive(true);
            Invoke(nameof(CloseAllAlert), 2.0f);

            dogFace.onClick.RemoveListener(OnClickDogFace);
            StartCoroutine(BrushTeeth());
        }
    }

    void CloseAllAlert()
    {
        CantBathAlertPanel.SetActive(false);
        HowToBathPanel1.SetActive(false);
        HowToBathPanel2.SetActive(false);
        HowToBathPanel3.SetActive(false);
        CantTeethAlertPanel.SetActive(false);
        BrushTeethPanel1.SetActive(false);
        BrushTeethPanel2.SetActive(false);
    }

    IEnumerator CheckBathTime()
    {
        string url = ServerSettings.SERVER_URL + "/api/bath/list";
        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        UnityWebRequest response = UnityWebRequest.Get(url);
        response.SetRequestHeader("Authorization", jwtToken);
        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log("BathScene CheckBathTime Success"); // Debug Code

            APIResponse<List<BathListResponseDtoClass>> apiResponse =
                JsonUtility.FromJson<APIResponse<List<BathListResponseDtoClass>>>(response.downloadHandler.text);

            if (apiResponse.status == "SUCCESS")
            {
                List<BathListResponseDtoClass> bathHistories = apiResponse.data;

                string lastBathTime = bathHistories[bathHistories.Count - 1].createTime;
                DateTime createDate = DateTime.Parse(lastBathTime);

                TimeSpan dateDiff = DateTime.Now.Date - createDate.Date;
                if (dateDiff.Days + 1 > 35) bathPossible = true; // 5 weeks

            }
            else
            {
                bathPossible = true;
            }
        }
        else
        {
            Debug.Log("BathScene CheckBathTime Error!"); // Debug Code

            if (response.responseCode == 404)
            {
                bathPossible = true;
                Debug.Log("목욕 기록이 없으므로 목욕 가능"); // Debug Code
            }
            else if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                Debug.Log("목욕 확인하려는데 403 나와서 토큰 재발급"); // Debug Code
                StartCoroutine(CheckBathTime());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }

    }

    IEnumerator CheckTeethPossible()
    {
        Debug.Log("양치 체크");

        string api_url = $"{ServerSettings.SERVER_URL}/api/brushing-teeth/check";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();
        Debug.Log("brushing-teeth/check result: " + response.downloadHandler.text); /// TEST

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text); // Debug Code

            TeethCheckResponseDtoClass teeth = JsonUtility.FromJson<APIResponse<TeethCheckResponseDtoClass>>(response.downloadHandler.text).data;
            brushTeethPossible = teeth.possibleYn;
            Debug.Log("양치 가능 여부: " + brushTeethPossible); // Debug Code

            Debug.Log("BathScene CheckTeethPossible Success!"); // Debug Code
        }
        else
        {
            Debug.Log("BathScene CheckTeethPossible Error!"); // Debug Code

            if (response.responseCode == 404)
            {
                brushTeethPossible = true;
                Debug.Log("양치 기록이 없으므로 양치 가능"); // Debug Code
            }
            else if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                Debug.Log("양치 확인하려는데 403 나와서 토큰 재발급"); // Debug Code
                StartCoroutine(CheckTeethPossible());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    IEnumerator DoBath()
    {
        string url = ServerSettings.SERVER_URL + "/api/bath/save"; // TEST CODE
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("BathScene DoBath Success! " + request.error);
        }
        else
        {
            Debug.Log("BathScene DoBath Error!");

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(DoBath());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    IEnumerator BrushTeeth()
    {
        string url = ServerSettings.SERVER_URL + "/api/brushing-teeth/save";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("BathScene BrushTeeth Success! " + request.error);
        }
        else
        {
            Debug.Log("BathScene BrushTeeth error!");

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(BrushTeeth());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("BathScene - 종료!!!!!\n입장 시간:" + PlayerPrefs.GetString("enterTime"));

            PlayerPrefs.SetString("exitTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));

            Debug.Log("퇴장 시간:" + PlayerPrefs.GetString("exitTime"));

            DateTime enterTime = DateTime.Parse(PlayerPrefs.GetString("enterTime"));
            DateTime exitTime = DateTime.Parse(PlayerPrefs.GetString("exitTime"));
            TimeSpan timeDiff = exitTime - enterTime;

            int diffMinute = timeDiff.Days * 24 * 60 + timeDiff.Hours * 60 + timeDiff.Minutes;

            Debug.Log("OnApplicationPause - diffMinute: " + diffMinute);
            RefreshTokenManager.Instance.UpdateTotalTime(diffMinute);
        }
        else
        {
            Debug.Log("BathScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
