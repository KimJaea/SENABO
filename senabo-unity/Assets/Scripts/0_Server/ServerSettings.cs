using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class APIResponse<T>
{
    public string status;
    public string message;
    public T data;
}

public class ServerSettings : MonoBehaviour
{
    public static readonly string SERVER_URL = "https://www.senabo.co.kr";
}