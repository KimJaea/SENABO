using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemberInfo : MonoBehaviour
{
    public Text dogNameText;
    public Text emailText;

    public Text affectionIndexText;
    public Text stressIndexText;

    public Text totalExpenditureAmountText;

    public Image googleIcon;

    private void Awake()
    {        
        StartCoroutine(GetMemberInfo());
        StartCoroutine(GetTotalExpenditureAmount());
    }

    IEnumerator GetMemberInfo()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/get";
        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);
        response.SetRequestHeader("Authorization", jwtToken); ;

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            MemberClass member = JsonUtility.FromJson<APIResponse<MemberClass>>(response.downloadHandler.text).data;

            dogNameText.text = member.dogName;
            emailText.text = member.email;
            affectionIndexText.text = $"{member.affection}";
            stressIndexText.text = $"{member.stressLevel}";
        }
        else
        {
            Debug.Log("사용자 정보 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetMemberInfo());
            }
            else
            {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    IEnumerator GetTotalExpenditureAmount()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/expense/total";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            TotalExpenditureAmountClass totalExpenditureAmount = JsonUtility.FromJson<APIResponse<TotalExpenditureAmountClass>>(response.downloadHandler.text).data;

            if (totalExpenditureAmount.totalAmount == 0)
            {
                totalExpenditureAmountText.text = "0원";
            } else
            {
                totalExpenditureAmountText.text = string.Format("{0:#,###}원", totalExpenditureAmount.totalAmount);
            }
        }
        else
        {
            Debug.Log("총 금액 불러오기 실패");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetTotalExpenditureAmount());
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
            Debug.Log("ProfileScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("ProfileScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
