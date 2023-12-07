using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommunicationModal : MonoBehaviour
{
    public GameObject DayCommunicationElement;
    public GameObject CommunicationRecordElemnt;
    public GameObject CommunicationContent;

    public GameObject CommunicationEmptyGroup;
    public GameObject CommunicationScrollView;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    private void Awake()
    {
        StartCoroutine(GetWeeklyCommunicationList());
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

            List<CommunicationClass> communications = JsonUtility.FromJson<APIResponse<List<CommunicationClass>>>(response.downloadHandler.text).data;

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
                    communicationRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{communication.type}을 하며 교감했어요.";
                }
            }
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
                SceneManager.LoadScene("WeeklyReportScene");
            }
        }
    }

}
