using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemSpawner;

public class UIItemActionManager : MonoBehaviour
{
    public GameObject goHomeButton;
    public GameObject itemPanel;
    public GameObject itemSpawner;

    [SerializeField]
    GameObject dogAnimationManager;

    DogAnimationManager dogAnimator;

    ItemSpawner itemSpawnerScript;

    [SerializeField]
    private GameObject poop;

    private bool isItemPanelOpen = false;

    [SerializeField]
    private ARObjectController arObjectController;

    void Start()
    {
        itemSpawnerScript = itemSpawner.GetComponent<ItemSpawner>();
    }
    public void SetIsItemPanelOpen(bool state)
    {
        itemPanel.SetActive(state);
        goHomeButton.SetActive(!state);
    }

    // ���� ��ư Ŭ�� ��
    public void HandleSnackItemClick()
    {
        if (!EventStatusManager.GetDogStopResolved()) //�̺�Ʈ ���� ���� ���ǹ�
        {
            if(dogAnimator == null)
            {
                dogAnimator = dogAnimationManager.GetComponent<DogAnimationManager>();
            }
            itemSpawnerScript.HandleSpawnAction(ItemType.Snack);
            EventStatusManager.SwitchDogStopResolved(true);
            dogAnimator.handleDogSuddenEvent("welshEat");
        }
    }

    // �躯 ���� ��ư Ŭ�� ��
    public void HandlePoopBagItemClick()
    {
        // poop�� Ȱ��ȭ ������ ��
        if (poop.activeInHierarchy)
        {
            arObjectController.setPoopEventTrigger();
        }
    }
}
