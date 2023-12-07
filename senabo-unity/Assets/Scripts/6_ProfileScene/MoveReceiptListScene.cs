using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveReceiptListScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("ReceiptListScene");
    }
}
