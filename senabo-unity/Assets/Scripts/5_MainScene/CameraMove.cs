using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Vector3 Origin, Difference, ResetCamera;

    private bool drag = false;

    private float minX, minY, maxX, maxY;

    private void Start()
    {
        ResetCamera = Camera.main.transform.position;

        Vector3 bgSize = new(25, 20, 1);
        float cameraOrthographicSize = Camera.main.orthographicSize;

        minX = -bgSize.x / 2 + cameraOrthographicSize;
        maxX = bgSize.x / 2 - cameraOrthographicSize;
        minY = -bgSize.y / 2 + cameraOrthographicSize;
        maxY = bgSize.y / 2 - cameraOrthographicSize;

        // Debug.Log("cameraOrthographicSize: " + cameraOrthographicSize);
        // Debug.Log("minX: " + minX + ", maxX: " + maxX + ", minY: " + minY + ", maxY: " + maxY);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            Vector3 newPosition = Origin - Difference;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            Camera.main.transform.position = newPosition;
        }

        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = ResetCamera;
        }

        // Debug.Log("x: " + Camera.main.transform.position.x + ", y: " + Camera.main.transform.position.y);

    }


}
