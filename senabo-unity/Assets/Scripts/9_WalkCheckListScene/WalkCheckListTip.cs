using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkCheckListTip : MonoBehaviour
{
    public GameObject WalkTip1ArrowIconRight;
    public GameObject WalkTip1ArrowIconDown;
    public GameObject WalkTip1BodyGroup;

    public GameObject WalkTip2ArrowIconRight;
    public GameObject WalkTip2ArrowIconDown;
    public GameObject WalkTip2BodyGroup;

    public GameObject WalkTip3ArrowIconRight;
    public GameObject WalkTip3ArrowIconDown;
    public GameObject WalkTip3BodyGroup;

    public GameObject WalkTip4ArrowIconRight;
    public GameObject WalkTip4ArrowIconDown;
    public GameObject WalkTip4BodyGroup;

    public GameObject WalkTip5ArrowIconRight;
    public GameObject WalkTip5ArrowIconDown;
    public GameObject WalkTip5BodyGroup;

    private bool walkTip1Flag;
    private bool walkTip2Flag;
    private bool walkTip3Flag;
    private bool walkTip4Flag;
    private bool walkTip5Flag;

    private void Start()
    {
        walkTip1Flag = false;
        WalkTip1ArrowIconRight.SetActive(true);
        WalkTip1ArrowIconDown.SetActive(false);
        WalkTip1BodyGroup.SetActive(false);

        walkTip2Flag = false;
        WalkTip2ArrowIconRight.SetActive(true);
        WalkTip2ArrowIconDown.SetActive(false);
        WalkTip2BodyGroup.SetActive(false);

        walkTip3Flag = false;
        WalkTip3ArrowIconRight.SetActive(true);
        WalkTip3ArrowIconDown.SetActive(false);
        WalkTip3BodyGroup.SetActive(false);

        walkTip4Flag = false;
        WalkTip4ArrowIconRight.SetActive(true);
        WalkTip4ArrowIconDown.SetActive(false);
        WalkTip4BodyGroup.SetActive(false);

        walkTip5Flag = false;
        WalkTip5ArrowIconRight.SetActive(true);
        WalkTip5ArrowIconDown.SetActive(false);
        WalkTip5BodyGroup.SetActive(false);
    }

    public void OnClickWalkTip1()
    {
        walkTip1Flag = !walkTip1Flag;
        WalkTip1ArrowIconRight.SetActive(!walkTip1Flag);
        WalkTip1ArrowIconDown.SetActive(walkTip1Flag);
        WalkTip1BodyGroup.SetActive(walkTip1Flag);
    }

    public void OnClickWalkTip2()
    {
        walkTip2Flag = !walkTip2Flag;
        WalkTip2ArrowIconRight.SetActive(!walkTip2Flag);
        WalkTip2ArrowIconDown.SetActive(walkTip2Flag);
        WalkTip2BodyGroup.SetActive(walkTip2Flag);
    }

    public void OnClickWalkTip3()
    {
        walkTip3Flag = !walkTip3Flag;
        WalkTip3ArrowIconRight.SetActive(!walkTip3Flag);
        WalkTip3ArrowIconDown.SetActive(walkTip3Flag);
        WalkTip3BodyGroup.SetActive(walkTip3Flag);
    }

    public void OnClickWalkTip4()
    {
        walkTip4Flag = !walkTip4Flag;
        WalkTip4ArrowIconRight.SetActive(!walkTip4Flag);
        WalkTip4ArrowIconDown.SetActive(walkTip4Flag);
        WalkTip4BodyGroup.SetActive(walkTip4Flag);
    }

    public void OnClickWalkTip5()
    {
        walkTip5Flag = !walkTip5Flag;
        WalkTip5ArrowIconRight.SetActive(!walkTip5Flag);
        WalkTip5ArrowIconDown.SetActive(walkTip5Flag);
        WalkTip5BodyGroup.SetActive(walkTip5Flag);
    }
}
