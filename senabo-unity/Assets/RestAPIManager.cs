using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;

public class RestAPIManager : MonoBehaviour
{
    // Replace with your API endpoint URL
    private string apiUrl = "https://dummy.restapiexample.com/api/v1/employees";

    private float requestDelay = 2f; // in seconds

    [Serializable]
    public class ApiData
    {
        public int id;
        public string employee_name;
        public int employee_salary;
        public int employee_age;
        public string profile_image;

    }

    void Start()
    {
        //(SendRequests());
        SendHttpClientRequest();
    }

    IEnumerator SendRequests()
    {
        while (true)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl);

            // Optionally, set headers or add other configuration
            Debug.Log(webRequest.result);

            yield return webRequest.SendWebRequest();

            // Request completed, check the result
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonText = webRequest.downloadHandler.text;
                Debug.Log(jsonText);

                yield break;
            }
            else
            {
                // Request failed, handle the error
                Debug.LogError("Error: " + webRequest.error);
            }

            // Clean up the web request object
            webRequest.Dispose();

            // Wait for the specified delay before making the next request
            yield return new WaitForSeconds(requestDelay);
        }
    }

    async void SendHttpClientRequest()
    {

        // Send GET request using HttpClient
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize JSON data
                    string jsonText = await response.Content.ReadAsStringAsync();
                    Debug.Log(jsonText);

                }
                else
                {
                    Debug.LogError("HTTP request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception during HTTP request: " + e.Message);
            }
        }
    }

}
