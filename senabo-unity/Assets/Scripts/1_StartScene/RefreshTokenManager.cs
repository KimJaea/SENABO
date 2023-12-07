using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class RefreshTokenManager : MonoBehaviour
{
    private static RefreshTokenManager _instance;

    public static RefreshTokenManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("RefreshTokenManager").AddComponent<RefreshTokenManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    public void UpdateTotalTime(int totalTime)
    {
        Debug.Log("--시간 등록하기--");
        StartCoroutine(PostTotalTime(totalTime));
    }

    public void ReIssueRefreshToken()
    {
        Debug.Log("--ReIssueRefreshToken--");
        StartCoroutine(PostReIssue());
    }

    IEnumerator PostReIssue()
    {
        Debug.Log("리프레시 토큰 테스트");

        RefreshTokenRequestDtoClass refreshTokenRequestDto = new RefreshTokenRequestDtoClass();
        refreshTokenRequestDto.refreshToken = PlayerPrefs.GetString("refreshToken");

        string jsonFile = JsonUtility.ToJson(refreshTokenRequestDto);
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/reissue";

        using (UnityWebRequest response = UnityWebRequest.PostWwwForm(api_url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            response.uploadHandler = new UploadHandlerRaw(jsonToSend);
            response.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            response.SetRequestHeader("Content-Type", "application/json");

            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";

            response.SetRequestHeader("Authorization", jwtToken);

            yield return response.SendWebRequest();

            if (response.error == null)
            {
                Debug.Log("리프레시 토큰 재발급 결과:" + response.downloadHandler.text);

                RefreshTokenResponseDtoClass refreshTokenResponseDto =
                    JsonUtility.FromJson<APIResponse<RefreshTokenResponseDtoClass>>(response.downloadHandler.text).data;

                PlayerPrefs.SetString("accessToken", refreshTokenResponseDto.accessToken);
            }
            else
            {
                Debug.Log("리프레시 토큰 재발급 실패");

                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene("LoginScene");
            }
        }
    }

    IEnumerator PostTotalTime(int totalTime)
    {
        Debug.Log("RefreshTokenManager - 사용 시간 갱신");

        TotalTimeRequestDtoClass totalTimeRequestDto = new TotalTimeRequestDtoClass();
        totalTimeRequestDto.totalTime = totalTime;

        string jsonFile = JsonUtility.ToJson(totalTimeRequestDto);
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/total-time";

        using (UnityWebRequest response = UnityWebRequest.PostWwwForm(api_url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            response.uploadHandler = new UploadHandlerRaw(jsonToSend);
            response.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            response.SetRequestHeader("Content-Type", "application/json");

            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";

            response.SetRequestHeader("Authorization", jwtToken);

            yield return response.SendWebRequest();

            if (response.error == null)
            {
                Debug.Log("사용 시간 갱신:" + response.downloadHandler.text);
            }
            else
            {
                Debug.Log("사용 시간 갱신 실패");
            }
        }
    }
}
