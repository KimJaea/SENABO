using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoopModal : MonoBehaviour
{
    public GameObject DayPoopElement;
    public GameObject PoopRecordElemnt;
    public GameObject PoopContent;

    public GameObject PoopEmptyGroup;
    public GameObject PoopScrollView;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    private void Awake()
    {
        StartCoroutine(GetWeeklyPoopList());
    }

    private string GetDiffTimeString(string startDate, string endDate)
    {
        DateTime StartDate = DateTime.Parse(startDate);
        DateTime EndDate = DateTime.Parse(endDate);
        TimeSpan dateDiff = EndDate - StartDate;

        int diffDay = dateDiff.Days;
        int diffHour = dateDiff.Hours;
        int diffMinute = dateDiff.Minutes;

        if (diffDay > 0)
        {
            if (diffHour > 0 || diffMinute > 0)
            {
                diffDay++;
            }

            return $"{diffDay}일 뒤 배변 패드 정리";
        }

        if (diffHour > 0)
        {
            if (diffMinute > 0)
            {
                diffHour++;
            }

            if (diffHour == 24)
            {
                return $"1일 뒤 배변 패드 정리";
            }

            return $"{diffHour}시간 뒤 배변 패드 정리";
        }

        return $"{diffMinute}분 뒤 배변 패드 정리";
    }

    IEnumerator GetWeeklyPoopList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/poop/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<PoopClass> poops = JsonUtility.FromJson<APIResponse<List<PoopClass>>>(response.downloadHandler.text).data;

            List<List<PoopClass>> poopsByCreateTime = new List<List<PoopClass>>();
            DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

            int index = -1;
            for (int i = 0; i < poops.Count; i++)
            {
                PoopClass poop = poops[i];

                DateTime poopDate = Convert.ToDateTime(DateTime.Parse(poop.createTime).ToString("yyyy.MM.dd"));

                if (DateTime.Compare(date, poopDate) != 0)
                {
                    date = poopDate;
                    poopsByCreateTime.Add(new List<PoopClass>());
                    index++;
                }

                poopsByCreateTime[index].Add(poop);
            }

            for (int i = 0; i < poopsByCreateTime.Count; i++)
            {
                GameObject dayPoopElement = Instantiate(DayPoopElement);
                dayPoopElement.name = $"{DateTime.Parse(poopsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                dayPoopElement.transform.SetParent(PoopContent.transform);
                dayPoopElement.transform.localScale = scale;

                dayPoopElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(poopsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // 날짜 지정

                for (int j = 0; j < poopsByCreateTime[i].Count; j++)
                {
                    PoopClass poop = poopsByCreateTime[i][j];

                    GameObject poopRecordElemnt = Instantiate(PoopRecordElemnt);
                    poopRecordElemnt.transform.SetParent(dayPoopElement.transform.GetChild(1).transform);
                    poopRecordElemnt.transform.localScale = scale;

                    poopRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(poop.createTime):HH:mm}";

                    if (poop.cleanYn)
                    {
                        poopRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = GetDiffTimeString(poop.createTime, poop.updateTime);
                    }
                    else
                    {
                        poopRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "아직 치우지 않았어요!";
                    }
                }
            }
        }
        else
        {
            Debug.Log("주간 배변 관리 내역 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWeeklyPoopList());
            }
            else
            {
                SceneManager.LoadScene("WeeklyReportScene");
            }
        }
    }
}
