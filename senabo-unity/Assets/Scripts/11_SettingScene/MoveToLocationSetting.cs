using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MoveToLocationSetting : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("LocationSettingScene");
    }
}
