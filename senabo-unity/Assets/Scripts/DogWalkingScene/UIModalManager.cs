using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIModalManager : MonoBehaviour
{

    [SerializeField]
    GameObject WalkStartTipModal;

    [SerializeField]
    GameObject WalkSuddenEventModal;

    [SerializeField]
    Text WalkSuddenEventText;

    public enum ModalType
    {
        DetectingFloor,
        StartTip,
        SuddenEvent
    }

    public ModalType DetectingFloor => ModalType.DetectingFloor;
    public ModalType StartTip => ModalType.StartTip;
    public ModalType SuddenEvent => ModalType.SuddenEvent;

    /*���� 'Ȯ�� ��ư ������ ���� ���â �ݱ�'�� Ȯ�ι�ư�� �ν����Ϳ��� ���� ��.*/

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OpenModal(ModalType modalType)
    {
        VerifyModal(modalType).SetActive(true);
    }
    public void CloseModal(ModalType modalType)
    {
        VerifyModal(modalType).SetActive(false);
    }

    private GameObject VerifyModal(ModalType modalType)
    {
        switch (modalType)
        {
            case ModalType.StartTip:
                return WalkStartTipModal;
            case ModalType.SuddenEvent:
                return WalkSuddenEventModal;
            default:
                return null;
        }
    }

    public void WriteSuddenEventText(string text)
    {
        WalkSuddenEventText.text = text;
    }
}
