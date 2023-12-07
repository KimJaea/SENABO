using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ReceiptHistory
{
    public string item;
    public string detail;
    public int amount;
}

public class ReceiptType
{
    public const int InitialCost = 100;
    public const int EssentialCost = 200;
    public const int MonthlyCost = 300;
    public const int HospitalCost1 = 410;
    public const int HospitalCost2 = 420;
    public const int HospitalCost3 = 430;
    public const int HospitalCost4 = 440;
    public const int HospitalCost5 = 450;
    public const int GroomingCost = 500;
    public const int DamageCost1 = 610;
    public const int DamageCost2 = 620;
    public const int DamageCost3 = 630;
}

public class ReceiptScene : MonoBehaviour
{
    public static int type;
    public Text totalPriceText;
    public GameObject receiptPrefab;
    public Transform receiptContent;
    private List<ReceiptHistory> receiptHistories = new List<ReceiptHistory>();

    void Start()
    {
        SelectReceiptPrefabs();
    }

    IEnumerator Upload(ReceiptHistory history)
    {
        string url = ServerSettings.SERVER_URL + "/api/expense/save";
        string postJsonData = JsonUtility.ToJson(history);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(postJsonData);
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.uploadHandler = uploadHandler;
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("ReceiptScene Success! " + request.error);
        }
        else
        {
            Debug.Log("Error!");

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(Upload(history));
            }
            else
            {
                SceneManager.LoadScene("ProfileScene");
            }
        }
    }

    void SelectReceiptPrefabs()
    {
        switch (type)
        {
            case ReceiptType.InitialCost: // 초기 비용 13만 원
                receiptHistories.Add(new ReceiptHistory { item = "리드줄", detail = "", amount = 29000 });
                receiptHistories.Add(new ReceiptHistory { item = "입마개", detail = "", amount = 8000 });
                receiptHistories.Add(new ReceiptHistory { item = "방석", detail = "", amount = 25000 });
                receiptHistories.Add(new ReceiptHistory { item = "장난감", detail = "", amount = 5000 });
                receiptHistories.Add(new ReceiptHistory { item = "발톱깎이", detail = "", amount = 10000 });
                receiptHistories.Add(new ReceiptHistory { item = "이동장", detail = "", amount = 15000 });
                break;
            case ReceiptType.EssentialCost: // 필수 병원 비용 59만 원
                receiptHistories.Add(new ReceiptHistory { item = "DHPPL 종합 백신 1차 접종", detail = "", amount = 25000 });
                receiptHistories.Add(new ReceiptHistory { item = "코로나 장염 백신 1차 접종", detail = "", amount = 15000 });
                receiptHistories.Add(new ReceiptHistory { item = "DHPPL 종합 백신 2차 접종", detail = "", amount = 25000 });
                receiptHistories.Add(new ReceiptHistory { item = "코로나 장염 백신 2차 접종", detail = "", amount = 15000 });
                receiptHistories.Add(new ReceiptHistory { item = "DHPPL 종합 백신 3차 접종", detail = "", amount = 25000 });
                receiptHistories.Add(new ReceiptHistory { item = "컨넬 코프 백신 1차 접종", detail = "", amount = 15000 });
                receiptHistories.Add(new ReceiptHistory { item = "DHPPL 종합 백신 4차 접종", detail = "", amount = 25000 });
                receiptHistories.Add(new ReceiptHistory { item = "컨넬 코프 백신 2차 접종", detail = "", amount = 15000 });
                receiptHistories.Add(new ReceiptHistory { item = "DHPPL 종합 백신 5차 접종", detail = "", amount = 25000 });
                receiptHistories.Add(new ReceiptHistory { item = "신종 플루 백신 1차 접종", detail = "", amount = 30000 });
                receiptHistories.Add(new ReceiptHistory { item = "신종 플루 백신 2차 접종", detail = "", amount = 30000 });
                receiptHistories.Add(new ReceiptHistory { item = "광견병 예방 접종", detail = "", amount = 20000 });
                receiptHistories.Add(new ReceiptHistory { item = "중성화 수술 비용", detail = "", amount = 300000 });
                receiptHistories.Add(new ReceiptHistory { item = "심장사상충 약", detail = "", amount = 25000 });
                break;
            case ReceiptType.MonthlyCost: // 정기 비용 19만 8천 원
                receiptHistories.Add(new ReceiptHistory { item = "사료", detail = "", amount = 100000 });
                receiptHistories.Add(new ReceiptHistory { item = "간식", detail = "", amount = 30000 });
                receiptHistories.Add(new ReceiptHistory { item = "배변 패드", detail = "", amount = 12000 });
                receiptHistories.Add(new ReceiptHistory { item = "배변 봉투", detail = "", amount = 5000 });
                receiptHistories.Add(new ReceiptHistory { item = "샴푸", detail = "", amount = 20000 });
                receiptHistories.Add(new ReceiptHistory { item = "귀 세정제", detail = "", amount = 13000 });
                receiptHistories.Add(new ReceiptHistory { item = "치약 및 칫솔", detail = "", amount = 18000 });
                break;
            case ReceiptType.HospitalCost1: // 1. 단순 검진 비용
                receiptHistories.Add(new ReceiptHistory { item = "진료 비용", detail = "", amount = 80000 });
                break;
            case ReceiptType.HospitalCost2: // 2. 정기 검진 비용
                receiptHistories.Add(new ReceiptHistory { item = "검진 비용", detail = "", amount = 250000 });
                break;
            case ReceiptType.HospitalCost3: // 3. 잘못 먹은 음식
                receiptHistories.Add(new ReceiptHistory { item = "X-ray 방사선 촬영", detail = "", amount = 55000 });
                receiptHistories.Add(new ReceiptHistory { item = "구토 유발 주사", detail = "", amount = 55000 });
                break;
            case ReceiptType.HospitalCost4: // 4. 슬개골 탈구 치료 비용
                receiptHistories.Add(new ReceiptHistory { item = "수술 전 혈액검사", detail = "", amount = 33000 });
                receiptHistories.Add(new ReceiptHistory { item = "방사선 검사", detail = "", amount = 40000 });
                receiptHistories.Add(new ReceiptHistory { item = "슬개골 탈구 수술 비용", detail = "", amount = 600000 });
                receiptHistories.Add(new ReceiptHistory { item = "입원비", detail = "", amount = 50000 });
                break;
            case ReceiptType.HospitalCost5: // 5. 구토
                receiptHistories.Add(new ReceiptHistory { item = "진료 비용", detail = "", amount = 80000 });
                receiptHistories.Add(new ReceiptHistory { item = "검진 비용", detail = "", amount = 250000 });
                receiptHistories.Add(new ReceiptHistory { item = "토 방지 주사", detail = "", amount = 50000 });
                break;
            case ReceiptType.GroomingCost: // 미용 비용
                receiptHistories.Add(new ReceiptHistory { item = "미용비", detail = "", amount = 50000 });
                break;
            case ReceiptType.DamageCost1: // 훼손된 카펫 복구 비용
                receiptHistories.Add(new ReceiptHistory { item = "원형 러그", detail = "", amount = 40000 });
                break;
            case ReceiptType.DamageCost2: // 훼손된 침대 복구 비용
                receiptHistories.Add(new ReceiptHistory { item = "베개", detail = "", amount = 20000 });
                receiptHistories.Add(new ReceiptHistory { item = "침구", detail = "", amount = 50000 });
                break;
            case ReceiptType.DamageCost3: // 훼손된 화분 복구 비용
                receiptHistories.Add(new ReceiptHistory { item = "화분", detail = "", amount = 10000 });
                break;
        }

        CreateReceiptPrefabs();
    }

    string intToStringWon(int amount)
    {
        return amount.ToString("#,0") + "원";
    }

    void CreateReceiptPrefabs()
    {
        int totalPrice = 0;
        foreach (ReceiptHistory history in receiptHistories)
        {
            GameObject newReceipt = Instantiate(receiptPrefab, receiptContent);
            Text[] textElements = newReceipt.GetComponentsInChildren<Text>();
            textElements[0].text = history.item;
            textElements[1].text = intToStringWon(history.amount);
            totalPrice += history.amount;

            StartCoroutine(Upload(history));
        }

        totalPriceText.text = intToStringWon(totalPrice);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("ReceiptScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("ReceiptScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
