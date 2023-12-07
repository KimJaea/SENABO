using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FeedModal : MonoBehaviour
{
    public GameObject DayFeedElement;
    public GameObject FeedRecordElemnt;
    public GameObject FeedContent;

    public GameObject FeedEmptyGroup;
    public GameObject FeedScrollView;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    private void Awake()
    {
        StartCoroutine(GetWeeklyFeedList());
    }

    string GetVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "에게";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이에게" : "에게";
    }

    IEnumerator GetWeeklyFeedList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/feed/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<FeedClass> feeds = JsonUtility.FromJson<APIResponse<List<FeedClass>>>(response.downloadHandler.text).data;

            List<List<FeedClass>> feedsByCreateTime = new List<List<FeedClass>>();
            DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

            int index = -1;
            for (int i = 0; i < feeds.Count; i++)
            {
                FeedClass feed = feeds[i];

                DateTime feedDate = Convert.ToDateTime(DateTime.Parse(feed.createTime).ToString("yyyy.MM.dd"));

                if (DateTime.Compare(date, feedDate) != 0)
                {
                    date = feedDate;
                    feedsByCreateTime.Add(new List<FeedClass>());
                    index++;
                }

                feedsByCreateTime[index].Add(feed);
            }

            for (int i = 0; i < feedsByCreateTime.Count; i++)
            {
                GameObject dayFeedElement = Instantiate(DayFeedElement);
                dayFeedElement.name = $"{DateTime.Parse(feedsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                dayFeedElement.transform.SetParent(FeedContent.transform);
                dayFeedElement.transform.localScale = scale;

                dayFeedElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(feedsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // 날짜 지정

                for (int j = 0; j < feedsByCreateTime[i].Count; j++)
                {
                    FeedClass feed = feedsByCreateTime[i][j];

                    GameObject feedRecordElemnt = Instantiate(FeedRecordElemnt);
                    feedRecordElemnt.transform.SetParent(dayFeedElement.transform.GetChild(1).transform);
                    feedRecordElemnt.transform.localScale = scale;

                    feedRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(feed.createTime):HH:mm}";
                    feedRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 밥을 줬어요!";
                }
            }
        }
        else
        {
            Debug.Log("주간 배식 내역 불러오기 실패");

            //if (response.responseCode == 403)
            //{
            //    RefreshTokenManager.Instance.ReIssueRefreshToken();

            //    StartCoroutine(GetWeeklyFeedList());
            //}
            //else
            //{
            //    SceneManager.LoadScene("WeeklyReportScene");
            //}
        }
    }
}
