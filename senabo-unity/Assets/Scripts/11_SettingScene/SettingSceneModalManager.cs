using Google;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SettingSceneModalManager : MonoBehaviour
{
    public GameObject LogoutModalPanel;
    public GameObject DeleteAccountModalPanel;
    public GameObject LogoutAlertPanel;
    public GameObject DeleteAccountAlertPanel;
    public GameObject ChangeLocationModalPanel;

    void Start()
    {
        LogoutModalPanel.SetActive(false);
        DeleteAccountModalPanel.SetActive(false);
        LogoutAlertPanel.SetActive(false);
        DeleteAccountAlertPanel.SetActive(false);
        ChangeLocationModalPanel.SetActive(false);
    }

    public void OnClickLocationButton()
    {
        ChangeLocationModalPanel.SetActive(true);
    }

    public void OnClickLocationCancelButton()
    {
        ChangeLocationModalPanel.SetActive(false);
    }

    public void OnClickLogoutButton()
    {
        ShowLogoutModalPanel();
    }

    private void ShowLogoutModalPanel()
    {
        LogoutModalPanel.SetActive(true);
    }

    public void OnClickLogoutModalPanelCancelButton()
    {
        CloseLogoutModalPanel();
    }

    private void CloseLogoutModalPanel()
    {
        LogoutModalPanel.SetActive(false);
        ShowLogoutAlertPanel();
    }

    public void SignOutFromGoogle()
    {
        Debug.Log("로그아웃 1 - SignOutFromGoogle");
        OnSignOut();
    }

    private void OnSignOut()
    {
        Debug.Log("로그아웃 2 - OnSignOut");
        GoogleSignIn.DefaultInstance.SignOut();

        StartCoroutine(PostSignOut());
    }

    private void ShowLogoutAlertPanel()
    {
        LogoutAlertPanel.SetActive(true);
        Invoke("CloseLogoutAlertPanel", 2.0f);
    }

    private void CloseLogoutAlertPanel()
    {
        LogoutAlertPanel.SetActive(false);

        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LoginScene");
    }

    public void OnClickDeleteAccountButton()
    {
        ShowDeleteAccountModalPanel();
    }

    private void ShowDeleteAccountModalPanel()
    {
        DeleteAccountModalPanel.SetActive(true);
    }

    public void OnClickDeleteAccountModalPanelCancelButton()
    {
        CloseDeleteAccountModalPanel();
    }

    private void CloseDeleteAccountModalPanel()
    {
        DeleteAccountModalPanel.SetActive(false);
    }

    public void DisconnectFromGoogle()
    {
        Debug.Log("회원탈퇴 1 - DisconnectFromGoogle");
        OnDisconnect();
    }

    private void OnDisconnect()
    {
        Debug.Log("회원탈퇴 2 - OnDisconnect");
        GoogleSignIn.DefaultInstance.SignOut();

        StartCoroutine(DeleteRemove());
    }

    private void ShowDeleteAccountAlertPanel()
    {
        DeleteAccountAlertPanel.SetActive(true);
        Invoke("CloseDeleteAccountAlertPanel", 2.0f);
    }

    private void CloseDeleteAccountAlertPanel()
    {
        DeleteAccountAlertPanel.SetActive(false);
    }

    IEnumerator PostSignOut()
    {
        RefreshTokenRequestDtoClass refreshTokenRequestDto = new RefreshTokenRequestDtoClass();
        refreshTokenRequestDto.refreshToken = PlayerPrefs.GetString("refreshToken");
        
        string jsonFile = JsonUtility.ToJson(refreshTokenRequestDto);
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/sign-out";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(api_url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";

            request.SetRequestHeader("Authorization", jwtToken);

            yield return request.SendWebRequest();

            if (request.error == null)
            {
                Debug.Log("로그아웃 결과:" + request.downloadHandler.text);

                CloseLogoutModalPanel();
            }
            else
            {
                Debug.Log("로그아웃 실패");
            }
        }
    }

    IEnumerator DeleteRemove()
    {
        RefreshTokenRequestDtoClass refreshTokenRequestDto = new RefreshTokenRequestDtoClass();
        refreshTokenRequestDto.refreshToken = PlayerPrefs.GetString("refreshToken");

        string jsonFile = JsonUtility.ToJson(refreshTokenRequestDto);
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/remove";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(api_url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";

            request.SetRequestHeader("Authorization", jwtToken);

            yield return request.SendWebRequest();

            if (request.error == null)
            {
                Debug.Log("회원탈퇴 결과:" + request.downloadHandler.text);

                CloseDeleteAccountModalPanel();
                ShowDeleteAccountAlertPanel();

                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                Debug.Log("회원탈퇴 실패");
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("SettingScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
