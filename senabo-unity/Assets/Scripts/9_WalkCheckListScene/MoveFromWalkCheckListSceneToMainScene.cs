using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveFromWalkCheckListSceneToMainScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void SceneChangeToTip()
    {
        SceneManager.LoadScene("TipModalScene");

    }
}
