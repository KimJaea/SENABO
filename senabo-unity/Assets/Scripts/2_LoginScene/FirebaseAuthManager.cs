using Firebase;
using Firebase.Messaging;
using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    static public SignUpRequestDtoClass signUpRequestDto;

    private string deviceToken;

    private void Awake()
    {
        Debug.Log("***************파이어베이스 어스 매니저 시작**********");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
               deviceToken = FirebaseMessaging.GetTokenAsync().Result.ToString();
            }
        });

    }

    public void SignInWithGoogle() 
    {
        Debug.Log("***************파이어베이스 어스 매니저 SignInWithGoogle**********");
        OnSignIn(); 
    }

    public void SignOutFromGoogle()
    {
        Debug.Log("로그아웃 - SignOutFromGoogle");
        OnSignOut();
    }

    private void OnSignIn()
    {
        Debug.Log("***************파이어베이스 어스 매니저 OnSignIn**********");
        
        Debug.Log("Calling SignIn");
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
        Debug.Log("End OnSignIn");
    }

    private void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();

        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene("LoginScene");
    }

    public void OnDisconnect()
    {
        Debug.Log("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        Debug.Log("***************파이어베이스 어스 매니저 OnAuthenticationFinished**********");
        if (task.IsFaulted)
        {
            Debug.Log("isFaulted");
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception:" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            Debug.Log("success");
            Debug.Log("Email = " + task.Result.Email);
            Debug.Log("Google ID Token = " + task.Result.IdToken);
            Debug.Log("AuthCode:" + task.Result.AuthCode);

            string email = task.Result.Email;

            StartCoroutine(PostFirebaseAuth(email));
        }
    }

    IEnumerator PostFirebaseAuth(string email)
    {
        SignInRequestDtoClass signInRequestDtoClass = new SignInRequestDtoClass();
        signInRequestDtoClass.email = email;

        string jsonFile = JsonUtility.ToJson(signInRequestDtoClass);
        string api_url = $"{ServerSettings.SERVER_URL}/api/member/sign-in";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(api_url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log("결과!!:\n" + request.downloadHandler.text);

            if (request.error == null)
            {
                Debug.Log(request.downloadHandler.text);

                SignInResponseDtoClass signInResponse = JsonUtility.FromJson<APIResponse<SignInResponseDtoClass>>(request.downloadHandler.text).data;

                if (signInResponse.isMember) // 회원인 경우
                {
                    Debug.Log("회원인 경우");
                    PlayerPrefs.SetString("dogName", signInResponse.dogName);
                    PlayerPrefs.SetString("email", signInResponse.email);
                    PlayerPrefs.SetFloat("houseLatitude", (float)signInResponse.houseLatitude);
                    PlayerPrefs.SetFloat("houseLongitude", (float)signInResponse.houseLongitude);
                    PlayerPrefs.SetString("accessToken", signInResponse.token.accessToken);
                    PlayerPrefs.SetString("refreshToken", signInResponse.token.refreshToken);
                    PlayerPrefs.SetString("createTime", signInResponse.createTime);
                    PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));

                    Debug.Log("입장시간: " + PlayerPrefs.GetString("enterTime"));

                    StartCoroutine(UpdateDeviceToken());
                }
                else // 비회원인 경우
                {
                    Debug.Log("비회원인 경우");

                    // 비회원 alert

                    signUpRequestDto = new SignUpRequestDtoClass();
                    signUpRequestDto.email = email;

                    SceneManager.LoadScene("LocationSettingScene");
                }
            }
            else
            {
                Debug.Log("회원 여부 확인 실패");
                SignOutFromGoogle();
            }
        }
    }

    IEnumerator UpdateDeviceToken()
    {
        Debug.Log("디바이스 토큰 갱신 요청");

        Debug.Log("디바이스 토큰:" + deviceToken);

        UpdateDeviceTokenRequestDtoClass updateDeviceTokenRequestDto = new UpdateDeviceTokenRequestDtoClass();
        updateDeviceTokenRequestDto.deviceToken = deviceToken;

        string api_url = $"{ServerSettings.SERVER_URL}/api/member/device-token";
        string jsonFile = JsonUtility.ToJson(updateDeviceTokenRequestDto);

        using(UnityWebRequest request = UnityWebRequest.Put(api_url, jsonFile))
        {
            string accessToken = PlayerPrefs.GetString("accessToken");
            string jwtToken = $"Bearer {accessToken}";
            request.SetRequestHeader("Authorization", jwtToken);

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.error == null)
            {
                Debug.Log(request.downloadHandler.text);

                SceneManager.LoadScene("MainScene");
            }
            else
            {
                Debug.Log("디바이스 토큰 갱신 실패 :" + request.responseCode);
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene("LoginScene");
            }
        }
    }
}
