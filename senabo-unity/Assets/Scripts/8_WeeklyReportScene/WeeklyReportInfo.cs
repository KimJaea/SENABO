using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeeklyReportInfo : MonoBehaviour
{
    public Text weeklyReportDateText;
    public Text weeklyReportTimeDynamicText;

    public Text contentText;

    public Text affectionIndexDiffText;
    public Text stressIndexDiffText;

    public Text affectionIndexText;
    public Text stressIndexText;

    public GameObject CommunicationGage;
    public GameObject FeedGage;
    public GameObject WalkGage;
    public GameObject PoopGage;
    public GameObject HealthGage;

    public GameObject TextGroup;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    private void Start()
    {
        CommunicationGage.SetActive(false);
        FeedGage.SetActive(false);
        WalkGage.SetActive(false);
        PoopGage.SetActive(false);
        HealthGage.SetActive(false);

        StartCoroutine(GetWeeklyCommunicationList());
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

    IEnumerator GetDetailReport()
    {
        Debug.Log(CountReport.selectedReportWeek);

        string api_url = $"{ServerSettings.SERVER_URL}/api/report/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            DetailReportClass report = 
                JsonUtility.FromJson<APIResponse<DetailReportClass>>(response.downloadHandler.text).data;

            Debug.Log(report.createTime);

            weeklyReportDateText.text = $"{DateTime.Parse(report.createTime):yyyy.MM.dd} - {DateTime.Parse(report.createTime).AddDays(6):yyyy.MM.dd}";

            weeklyReportTimeDynamicText.text = $"{report.dogName}{GetVerb(report.dogName)} {GetTotalTimeString(report.totalTime)}";

            int affectionIndexDiff = report.endAffectionScore - report.startAffectionScore;

            if (affectionIndexDiff != 0)
            {
                affectionIndexDiffText.text = $"{(affectionIndexDiff < 0 ? "-" : "+")}{Math.Abs(affectionIndexDiff)}";

                if (affectionIndexDiff < 0)
                {
                    affectionIndexDiffText.color = Color.red;
                }
                else
                {
                    affectionIndexDiffText.color = Color.blue;
                }
            }

            int stressIndexDiff = report.endStressScore - report.startStressScore;

            if (stressIndexDiff != 0)
            {
                stressIndexDiffText.text = $"{(stressIndexDiff < 0 ? "-" : "+")}{Math.Abs(stressIndexDiff)}";
                if (stressIndexDiff < 0)
                {
                    stressIndexDiffText.color = Color.blue;
                }
                else
                {
                    stressIndexDiffText.color = Color.red;
                }
            }

            affectionIndexText.text = report.endAffectionScore.ToString();
            stressIndexText.text = report.endStressScore.ToString();

            StartCoroutine(DisplayGage(CommunicationGage, report.communicationScore));
            StartCoroutine(DisplayGage(FeedGage, report.feedScore));
            StartCoroutine(DisplayGage(WalkGage, report.walkScore));
            StartCoroutine(DisplayGage(PoopGage, report.poopScore));
            StartCoroutine(DisplayGage(HealthGage, report.diseaseScore));
        }
        else
        {
            Debug.Log("주간 상세 리포트 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetDetailReport());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }

        contentText.transform.SetParent(TextGroup.transform);
        contentText.transform.localScale = scale;
    }

    /*
     *  Poop Modal
     */

    public GameObject DayPoopElement;
    public GameObject PoopRecordElemnt;
    public GameObject PoopContent;

    public GameObject PoopEmptyGroup;
    public GameObject PoopScrollView;

    string GetPoopVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "의";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이의" : "의";
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
        string api_url = $"{ServerSettings.SERVER_URL}/api/feed/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<PoopClass> poops = JsonUtility.FromJson<APIResponse<List<PoopClass>>>(response.downloadHandler.text).data;

            if (poops.Count == 0)
            {
                PoopScrollView.SetActive(false);
                PoopEmptyGroup.SetActive(true);

                contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} 배변을 하지 않았어요!\n밥을 주지 않아 그런 걸 수도 있으니 세심한 관리가 필요해요.\n\n";
            }
            else
            {
                PoopScrollView.SetActive(true);
                PoopEmptyGroup.SetActive(false);

                List<List<PoopClass>> poopsByCreateTime = new List<List<PoopClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                for (int i = 0; i < poops.Count; i++)
                {
                    PoopClass poop = poops[i];

                    if (Convert.ToDateTime(poop.createTime).AddHours(1) > DateTime.Now)
                    {
                        continue;
                    }

                    DateTime poopDate = Convert.ToDateTime(DateTime.Parse(poop.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, poopDate) != 0)
                    {
                        date = poopDate;
                        poopsByCreateTime.Add(new List<PoopClass>());
                        index++;
                    }

                    poopsByCreateTime[index].Add(poop);
                }

                int totalTimeMinute = 0;
                if (poopsByCreateTime.Count == 0)
                {
                    PoopScrollView.SetActive(false);
                    PoopEmptyGroup.SetActive(true);
                }
                else
                {
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

                            poopRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(poop.createTime).AddHours(1):HH:mm}";

                            if (poop.cleanYn)
                            {
                                poopRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = GetDiffTimeString(DateTime.Parse(poop.createTime).AddHours(1).ToString(), poop.updateTime);
                                totalTimeMinute += GetTimeDiff(DateTime.Parse(poop.createTime).AddHours(1).ToString(), poop.updateTime);
                            }
                            else
                            {
                                poopRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "아직 치우지 않았어요!";
                                totalTimeMinute += GetTimeDiff(DateTime.Parse(poop.createTime).AddHours(1).ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                    }

                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetPoopVerb(PlayerPrefs.GetString("dogName"))} 배변을 평균 {GetTotalTimeString(totalTimeMinute / poops.Count)} 후에 치웠어요.\n";
                    if (totalTimeMinute / poops.Count <= 30)
                    {
                        contentText.text += "배변 관리를 아주 잘하고 있어요!\n\n";
                    }
                    else
                    {
                        contentText.text += $"배변을 제때 치우지 않아 {PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} 스트레스를 받아요.\n\n";
                    }
                }

                StartCoroutine(GetWeeklyHealthList());
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
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    /*
     *  Communication Modal
     */
    public GameObject DayCommunicationElement;
    public GameObject CommunicationRecordElemnt;
    public GameObject CommunicationContent;

    public GameObject CommunicationEmptyGroup;
    public GameObject CommunicationScrollView;

    private string GetActivityTypeString(string type)
    {
        string value = "";

        switch (type)
        {
            case "WAIT":
                value = "기다려 훈련을 했어요.";
                break;
            case "SIT":
                value = "앉아 훈련을 했어요.";
                break;
            case "HAND":
                value = "손 훈련을 했어요.";
                break;
            case "PAT":
                value = "쓰다듬기를 했어요.";
                break;
            case "TUG":
                value = "터그 놀이를 했어요.";
                break;
            case "WALK":
                value = "산책을 했어요.";
                break;
        }

        return value;
    }

    IEnumerator GetWeeklyCommunicationList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/communication/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<CommunicationClass> communications 
                = JsonUtility.FromJson<APIResponse<List<CommunicationClass>>>(response.downloadHandler.text).data;

            if (communications.Count == 0)
            {
                CommunicationScrollView.SetActive(false);
                CommunicationEmptyGroup.SetActive(true);

                contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 교감을 하지 않았어요!\n훈련을 위해선 충분한 교감이 필요해요.\n\n";
            } else
            {
                CommunicationScrollView.SetActive(true);
                CommunicationEmptyGroup.SetActive(false);

                List<List<CommunicationClass>> communicationsByCreateTime = new List<List<CommunicationClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                for (int i = 0; i < communications.Count; i++)
                {
                    CommunicationClass communication = communications[i];

                    DateTime communicationDate = Convert.ToDateTime(DateTime.Parse(communication.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, communicationDate) != 0)
                    {
                        date = communicationDate;
                        communicationsByCreateTime.Add(new List<CommunicationClass>());
                        index++;
                    }

                    communicationsByCreateTime[index].Add(communication);
                }

                for (int i = 0; i < communicationsByCreateTime.Count; i++)
                {
                    GameObject dayCommunicationElement = Instantiate(DayCommunicationElement);
                    dayCommunicationElement.name = $"{DateTime.Parse(communicationsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayCommunicationElement.transform.SetParent(CommunicationContent.transform);
                    dayCommunicationElement.transform.localScale = scale;

                    dayCommunicationElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(communicationsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // 날짜 지정

                    for (int j = 0; j < communicationsByCreateTime[i].Count; j++)
                    {
                        CommunicationClass communication = communicationsByCreateTime[i][j];

                        GameObject communicationRecordElemnt = Instantiate(CommunicationRecordElemnt);
                        communicationRecordElemnt.transform.SetParent(dayCommunicationElement.transform.GetChild(1).transform);
                        communicationRecordElemnt.transform.localScale = scale;

                        communicationRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(communication.createTime):HH:mm}";
                        communicationRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetActivityTypeString(communication.type)}";
                    }
                }

                contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} {communications.Count}회 교감했어요.\n";
                if (communications.Count <= 25)
                {
                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 교감이 매우 적어요.\n\n";
                } else if(communications.Count <= 35)
                {
                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 교감이 더 필요해요.\n\n";
                } else
                {
                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 교감을 잘하고 있어요!\n\n";
                }
            }

            StartCoroutine(GetWeeklyFeedList());
        }
        else
        {
            Debug.Log("주간 교감 내역 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWeeklyCommunicationList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    /*
     *  Walk Modal
     */

    public GameObject DayWalkElement;
    public GameObject WalkRecordElemnt;
    public GameObject WalkContent;

    public GameObject WalkEmptyGroup;
    public GameObject WalkScrollView;

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

    string GetTotalTimeString(int totalTimeMinute)
    {
        int hours = totalTimeMinute / 60;
        int minutes = totalTimeMinute % 60;

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

    int GetTimeDiff(string startTime, string endTime)
    {
        DateTime StartTime = DateTime.Parse(startTime);
        DateTime EndTime = DateTime.Parse(endTime);
        TimeSpan dateDiff = EndTime - StartTime;

        int time = (int)dateDiff.TotalMinutes;

        return time;
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

            if (walks.Count == 0)
            {
                WalkScrollView.SetActive(false);
                WalkEmptyGroup.SetActive(true);
                contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 산책을 하지 않았어요!\n산책하지 않으면 {PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} 스트레스를 받아요.\n\n";
            }
            else
            {
                WalkScrollView.SetActive(true);
                WalkEmptyGroup.SetActive(false);

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

                int totalTimeMinute = 0;
                double totalDistance = 0;

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
                        walkRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetTimeFormat(walk.startTime, walk.endTime)} 동안\n{Math.Round(walk.distance, 2)}km를 걸었어요.";

                        totalTimeMinute += GetTimeDiff(walk.startTime, walk.endTime);
                        totalDistance += walk.distance;
                    }
                }

                contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 총 {GetTotalTimeString(totalTimeMinute)} 동안 {Math.Round(totalDistance, 2)}km를 산책했고,\n하루 평균 {GetTotalTimeString(totalTimeMinute / 7)} 동안 {Math.Round(totalDistance / 7, 2)}km를 산책했어요.\n";
                if (totalTimeMinute <= 8)
                {
                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 산책이 매우 적어요.\n\n";
                }
                else if (totalDistance <= 15)
                {
                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 산책이 더 필요해요.\n\n";
                }
                else
                {
                    contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 산책을 잘하고 있어요!\n\n";
                }
            }

            StartCoroutine(GetWeeklyPoopList());
        }
        else
        {
            Debug.Log("주간 산책 내역 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWeeklyWalkList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    /*
     *  Health Modal
     */
    public GameObject DayHealthElement;
    public GameObject HealthRecordElemnt;
    public GameObject HealthContent;

    public GameObject HealthEmptyGroup;
    public GameObject HealthScrollView;

    string GetHealthVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "가";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이가" : "가";
    }

    string GetNeunVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "는";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이는" : "는";
    }

    IEnumerator GetWeeklyHealthList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/disease/list/{CountReport.selectedReportWeek}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<HealthClass> healths = JsonUtility.FromJson<APIResponse<List<HealthClass>>>(response.downloadHandler.text).data;

            if (healths.Count == 0)
            {
                HealthScrollView.SetActive(false);
                HealthEmptyGroup.SetActive(true);

                contentText.text += $"이번 주 {PlayerPrefs.GetString("dogName")}{GetNeunVerb(PlayerPrefs.GetString("dogName"))} 건강해요.\n\n";
            }
            else
            {
                HealthScrollView.SetActive(true);
                HealthEmptyGroup.SetActive(false);

                List<List<HealthClass>> healthsByCreateTime = new List<List<HealthClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                for (int i = 0; i < healths.Count; i++)
                {
                    HealthClass health = healths[i];

                    DateTime healthDate = Convert.ToDateTime(DateTime.Parse(health.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, healthDate) != 0)
                    {
                        date = healthDate;
                        healthsByCreateTime.Add(new List<HealthClass>());
                        index++;
                    }

                    healthsByCreateTime[index].Add(health);
                }

                for (int i = 0; i < healthsByCreateTime.Count; i++)
                {
                    GameObject dayHealthElement = Instantiate(DayHealthElement);
                    dayHealthElement.name = $"{DateTime.Parse(healthsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayHealthElement.transform.SetParent(HealthContent.transform);
                    dayHealthElement.transform.localScale = scale;

                    dayHealthElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(healthsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // 날짜 지정

                    for (int j = 0; j < healthsByCreateTime[i].Count; j++)
                    {
                        HealthClass health = healthsByCreateTime[i][j];

                        GameObject healthRecordElemnt = Instantiate(HealthRecordElemnt);
                        healthRecordElemnt.transform.SetParent(dayHealthElement.transform.GetChild(1).transform);
                        healthRecordElemnt.transform.localScale = scale;

                        healthRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(health.createTime):HH:mm}";
                        healthRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} {health.diseaseName}에 걸려 아팠어요.";
                    }
                }

                contentText.text += $"이번 주 {PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} {healths.Count}번 아팠어요. 세심한 관리가 필요해요.\n\n";
            }

            StartCoroutine(GetDetailReport());
        }
        else
        {
            Debug.Log("주간 건강 내역 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWeeklyHealthList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    /*
     *  Feed Modal
     */
    public GameObject DayFeedElement;
    public GameObject FeedRecordElemnt;
    public GameObject FeedContent;

    public GameObject FeedEmptyGroup;
    public GameObject FeedScrollView;

    string GetFeedVerb(string dogName)
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

            if (feeds.Count == 0)
            {
                FeedScrollView.SetActive(false);
                FeedEmptyGroup.SetActive(true);

                contentText.text += $"이번 주 {PlayerPrefs.GetString("dogName")}{GetFeedVerb(PlayerPrefs.GetString("dogName"))} 밥을 주지 않았어요!\n밥을 주지 않으면 {PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} 아파요.\n\n";
            }
            else
            {
                FeedScrollView.SetActive(true);
                FeedEmptyGroup.SetActive(false);

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
                        feedRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{PlayerPrefs.GetString("dogName")}{GetFeedVerb(PlayerPrefs.GetString("dogName"))} 밥을 줬어요!";
                    }
                }

                contentText.text += $"{PlayerPrefs.GetString("dogName")}{GetFeedVerb(PlayerPrefs.GetString("dogName"))} {feeds.Count}회 밥을 주었어요.\n";
                if (feeds.Count < 14)
                {
                    contentText.text += $"배식 시간을 정확히 지켜주지 않으면 {PlayerPrefs.GetString("dogName")}{GetHealthVerb(PlayerPrefs.GetString("dogName"))} 아플 수도 있어요.\n\n";
                }
                else
                {
                    contentText.text += "배식 시간을 잘 지켰어요.\n\n";
                }
            }

            StartCoroutine(GetWeeklyWalkList());
        }
        else
        {
            Debug.Log("주간 배식 내역 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWeeklyFeedList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("WeeklyReportScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("WeeklyReportScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }

    IEnumerator DisplayGage(GameObject gage, float value)
    {
        gage.SetActive(true);

        Image image = gage.GetComponent<Image>();
        float multiplier = 6.3f;

        Vector2 initialSize = new Vector2(0, 50);
        Vector2 targetSize = new Vector2(multiplier * value, 50);

        float elapsedTime = 0f;
        float animationDuration = 1.5f;

        while (elapsedTime < animationDuration)
        {
            image.rectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.rectTransform.sizeDelta = targetSize;
    }
}