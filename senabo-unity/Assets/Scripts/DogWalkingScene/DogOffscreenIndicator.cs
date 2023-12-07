using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DogOffscreenIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject dogObject;

    [SerializeField]
    GameObject dogIndicator;

    [SerializeField]
    GameObject rotator;

    private Vector3 screenPoint;
    private Vector3 indicatorPosition;

    void Start()
    {
        dogIndicator.SetActive(false);
     }

    // Update is called once per frame
    void Update()
    {
        if (dogObject.activeInHierarchy)
        {
            screenPoint = Camera.main.WorldToViewportPoint(dogObject.transform.position);

            if (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1)
            {
                dogIndicator.SetActive(false);
            }
            else
            {
                if(!dogIndicator.activeInHierarchy) {
                    dogIndicator.SetActive(true);
                }
                Vector3 indicatorPosition = new Vector3(IndicatorPoint(screenPoint.x), IndicatorPoint(screenPoint.y), 0);
                dogIndicator.transform.position = Camera.main.ViewportToScreenPoint(indicatorPosition);
                
                Vector3 vectorToTarget = dogObject.transform.position - rotator.transform.position;
                vectorToTarget.z = 0;
                Vector3 quaternionToTarget = Quaternion.Euler(0, 0, 0) * vectorToTarget;

                Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: quaternionToTarget);

                rotator.transform.rotation = Quaternion.RotateTowards(rotator.transform.rotation, rot, 1000 * Time.deltaTime);
            }
        }
    }

    private float IndicatorPoint(float point)
    {
        if (point < 0.1) return 0.1f;
        else if (point > 0.9) return 0.9f;
        else return point;
    }
}
