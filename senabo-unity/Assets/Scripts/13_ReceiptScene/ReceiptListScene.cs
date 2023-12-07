using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ReceiptResponseDtoClass
{
    public long id;
    public long memberId;
    public string item;
    public string detail;
    public int amount;
    public string createTime;
    public string updateTime;
}

public class ReceiptListScene : MonoBehaviour
{
    public Text totalPriceText;
    public GameObject receiptPrefab;
    public Transform receiptContent;

    void Start()
    {
        StartCoroutine(GetReceiptHistory());
    }

    IEnumerator GetReceiptHistory()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/expense/list";
        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);
        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            List<ReceiptResponseDtoClass> receiptHistories =
                JsonUtility.FromJson<APIResponse<List<ReceiptResponseDtoClass>>>(response.downloadHandler.text).data;

            CreateReceiptPrefabs(receiptHistories);
        }
        else
        {
            Debug.Log("전체 영수증 기록 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetReceiptHistory());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    string intToStringWon(int amount)
    {
        return amount.ToString("#,0") + "원";
    }

    void CreateReceiptPrefabs(List<ReceiptResponseDtoClass> receiptHistories)
    {
        int totalPrice = 0;
        foreach (ReceiptResponseDtoClass history in receiptHistories)
        {
            GameObject newReceipt = Instantiate(receiptPrefab, receiptContent);
            Text[] textElements = newReceipt.GetComponentsInChildren<Text>();
            textElements[0].text = history.item;
            textElements[1].text = intToStringWon(history.amount);
            totalPrice += history.amount;
        }

        totalPriceText.text = intToStringWon(totalPrice);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("ReceiptListScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("ReceiptListScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
