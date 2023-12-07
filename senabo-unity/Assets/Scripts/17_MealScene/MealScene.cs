using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

[System.Serializable]
public class MealSceneClass
{
    public bool possibleYn;
    public string lastFeedDateTime;
    public string nowDateTime;
}

public class MealScene : MonoBehaviour
{
    public Image mealImage;
    public Sprite fullSprite;
    public GameObject MealFedAlertPanel;
    private bool mealable = false;
    private Button button;
    void Start()
    {
        MealFedAlertPanel.SetActive(false);

        button = mealImage.GetComponent<Button>();
        button.onClick.AddListener(OnClickMeal);

        StartCoroutine(CheckFeed());
    }

    IEnumerator CheckFeed()
    {
        string url = ServerSettings.SERVER_URL + "/api/feed/check";
        UnityWebRequest request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("MealScene CheckFeed Success!"); // Debug Code

            string jsonString = request.downloadHandler.text;
            var response = JsonUtility.FromJson<APIResponse<MealSceneClass>>(jsonString);

            mealable = response.data.possibleYn;
        }
        else
        {
            Debug.Log("MealScene CheckFeed Error! " + request.error); // Debug Code
        }
    }

    IEnumerator GiveFeed()
    {
        string url = ServerSettings.SERVER_URL + "/api/feed/save"; // TEST CODE
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("MealScene GiveFeed Success! " + request.error);
        }
        else
        {
            Debug.Log("MealScene GiveFeed error!");

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GiveFeed());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    void OnClickMeal()
    {
        if (mealable)
        {
            // 배식 가능 상태, 배식 진행
            mealImage.sprite = fullSprite;

            StartCoroutine(GiveFeed());
            mealable = false;
        }
        else
        {
            // 배식 불가 상태, 해당 사항 알림
            MealFedAlertPanel.SetActive(true);
            Invoke(nameof(DestroyModal), 2.0f);
        }
    }

    void DestroyModal()
    {
        MealFedAlertPanel.SetActive(false);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("MealScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("MealScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
