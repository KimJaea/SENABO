using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dogObject;
    
    [SerializeField]
    private GameObject strapObject;

    [SerializeField]
    private GameObject aimPaw;


    Animator welshAnim;
    Animator strapAnim;

    void Start()
    {
        if (welshAnim == null)
            welshAnim = dogObject.GetComponentInChildren<Animator>();
        if (strapAnim == null && strapObject.activeInHierarchy) { }
            strapAnim = strapObject.GetComponentInChildren<Animator>();
    }

    public void handleDogMovement(string motion)
    {
        if (motion == "WelshIdle")
        {
            aimPaw.SetActive(false);
        }
        else
        {
            aimPaw.SetActive(true);
        }
        if (!EventStatusManager.GetDogEvent())
        {
            welshAnim.SetTrigger(motion);
            if (strapObject.activeInHierarchy)
            {
                strapAnim.SetTrigger(motion);
            }
        }
    }

    public void handleDogSuddenEvent(string motion)
    {
        welshAnim.SetTrigger(motion);
        
       if (strapObject.activeInHierarchy)
        {
            strapAnim.SetTrigger(motion);
        }
    }

    IEnumerator AnimationStatus()
    {
        int t = 0;
        while (t < 10)
        {
            Debug.Log(welshAnim.GetCurrentAnimatorStateInfo(0).loop);

            Debug.Log(welshAnim.GetCurrentAnimatorClipInfo(0)[0].clip.ToString());

            t++;

            yield return new WaitForSeconds(1);
        }
    }
}
