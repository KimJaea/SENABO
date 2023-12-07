using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorgiMovementControll : MonoBehaviour
{

    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        anim.SetFloat("movementspeed", 5); 
    }

    // Update is called once per frame
    void Update()
    {
        SwitchMotionByFingersNumber(); 
       
    }

    private void SwitchMotionByFingersNumber()
    {
        switch (Input.touchCount)
        {
            case 0:
                break;
            case 1:
                anim.SetTrigger("WelshWalkSlow");
                break;
            case 2:
                anim.SetTrigger("WelshIdle");
                break;
            case 3:
                anim.SetTrigger("WelshRun");
                break;
        }
    }
}
