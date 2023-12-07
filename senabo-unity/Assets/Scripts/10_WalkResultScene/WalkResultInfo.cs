using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WalkResultInfo : MonoBehaviour
{
    public Text DogNameText;
    public Text WithText;
    public Text TimeText;
    public Text DistanceText;

    public Text TodayWalkTimeText;
    public Text TodayWalkDistanceText;

    private void Awake()
    {
        DogNameText.text = PlayerPrefs.GetString("dogName");
        WithText.text = GetVerb(PlayerPrefs.GetString("dogName"));

        StartCoroutine(GetLatestWalk());
        StartCoroutine(GetTodayWalkInfo());
    }

    private string GetDiffTimeString(string startDate, string endDate)
    {
        DateTime StartDate = DateTime.Parse(startDate);
        DateTime EndDate = DateTime.Parse(endDate);
        TimeSpan dateDiff = EndDate - StartDate;

        int diffHour = dateDiff.Hours;
        int diffMinute = dateDiff.Minutes;

        if (diffHour > 0)
        {
            if (diffMinute > 0)
            {
                return $"{diffHour}시간 {diffMinute}분";
            }

            return $"{diffHour}시간";
        }

        return $"{diffMinute}분";
    }

    string GetVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "와";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이와" : "와";
    }

    string GetTimeFormat(int time)
    {
        int hours = time / 60;
        int minutes = time % 60;

        if (hours > 0 && minutes > 0)
        {
            return $"{hours}시간 {minutes}분";
        }

        if (hours > 0)
        {
            return $"{hours}시간";
        }

        return $"{minutes}분";
    }

    IEnumerator GetLatestWalk()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/walk/latest";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            LatestWalkInfoClass latestWalkInfo = JsonUtility.FromJson<APIResponse<LatestWalkInfoClass>>(response.downloadHandler.text).data;

            TimeText.text = TimeText.text = GetDiffTimeString(latestWalkInfo.startTime, latestWalkInfo.endTime);
            DistanceText.text = $" {Math.Round(latestWalkInfo.distance, 2)}km";
        }
        else
        {
            Debug.Log("최신 산책 정보 불러오기 실패");

            if (response.responseCode == 404)
            {
                // alert 오류 
                SceneManager.LoadScene("MainScene");
            }
            else if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetLatestWalk());
            }
            else
            {
                Debug.Log("최신 산책 정보 리프레시 토큰 발급 실패");

                SceneManager.LoadScene("MainScene");
            }
        }
    }

    IEnumerator GetTodayWalkInfo()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/walk/today";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            TodayWalkInfoClass todayWalkInfo = JsonUtility.FromJson<APIResponse<TodayWalkInfoClass>>(response.downloadHandler.text).data;

            TodayWalkTimeText.text = GetTimeFormat(todayWalkInfo.todayTotalWalkTime);
            TodayWalkDistanceText.text = $"{Math.Round(todayWalkInfo.todayTotalWalkDistance, 2)}km";
        }
        else
        {
            Debug.Log("일일 산책 정보 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetTodayWalkInfo());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("WalkResultScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("ReportScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
