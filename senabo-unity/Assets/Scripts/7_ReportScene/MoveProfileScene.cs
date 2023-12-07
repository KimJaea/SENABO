using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveProfileScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("ProfileScene");
    }
}
