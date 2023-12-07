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

    // 간식 버튼 클릭 시
    public void HandleSnackItemClick()
    {
        if (!EventStatusManager.GetDogStopResolved()) //이벤트 진행 여부 조건문
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

    // 배변 봉투 버튼 클릭 시
    public void HandlePoopBagItemClick()
    {
        // poop이 활성화 상태일 때
        if (poop.activeInHierarchy)
        {
            arObjectController.setPoopEventTrigger();
        }
    }
}
