using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogLeadDrawer : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lineRenderer;
    Vector3 dogPos, cameraPos;

    [SerializeField]
    GameObject dogObject;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = .02f;
        lineRenderer.endWidth = .02f;
    }

    void Update() {
        dogPos = dogObject.GetComponent<Transform>().position;
        cameraPos = Camera.current.ViewportToWorldPoint(new Vector3(-5f, -1f, Camera.current.nearClipPlane - 1f));
        lineRenderer.SetPosition(0, dogPos);
        lineRenderer.SetPosition(1, cameraPos);
    }

}
