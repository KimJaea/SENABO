using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveFromLocationSettingSceneToLoginScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
