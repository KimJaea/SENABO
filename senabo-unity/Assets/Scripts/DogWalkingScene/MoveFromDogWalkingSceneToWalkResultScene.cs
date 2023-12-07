using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveFromDogWalkingSceneToWalkResultScene : MonoBehaviour
{
    [SerializeField]
    public DogWalkingRestAPIManager restAPIManager;
    public void OnGoHomeButton()
    {
        restAPIManager.HandleWalkEnd();
        //SceneManager.LoadScene("WalkResultScene");
    }
}
