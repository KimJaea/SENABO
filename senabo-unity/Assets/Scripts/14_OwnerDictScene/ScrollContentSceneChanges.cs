using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollContentSceneChanges : MonoBehaviour
{
    public void SceneChangeYoung()
    {
        SceneManager.LoadScene("YoungPuppyOwnerDictDetailScene");

    }

    public void SceneChangeFood()
    {
        SceneManager.LoadScene("FoodFeedingOwnerDictDetailScene");

    }

    public void SceneChangeBath()
    {
        SceneManager.LoadScene("BathOwnerDictDetailScene");

    }

    public void SceneChangeWalking()
    {
        SceneManager.LoadScene("WalkingOwnerDictDetailScene");

    }

    public void SceneChangeSnack()
    {
        SceneManager.LoadScene("SnackTimeOwnerDictDetailScene");

    }

    public void SceneChangeHospital()
    {
        SceneManager.LoadScene("HospitalTimeOwnerDictDetailScene");

    }

    public void SceneChangeIshouldcare()
    {
        SceneManager.LoadScene("IShouldCareOwnerDictDetailScene");

    }

    public void SceneChangeEmergency()
    {
        SceneManager.LoadScene("EmergencyIssueOwnerDictDetailScene");

    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("OwnerDictScene - 종료!!!!!\n입장시간:" + PlayerPrefs.GetString("enterTime"));

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
            Debug.Log("OwnerDictScene - OnApplicationPause");
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
            Debug.Log("새로운 입장 시간:" + PlayerPrefs.GetString("enterTime"));
        }
    }
}
