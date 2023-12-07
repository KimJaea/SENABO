using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class changePlaneColor : MonoBehaviour
{
    public ARPlane _ARPlane;
    public MeshRenderer _PlaneMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Color planematColor = Color.white;
        planematColor.a = 0.01f;
        _PlaneMeshRenderer.material.color = planematColor;
    }
}
