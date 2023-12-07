using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveFromProfileSettingSceneToLocationSettingScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("LocationSettingScene");
    }
}
