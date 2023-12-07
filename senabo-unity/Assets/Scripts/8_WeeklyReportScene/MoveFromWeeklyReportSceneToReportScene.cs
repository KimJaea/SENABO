using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveFromWeeklyReportSceneToReportScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("ReportScene");
    }

}
