using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    public GameObject actionModal, LocationAlertModalPanel, EmerPoopModalPanel, EmerBiteModalPanel;
    public Text LocationAlertModalText;
    public Text MainTitleText;
    public GameObject dogImage, imageBedPoop, imageHurt, imageVomit;
    public Image imageBed, imageCarpet, imageFlower, imagePoopPad, imageWaterBowl, imageFoodBowl;
    public Sprite bedClean, bedDirty, carpetClean, carpetDirty, flowerClean, flowerDirty, poopPadClean, poopPadDirty,
                fullWaterBowl, emptyWaterBowl, fullFoodBowl, emptyFoodBowl;
    public bool isPoop = false, emerPoop = false, emerStomachache = false, emerAnxiety = false, emerDepression = false,
                emerCrush = false, emerBite = false, emerWalk = false, emerBarking = false, emerVomiting = false;
    private long[] emerIdArray;
    private Button dogBody;
    private bool actionable;

    [SerializeField]
    private GameObject LocationManagerObject;
    private MainLocationManager locationManager;

    void Start()
    {
        isPoop = false;
        emerPoop = false;
        emerStomachache = false;
        emerAnxiety = false;
        emerDepression = false;
        emerCrush = false;
        emerBite = false;
        emerWalk = false;
        emerBarking = false;
        emerVomiting = false;
        actionable = true;
        emerIdArray = new long[9];

        actionModal.SetActive(false);

        StartCoroutine(CheckIsPoop());
        StartCoroutine(CheckEmergency());

        LocationAlertModalText.text = $"{PlayerPrefs.GetString("dogName")}{GetGaVerb(PlayerPrefs.GetString("dogName"))}\n집에서 기다리고 있어요!";
        locationManager = LocationManagerObject.GetComponent<MainLocationManager>();
        SetTitleDayCount();
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

    string GetGaVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // 한글의 제일 처음과 끝의 범위 밖일 경우 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "가";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "이가" : "가";
    }

    public void OnClickEmerPoop()
    {
        if (emerPoop) // Never Used
        {
            StartCoroutine(CleanEmergency(emerIdArray[0]));
            emerIdArray[0] = 0;

            emerPoop = false;
            imageBedPoop.SetActive(false);
            EmerPoopModalPanel.SetActive(true);
            Invoke(nameof(CloseLocationAlertModal), 2.0f);
        }
    }

    public void OnClickEmerStomachache()
    {
        if (emerStomachache) // Never Used
        {
            StartCoroutine(CleanEmergency(emerIdArray[1]));
            emerIdArray[1] = 0;

            emerStomachache = false;
            imageHurt.SetActive(false);
            ReceiptScene.type = ReceiptType.HospitalCost3;
            StartCoroutine(UpdatePosition("MoveHospitalScene"));
        }
    }

    public void OnClickEmerAnxiety()
    {
        if (emerAnxiety)
        {
            StartCoroutine(CleanEmergency(emerIdArray[2]));
            emerIdArray[2] = 0;

            emerAnxiety = false;
            // Other Loading Scene is needed!!!
            ReceiptScene.type = ReceiptType.DamageCost1;
            StartCoroutine(UpdatePosition("ReceiptScene"));
        }
    }

    public void OnClickEmerDepression()
    {
        if (emerDepression)
        {
            StartCoroutine(CleanEmergency(emerIdArray[3]));
            emerIdArray[3] = 0;

            emerDepression = false;
            // Other Loading Scene is needed!!!
            ReceiptScene.type = ReceiptType.DamageCost2;
            StartCoroutine(UpdatePosition("ReceiptScene"));
        }
    }

    public void OnClickEmerCrush()
    {
        if (emerCrush)
        {
            StartCoroutine(CleanEmergency(emerIdArray[4]));
            emerIdArray[4] = 0;

            emerCrush = false;
            // Other Loading Scene is needed!!!
            ReceiptScene.type = ReceiptType.DamageCost3;
            StartCoroutine(UpdatePosition("ReceiptScene"));
        }
    }

    public void OnClickEmerVomit()
    {
        if (emerVomiting)
        {
            StartCoroutine(CleanEmergency(emerIdArray[8]));
            emerIdArray[8] = 0;

            emerVomiting = false;
            imageVomit.SetActive(false);
            ReceiptScene.type = ReceiptType.HospitalCost5;
            StartCoroutine(UpdatePosition("MoveHospitalScene"));
        }
    }

    void AfterJudgePoop()
    {
        Debug.Log("배변 여부: " + isPoop); // DEBUG CODE
        if (isPoop)
        {
            imagePoopPad.sprite = poopPadDirty;
        }
    }

    void AfterJudgeEmergency()
    {
        if (emerPoop) imageBedPoop.SetActive(true);
        else imageBedPoop.SetActive(false);

        if (emerStomachache)
        {
            imageHurt.SetActive(true);
            imageWaterBowl.sprite = emptyWaterBowl; // 물을 마심
            imageFoodBowl.sprite = fullFoodBowl; // 밥을 못 먹음
        }
        else
        {
            imageWaterBowl.sprite = emptyWaterBowl;
            imageFoodBowl.sprite = emptyFoodBowl;
        }

        if (emerAnxiety) // carpet, bed, flower random
        {
            imageCarpet.sprite = carpetDirty;
        }
        else
        {
            imageCarpet.sprite = carpetClean;
        }

        if (emerDepression)
        {
            imageBed.sprite = bedDirty;
        }
        else
        {
            imageBed.sprite = bedClean;
        }

        if (emerCrush)
        {
            imageFlower.sprite = flowerDirty;
        }
        else
        {
            imageFlower.sprite = flowerClean;
        }

        if (emerBite)
        {
            StartCoroutine(CleanEmergency(emerIdArray[5]));
            emerIdArray[5] = 0;

            EmerBiteModalPanel.SetActive(true);
            Invoke(nameof(CloseLocationAlertModal), 2.0f);
        }

        if (emerWalk)
        {
            Debug.Log("산책 호소"); // DEBUG CODE
        }

        if (emerBarking)
        {
            Debug.Log("큰 소리로 짖음"); // DEBUG CODE
        }

        if (emerVomiting)
        {
            imageVomit.SetActive(true);
            imageWaterBowl.sprite = fullWaterBowl; // 물을 못 마심
            imageFoodBowl.sprite = emptyFoodBowl; // 밥을 먹고 토를 함
        }
        else
        {
            imageVomit.SetActive(false);
        }

    }

    void SetBooleanValue(string type, bool state, long emerId)
    {
        Debug.Log(type + " 타입: " + state); // DEBUG CODE

        switch (type)
        {
            case "POOP":
                if (!state)
                {
                    emerPoop = !state;
                    emerIdArray[0] = emerId;
                }
                break;
            case "STOMACHACHE":
                if (!state)
                {
                    emerStomachache = !state;
                    emerIdArray[1] = emerId;
                }
                break;
            case "ANXIETY":
                if (!state)
                {
                    emerAnxiety = !state;
                    emerIdArray[2] = emerId;
                }
                break;
            case "DEPRESSION":
                if (!state)
                {
                    emerDepression = !state;
                    emerIdArray[3] = emerId;
                }
                emerDepression = !state; break;
            case "CRUSH":
                if (!state)
                {
                    emerCrush = !state;
                    emerIdArray[4] = emerId;
                }
                break;
            case "BITE":
                if (!state)
                {
                    emerBite = !state;
                    emerIdArray[5] = emerId;
                }
                break;
            case "WALK":
                if (!state)
                {
                    emerWalk = !state;
                    emerIdArray[6] = emerId;
                }
                break;
            case "BARKING":
                if (!state)
                {
                    emerBarking = !state;
                    emerIdArray[7] = emerId;
                }
                break;
            case "VOMITING":
                if (!state)
                {
                    emerVomiting = !state;
                    emerIdArray[8] = emerId;
                }
                break;
        }
    }

    IEnumerator CheckIsPoop()
    {
        string url = ServerSettings.SERVER_URL + "/api/feed/latest"; // TEST CODE

        UnityWebRequest www = UnityWebRequest.Get(url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        www.SetRequestHeader("Authorization", jwtToken);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log("MainScene CheckIsPoop Success"); // Debug Code

            FeedLatestDtoClass poop = JsonUtility.FromJson<APIResponse<FeedLatestDtoClass>>(www.downloadHandler.text).data;

            DateTime poopTime = Convert.ToDateTime(poop.createTime).AddHours(1);

            if (DateTime.Now >= poopTime && !poop.cleanYn)
            {
                isPoop = true;
            }
        }
        else
        {
            Debug.Log("MainScene CheckIsPoop Error:"); // Debug Code

            if (www.responseCode == 404)
            {
                isPoop = false;
            }
            else if (www.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();
                StartCoroutine(CheckIsPoop());
            }
            else if (www.responseCode != 404)
            {
                Debug.Log("CheckIsPoop - 이미 메인화면이기 때문에 메인화면으로 돌아가는 방식 사용 불가!"); // NEED HELP
                SceneManager.LoadScene("MainScene");
            }
        }

        Debug.Log("MainScene의 배변 여부: " + isPoop);// TEST
        AfterJudgePoop();
    }

    IEnumerator CheckEmergency()
    {
        string url = ServerSettings.SERVER_URL + "/api/emergency/get";

        UnityWebRequest response = UnityWebRequest.Get(url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log("MainScene CheckEmergency Success"); // Debug Code

            APIResponse<List<EmergencyDtoClass>> apiResponse
            = JsonUtility.FromJson<APIResponse<List<EmergencyDtoClass>>>(response.downloadHandler.text);

            foreach (var emer in apiResponse.data)
            {
                SetBooleanValue(emer.type, emer.solved, emer.id);
            }
        }
        else
        {
            Debug.Log("MainScene CheckEmergency Error"); // Debug Code

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();
                StartCoroutine(CheckEmergency());
            }
            else if (response.responseCode != 404)
            {
                Debug.Log("이미 메인화면이기 때문에 메인화면으로 돌아가는 방식 사용 불가!"); // NEED HELP
                SceneManager.LoadScene("MainScene");
            }
        }
        AfterJudgeEmergency();
    }

    IEnumerator CleanEmergency(long emerId)
    {
        // string url = ServerSettings.SERVER_URL + "/api/emergency/solve";
        string url = ServerSettings.SERVER_URL + $"/api/emergency/solve?id={emerId}";
        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";
        request.SetRequestHeader("Authorization", jwtToken);

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("MainScene CleanEmergency Success!");
        }
        else
        {
            Debug.Log("MainScene CleanEmergency Error! " + request.error);

            if (request.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(CleanEmergency(emerId));
            }
            else if (request.responseCode != 404)
            {
                Debug.Log("이미 메인화면이기 때문에 메인화면으로 돌아가는 방식 사용 불가!"); // NEED HELP
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    void SetTitleDayCount()
    {
        string createTime = PlayerPrefs.GetString("createTime");
        TimeSpan dateDiff = DateTime.Now - DateTime.Parse(createTime);
        MainTitleText.text = $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))} 함께한 지 {dateDiff.Days + 1}일 째 ";

        if (dateDiff.Days + 1 > 56)
        {
            actionable = false;
        }
    }

    public void LoadBathScene()
    {
        StartCoroutine(UpdatePosition("BathScene"));
    }

    public void LoadMealScene()
    {
        StartCoroutine(UpdatePosition("MealScene"));
    }

    public void LoadPoopScene()
    {
        StartCoroutine(UpdatePosition("PoopScene"));
    }

    public void LoadDogWalking2DScene()
    {
        if (emerWalk)
        {
            StartCoroutine(CleanEmergency(emerIdArray[6]));
            emerIdArray[6] = 0;

            emerWalk = false;
        }

        StartCoroutine(UpdatePosition("WalkCheckListScene"));
    }

    public void LoadMoveHospitalScene()
    {
        ReceiptScene.type = ReceiptType.HospitalCost1;
        StartCoroutine(UpdatePosition("MoveHospitalScene"));
    }

    public void LoadMoveGroomingScene()
    {
        ReceiptScene.type = ReceiptType.GroomingCost;
        StartCoroutine(UpdatePosition("MoveGroomingScene"));
    }

    public void LoadHeartScene()
    {
        StartCoroutine(UpdatePosition("HeartScene"));
    }

    public void LoadOwnerDictScene()
    {
        SceneManager.LoadScene("OwnerDictScene");
    }

    public void LoadProfileScene()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public void LoadSettingScene()
    {
        SceneManager.LoadScene("SettingScene");
    }

    void CloseLocationAlertModal()
    {
        LocationAlertModalPanel.SetActive(false);
        EmerPoopModalPanel.SetActive(false);
        EmerBiteModalPanel.SetActive(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("MainScene - 종료!!!!!\n입장 시간:" + PlayerPrefs.GetString("enterTime"));

            PlayerPrefs.SetString("exitTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));

            Debug.Log("퇴장 시간:" + PlayerPrefs.GetString("exitTime"));

            DateTime enterTime = DateTime.Parse(PlayerPrefs.GetString("enterTime"));
            DateTime exitTime = DateTime.Parse(PlayerPrefs.GetString("exitTime"));
            TimeSpan timeDiff = exitTime - enterTime;

            int diffMinute = timeDiff.Days * 24 * 60 + timeDiff.Hours * 60 + timeDiff.Minutes;

            Debug.Log("OnApplicationPause - diffMinute: " + diffMinute);
            RefreshTokenManager.Instance.UpdateTotalTime(diffMinute);
        }
        else
        {
            Debug.Log("RefreshTokenManager - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }

    // 현재 위치를 불러오는 메서드
    IEnumerator UpdatePosition(String sceneName)
    {
        LocationManagerObject.SetActive(true);
        while (true)
        {
            // 작업이 완료 되었을 때
            if (locationManager.getIsLocationManagerFinished())
            {
                if (locationManager.getIsAtHome())
                {
                    if (emerBarking)
                    {
                        StartCoroutine(CleanEmergency(emerIdArray[7]));
                        emerIdArray[7] = 0;
                        emerBarking = false;
                    }

                    Debug.Log("집(안) 입니다.");
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    Debug.Log("집(밖) 입니다.");

                    LocationAlertModalPanel.SetActive(true);
                    Invoke("CloseLocationAlertModal", 2.0f);
                }
                LocationManagerObject.SetActive(false);
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
    }
}
