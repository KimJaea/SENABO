using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeartType
{
    public const int WAIT = 100;
    public const int SIT = 200;
    public const int HAND = 300;
    public const int PAT = 400;
    public const int TUG = 500;
    public const int WALK = 600;
}

public class HeartScene : MonoBehaviour
{
    public GameObject dogImage, heartImage, dogAnimationGroup;
    public GameObject[] heartButtonObjects;
    public GameObject[] dogAnimations;

    private bool heartAnimating = false;
    private bool communicating = false;
    private readonly float heartDelayTime = 1.0f;
    private float[] animationDelayTime = { 2.2f, 2.1f, 2.0f, 5.7f, 4.0f };
    private Button dogBody;

    void Start()
    {
        dogBody = dogAnimationGroup.GetComponent<Button>();
        dogBody.onClick.AddListener(GiveHeart);
    }

    public void OnClickButton(int type)
    {
        if (communicating)
        {
            return;
        }

        communicating = true;

        PlaySelectedAnimation(type);
        StartCoroutine(CommunicationAfterDelay(type));
    }

    void PlaySelectedAnimation(int type)
    {
        AllButtonsDisable();
        dogImage.SetActive(false);
        StopAnimation();
        dogAnimations[type].SetActive(true);
    }

    void StopAnimation()
    {
        for (int i = 0; i < dogAnimations.Length; i++)
        {
            dogAnimations[i].SetActive(false);
        }
    }

    void AllButtonsDisable()
    {
        for (int i = 0; i < heartButtonObjects.Length; i++)
        {
            heartButtonObjects[i].GetComponent<Button>().interactable = false;
        }
    }

    void AllButtonsAble()
    {
        for (int i = 0; i < heartButtonObjects.Length; i++)
        {
            heartButtonObjects[i].GetComponent<Button>().interactable = true;
        }
    }

    public void GiveHeart()
    {
        if (heartAnimating)
        {
            return;
        }

        heartAnimating = true;
        heartImage.SetActive(true);

        Invoke(nameof(StopHeart), heartDelayTime);
    }

    public void StopHeart()
    {
        heartAnimating = false;
        heartImage.SetActive(false);
    }

    void FinishCommunicating()
    {
        StopAnimation();
        dogImage.SetActive(true);
        AllButtonsAble();
        communicating = false;
        Debug.Log("모두 끝남." + DateTime.Now);
    }

    IEnumerator CommunicationAfterDelay(int type)
    {
        yield return new WaitForSeconds(animationDelayTime[type]);

        StartCoroutine(Upload(type));
    }

    string GetHeartText(int type)
    {
        return type switch
        {
            0 => "WAIT",
            1 => "SIT",
            2 => "HAND",
            3 => "PAT",
            4 => "TUG",
            _ => "",// SHOULD NEVER RUN
        };
    }

    IEnumerator Upload(int type)
    {
        string url = $"{ServerSettings.SERVER_URL}/api/communication/save/{GetHeartText(type)}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        communicating = false;

        if (request.error == null)
        {
            Debug.Log("HeartScene Success! " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("HeartScene error!");

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(Upload(type));
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }

        FinishCommunicating();
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("HeartScene - 종료!!!!!\n입장 시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("HeartScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
