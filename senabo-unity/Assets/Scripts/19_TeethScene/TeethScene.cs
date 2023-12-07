using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class TeethScene : MonoBehaviour
{
    public Image background, dogImage;
    // public Sprite cleanTeethSprite;
    private int clickedCount = 0, changeLimit = 5;
    private Button button;

    void Start()
    {
        button = dogImage.GetComponent<Button>();
        button.onClick.AddListener(OnClickDog);
    }

    IEnumerator Upload()
    {
        string url = ServerSettings.SERVER_URL + "/api/brushing-teeth/save"; // TEST CODE
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("TeethScene Success!");
        }
        else
        {
            Debug.Log("TeethScene Error! " + request.error);

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(Upload());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    void OnClickDog()
    {
        clickedCount++;
        if (clickedCount > changeLimit)
        {
            StartCoroutine(Upload());
            button.onClick.RemoveListener(OnClickDog);
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
            Debug.Log("TeethScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("TeethScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
