using Firebase;
using Firebase.Messaging;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignUp : MonoBehaviour
{
    public Text EmailText;
    public GameObject FailSignUpAlertModalPanel;
    public GameObject TextLengthOverModalPanel;
    public InputField inputField;

    void Awake()
    {
        TextLengthOverModalPanel.SetActive(false);
        FailSignUpAlertModalPanel.SetActive(false); 
    }

    void Start()
    {
        EmailText.text = FirebaseAuthManager.signUpRequestDto.email;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseAuthManager.signUpRequestDto.deviceToken = FirebaseMessaging.GetTokenAsync().Result.ToString();
            }
        });
    }

    void Update()
    {
        if (inputField.text.Length >= 4)
        {
            TextLengthOverModalPanel.SetActive(true);
            Invoke("CloseTextLengthOverModalPanel", 2.0f);
            inputField.text = inputField.text[..3];
        }
    }

    void CloseTextLengthOverModalPanel()
    {
        TextLengthOverModalPanel.SetActive(false);
    }

    public void OnAdoptButtonClick()
    {
        if (inputField.text.Length == 0 || inputField.text.Length > 3)
        {
            TextLengthOverModalPanel.SetActive(true);
            Invoke("CloseTextLengthOverModalPanel", 2.0f);
            inputField.text = inputField.text[..3];

            return;
        }

        FirebaseAuthManager.signUpRequestDto.dogName = inputField.text;
        StartCoroutine(PostMember());
    }

    void CloseFailSignUpAlertModalPanel()
    {
        FailSignUpAlertModalPanel.SetActive(false);
        SceneManager.LoadScene("SignUpScene");
    }

    IEnumerator PostMember()
    {
        Debug.Log("회원가입 PostMember:");

        FirebaseAuthManager.signUpRequestDto.species = "CORGI";
        FirebaseAuthManager.signUpRequestDto.sex = "F";

        string jsonFile = JsonUtility.ToJson(FirebaseAuthManager.signUpRequestDto);

        string api_url = $"{ServerSettings.SERVER_URL}/api/member/sign-up";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(api_url, jsonFile))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonFile);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.error == null)
            {
                Debug.Log(request.downloadHandler.text);
                SignUpResponseDtoClass signUpResponseDto = 
                    JsonUtility.FromJson<APIResponse<SignUpResponseDtoClass>>(request.downloadHandler.text).data;

                PlayerPrefs.SetString("dogName", signUpResponseDto.dogName);
                PlayerPrefs.SetString("email", signUpResponseDto.email);
                PlayerPrefs.SetFloat("houseLatitude", (float)signUpResponseDto.houseLatitude);
                PlayerPrefs.SetFloat("houseLongitude", (float)signUpResponseDto.houseLongitude);
                PlayerPrefs.SetString("accessToken", signUpResponseDto.token.accessToken);
                PlayerPrefs.SetString("refreshToken", signUpResponseDto.token.refreshToken);
                PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
                PlayerPrefs.SetString("createTime", signUpResponseDto.createTime);
                Debug.Log("입장시간: " + PlayerPrefs.GetString("enterTime"));
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                Debug.Log("회원가입 실패");
                PlayerPrefs.DeleteAll();
                FailSignUpAlertModalPanel.SetActive(true);
                Invoke("CloseFailSignUpAlertModalPanel", 2.0f);
            }
        }
    }
}   
