using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class DogStroll : MonoBehaviour
{
    public GameObject dog, shadow;
    public Animator animator;
    private float timer = 2.0f;
    private float interval = 2.0f;
    private float sittedInterval = 1.4f;
    private float speed = 2.0f;
    private Vector3 moveDirection;
    private bool wasSitting = false;

    void Start()
    {
        dog.GetComponent<Collider2D>().enabled = true;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            bool randomValue = Random.Range(0, 2) == 0;
            if (randomValue)
            {
                SetStateWalk();
                Debug.Log("walk direction: " + moveDirection); // Debug Code
            }
            else
            {
                SetStateStop();
                Debug.Log("stop direction: " + moveDirection); // Debug Code
            }
            timer = 0.0f;
        }
        else
        {
            MoveDog();
            // Debug.Log("Current Position: " + dog.transform.position); // Debug Code
        }
    }

    void SetStateWalk()
    {
        wasSitting = Vector3.Equals(moveDirection, new Vector3(0.0f, 0.0f));
        moveDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        SetDogImageDirection();
        animator.SetFloat("Walking", 1);
        animator.SetFloat("Sitting", 0);
    }

    void SetStateStop()
    {
        moveDirection = new Vector3(0.0f, 0.0f).normalized;
        animator.SetFloat("Walking", 0);
        animator.SetFloat("Sitting", 1);
    }

    void SetDogImageDirection()
    {
        if (moveDirection.x >= 0) dog.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else dog.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
    }

    void MoveDog()
    {
        if (wasSitting && timer <= sittedInterval)
        {
            return;
        }

        dog.transform.position += speed * Time.deltaTime * moveDirection;
        shadow.transform.position += speed * Time.deltaTime * moveDirection;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Collider Entered!"); // Debug Code
        if (collider.gameObject.CompareTag("Barrier"))
        {
            Debug.Log("Barrier Collision!"); // Debug Code

            moveDirection = (new Vector3(0.0f, -5.0f) - dog.transform.position).normalized;
            moveDirection *= speed;
            SetDogImageDirection();
            timer = 0.5f;
        }
    }
}
