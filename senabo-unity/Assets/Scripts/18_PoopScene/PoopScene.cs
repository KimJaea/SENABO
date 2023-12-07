using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoopScene : MonoBehaviour
{
    public Image poopPadImage;
    public GameObject poopPadObject, plasticBagObject, shiningObject;
    public Sprite[] poopPadSprites; // clean, fold1, fold2, fold3, dirt1, dirt2, dirt3
    public GameObject NoPoopPanel, PoopCleanPanel1, PoopCleanPanel2, PoopCleanPanel3;
    private bool isPoop = false;
    private int padCount = 0, padState = 0, padLimit = 3;
    private Button button, button2;

    void Start()
    {
        StartCoroutine(CheckIsPoop());
    }

    void AfterJudgePoop()
    {
        Debug.Log("PoopScene 배변 여무:" + isPoop);

        poopPadObject.SetActive(true);

        if (isPoop)
        {
            PoopCleanPanel1.SetActive(true);
            Invoke(nameof(CloseAllPanel), 1.0f);

            SetPoopPadImage();
            button = poopPadImage.GetComponent<Button>();
            button.onClick.AddListener(OnClickPoopPad);
            button2 = plasticBagObject.GetComponent<Button>();
            button2.onClick.AddListener(OnClickPoopPad);
        }
        else
        {
            shiningObject.SetActive(true);
            NoPoopPanel.SetActive(true);
            Invoke(nameof(CloseAllPanel), 2.0f);
        }
    }

    void SetPoopPadImage()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(2); // dirt sprite: 2
        poopPadImage.sprite = poopPadSprites[randomNumber + 4];
    }

    void OnClickPoopPad()
    {
        if (padState < 3)
        {
            padCount++;

            if (padCount >= padLimit)
            {
                Debug.Log("똥 다치움:" + padCount);

                poopPadImage.sprite = poopPadSprites[++padState];
                padCount = 0;

                if (padState == 3)
                {
                    PoopCleanPanel2.SetActive(true);
                    Invoke(nameof(CloseAllPanel), 1.0f);

                    poopPadObject.transform.position = new Vector3(0, 2);
                    plasticBagObject.SetActive(true);
                }
            }
        }
        else
        {
            if (padCount == 0)
            {
                poopPadObject.SetActive(false);
            }
            else if (padCount == 1)
            {
                plasticBagObject.SetActive(false);
                poopPadObject.SetActive(true);
                poopPadImage.sprite = poopPadSprites[0];

                poopPadObject.transform.position = new Vector3(0.0f, 0.0f);

                PoopCleanPanel3.SetActive(true);
                Invoke(nameof(CloseAllPanel), 2.0f);
                shiningObject.SetActive(true);

                button.onClick.RemoveListener(OnClickPoopPad);
                button2.onClick.RemoveListener(OnClickPoopPad);

                StartCoroutine(CleanPoop());
            }

            padCount++;
        }
    }

    void CloseAllPanel()
    {
        NoPoopPanel.SetActive(false);
        PoopCleanPanel1.SetActive(false);
        PoopCleanPanel2.SetActive(false);
        PoopCleanPanel3.SetActive(false);
    }

    IEnumerator CreatePoop()
    {
        string url = ServerSettings.SERVER_URL + "/api/poop/save"; // TEST CODE
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("PoopScene CreatePoop Success! ");
        }
        else
        {
            Debug.Log("PoopScene CreatePoop error!" + request.error);

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(CreatePoop());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    IEnumerator CheckIsPoop()
    {
        Debug.Log("Poop Scene 들어옴");
        string url = ServerSettings.SERVER_URL + "/api/feed/latest"; // TEST CODE

        UnityWebRequest www = UnityWebRequest.Get(url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        www.SetRequestHeader("Authorization", jwtToken);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log("PoopScene CheckIsPoop Success"); // Debug Code

            FeedLatestDtoClass poop = JsonUtility.FromJson<APIResponse<FeedLatestDtoClass>>(www.downloadHandler.text).data;

            DateTime poopTime = Convert.ToDateTime(poop.createTime).AddHours(1);

            if (DateTime.Now >= poopTime && !poop.cleanYn)
            {
                isPoop = true;
            }
            else
            {
            }
        }
        else
        {
            Debug.Log("PoopScene CheckIsPoop Error:" + www.responseCode); // Debug Code

            if (www.responseCode == 404)
            {
                isPoop = false;
            }
            else if (www.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();
                StartCoroutine(CheckIsPoop());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
        Debug.Log("PoopScene의 배변 여부: " + isPoop);// TEST
        AfterJudgePoop();
    }

    IEnumerator CleanPoop()
    {
        Debug.Log("똥치우기!");

        string url = ServerSettings.SERVER_URL + "/api/feed/clean";
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("PoopScene CleanPoop Success!");
        }
        else
        {
            Debug.Log("PoopScene CleanPoop Error! " + request.error);

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(CleanPoop());
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
            Debug.Log("PoopScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

            PlayerPrefs.SetString("exitTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));

            Debug.Log("퇴장시간:" + PlayerPrefs.GetString("exitTime"));

            DateTime enterTime = DateTime.Parse(PlayerPrefs.GetString("enterTime"));
            DateTime exitTime = DateTime.Parse(PlayerPrefs.GetString("exitTime"));
            TimeSpan timeDiff = exitTime - enterTime;

            int diffMinute = timeDiff.Days * 24 * 60 + timeDiff.Hours * 60 + timeDiff.Minutes;

            Debug.Log("OnApplicationPause - diffMinute: " + diffMinute);
            RefreshTokenManager.Instance.UpdateTotalTime(diffMinute);
        }
        else
        {
            Debug.Log("PoopScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
