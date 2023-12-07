using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalManager : MonoBehaviour
{
    public GameObject PoopModalPanel;
    public GameObject FeedModalPanel;
    public GameObject WalkModalPanel;
    public GameObject CommunicationModalPanel;
    public GameObject HealthModalPanel;

    private void Start()
    {
        PoopModalPanel.SetActive(false);
        FeedModalPanel.SetActive(false);
        WalkModalPanel.SetActive(false);
        CommunicationModalPanel.SetActive(false);
        HealthModalPanel.SetActive(false);
    }

    public void ShowPoopModalPanel()
    {
        PoopModalPanel.SetActive(true);
    }

    public void ClosePoopModalPanel()
    {
        PoopModalPanel.SetActive(false);
    }

    public void ShowFeedModalPanell()
    {
        FeedModalPanel.SetActive(true);
    }

    public void CloseFeedModalPanel()
    {
        FeedModalPanel.SetActive(false);
    }

    public void ShowWalkModalPanel()
    {
        WalkModalPanel.SetActive(true);
    }

    public void CloseWalkModalPanel()
    {
        WalkModalPanel.SetActive(false);
    }

    public void ShowCommunicationModalPanel()
    {
        CommunicationModalPanel.SetActive(true);
    }

    public void CloseCommunicationModalPanel()
    {
        CommunicationModalPanel.SetActive(false);
    }

    public void ShowHealthModalPanel()
    {
        HealthModalPanel.SetActive(true);
    }

    public void CloseHealthModalPanel()
    {
        HealthModalPanel.SetActive(false);
    }
}
