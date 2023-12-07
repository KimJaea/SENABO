using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLeadShaper : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField, Min(3)]
    private int lineSegments = 60;

    [SerializeField, Min(1)]
    private float timeOfTheFlight = 5;

    public void ShowTrajectoryLine(Vector3 startpoint, Vector3 startVelocity)
    {
        float timeStep = timeOfTheFlight / lineSegments;
        // [6:36] https://www.youtube.com/watch?v=U3hovyIWBLk
    }
}
