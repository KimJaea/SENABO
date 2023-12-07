using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WalkModal : MonoBehaviour
{
    public GameObject DayWalkElement;
    public GameObject WalkRecordElemnt;
    public GameObject WalkContent;

    public GameObject WalkEmptyGroup;
    public GameObject WalkScrollView;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    private void Awake()
    {
        StartCoroutine(GetWeeklyWalkList());
    }

    string GetTimeFormat(string startTime, string endTime)
    {
        DateTime StartTime = DateTime.Parse(startTime);
        DateTime EndTime = DateTime.Parse(endTime);
        TimeSpan dateDiff = EndTime - StartTime;

        int time = (int)dateDiff.TotalMinutes;

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

    IEnumerator GetWeeklyWalkList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/walk/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<WalkClass> walks = JsonUtility.FromJson<APIResponse<List<WalkClass>>>(response.downloadHandler.text).data;

            List<List<WalkClass>> walksByCreateTime = new List<List<WalkClass>>();
            DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

            int index = -1;
            for (int i = 0; i < walks.Count; i++)
            {
                WalkClass walk = walks[i];

                DateTime walkDate = Convert.ToDateTime(DateTime.Parse(walk.startTime).ToString("yyyy.MM.dd"));

                if (DateTime.Compare(date, walkDate) != 0)
                {
                    date = walkDate;
                    walksByCreateTime.Add(new List<WalkClass>());
                    index++;
                }

                walksByCreateTime[index].Add(walk);
            }

            for (int i = 0; i < walksByCreateTime.Count; i++)
            {
                GameObject dayWalkElement = Instantiate(DayWalkElement);
                dayWalkElement.name = $"{DateTime.Parse(walksByCreateTime[i][0].createTime):yyyy.MM.dd}";
                dayWalkElement.transform.SetParent(WalkContent.transform);
                dayWalkElement.transform.localScale = scale;

                dayWalkElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(walksByCreateTime[i][0].startTime):yyyy.MM.dd}"; // 날짜 지정

                for (int j = 0; j < walksByCreateTime[i].Count; j++)
                {
                    WalkClass walk = walksByCreateTime[i][j];

                    GameObject walkRecordElemnt = Instantiate(WalkRecordElemnt);
                    walkRecordElemnt.transform.SetParent(dayWalkElement.transform.GetChild(1).transform);
                    walkRecordElemnt.transform.localScale = scale;

                    walkRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(walk.createTime):HH:mm}";
                    walkRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetTimeFormat(walk.startTime, walk.endTime)} 동안\n{walk.distance}km를 걸었어요.";
                }
            }
        }
        else
        {
            Debug.Log("주간 산책 관리 내역 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWeeklyWalkList());
            }
            else
            {
                SceneManager.LoadScene("WeeklyReportScene");
            }
        }
    }
}
