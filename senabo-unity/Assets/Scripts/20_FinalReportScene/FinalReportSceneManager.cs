using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalReportSceneManager : MonoBehaviour
{
    public Text DateText;
    public Text DogNameText;
    public Text WithText;

    public Text RecordHeaderText;

    public GameObject GoodResultText;
    public GameObject BadResultText;

    public Text AffectionIndexText;
    public Text StressIndexText;

    public GameObject GoodResultMentText;
    public GameObject BadResultMentText;

    public Text CommunicationBodyText;
    public Text FeedBodyText;
    public Text WalkBodyText;
    public Text PoopBodyText;

    public GameObject HealthBadGroup;
    public GameObject HealthGoodGroup;
    public Text HealthGoodText;
    public Text HealthBadText;

    public Text AffectionText;
    public Text StressText;

    public GameObject WalkGoodCommentText;
    public GameObject WalkBadCommentText;
    public GameObject CommunicationGoodCommentText;
    public GameObject CommunicationBadCommentText;
    public GameObject PoopGoodCommentText;
    public GameObject PoopBadCommentText;
    public GameObject FeedGoodCommentText;
    public GameObject FeedBadCommentText;
    public GameObject HealthBadCommentText;
    public GameObject HealthGoodCommentText;
    public GameObject StressBadCommentText;
    public GameObject StressGoodCommentText;

    public GameObject AffectionEmptyGroup;
    public GameObject AffectionScrollView;

    public GameObject StressEmptyGroup;
    public GameObject StressScrollView;

    public GameObject StressModalPanel;
    public GameObject AffectionModalPanel;
    public GameObject PoopModalPanel;
    public GameObject FeedModalPanel;
    public GameObject WalkModalPanel;
    public GameObject CommunicationModalPanel;

    private List<AffectionResponseDtoClass> affections;
    private List<AffectionResponseDtoClass> stresses;
    private List<WalkClass> walks;
    private List<CommunicationClass> communications;
    private List<PoopClass> poops;
    private List<FeedClass> feeds;
    private List<BathResponseDtoClass> baths;
    private List<BathResponseDtoClass> brushingTeeths;

    private Vector3 scale = new Vector3(1f, 1f, 1f);

    // Start is called before the first frame update
    void Awake()
    {
        StressModalPanel.SetActive(false);
        AffectionModalPanel.SetActive(false);
        PoopModalPanel.SetActive(false);
        FeedModalPanel.SetActive(false);
        WalkModalPanel.SetActive(false);
        CommunicationModalPanel.SetActive(false);

        DateText.text = $"{DateTime.Parse(PlayerPrefs.GetString("createTime")):yyyy.MM.dd} - {DateTime.Parse(PlayerPrefs.GetString("createTime")).AddDays(55):yyyy.MM.dd}";
        DogNameText.text = PlayerPrefs.GetString("dogName");
        WithText.text = GetVerb(PlayerPrefs.GetString("dogName"));

        RecordHeaderText.text = $"{PlayerPrefs.GetString("dogName")}{GetVerb(PlayerPrefs.GetString("dogName"))}\n8�� ���� ����";

        GoodResultText.SetActive(false);
        BadResultText.SetActive(false);

        GoodResultMentText.SetActive(false);
        BadResultMentText.SetActive(false);
        
        HealthGoodGroup.SetActive(true);
        HealthBadGroup.SetActive(false);
        
        HealthGoodText.text = $"{PlayerPrefs.GetString("dogName")}{GetEuiVerb(PlayerPrefs.GetString("dogName"))} û����\n�� �����߾��.";
        HealthBadText.text = $"{PlayerPrefs.GetString("dogName")}{GetEuiVerb(PlayerPrefs.GetString("dogName"))} û����\n�� �������� ���߾��.";

        WalkGoodCommentText.SetActive(true);
        WalkBadCommentText.SetActive(true);

        CommunicationGoodCommentText.SetActive(true);
        CommunicationBadCommentText.SetActive(false);

        PoopGoodCommentText.SetActive(true);
        PoopBadCommentText.SetActive(false);

        FeedGoodCommentText.SetActive(true);
        FeedBadCommentText.SetActive(false);

        HealthBadCommentText.SetActive(false);
        HealthGoodCommentText.SetActive(true);

        StressBadCommentText.SetActive(false);
        StressGoodCommentText.SetActive(false);

        AffectionEmptyGroup.SetActive(false);
        AffectionScrollView.SetActive(false);

        StressEmptyGroup.SetActive(false);
        StressScrollView.SetActive(false);

        StartCoroutine(GetMemberInfo());
        StartCoroutine(GetAffectionList());
        StartCoroutine(GetStressList());
        StartCoroutine(GetWalkList());
        StartCoroutine(GetCommunicationList());
        StartCoroutine(GetPoopList());
        StartCoroutine(GetFeedList());
        StartCoroutine(GetBathList());
        StartCoroutine(GetBrushingTeethList());
    }

    IEnumerator GetMemberInfo()
    {
        Debug.Log("ȸ�� ���� �ҷ����� ����");

        string api_url = $"{ServerSettings.SERVER_URL}/api/member/get";
        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        UnityWebRequest response = UnityWebRequest.Get(api_url);
        response.SetRequestHeader("Authorization", jwtToken); ;

        yield return response.SendWebRequest();

        Debug.Log("���� �ڵ�:" + response.responseCode);

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            MemberClass member = JsonUtility.FromJson<APIResponse<MemberClass>>(response.downloadHandler.text).data;

            AffectionIndexText.text = member.affection.ToString();
            StressIndexText.text = member.stressLevel.ToString();

            if (member.stressLevel > 20)
            {
                StressBadCommentText.SetActive(true);
                StressGoodCommentText.SetActive(false);
            } else if (member.stressLevel <= 20)
            {
                StressBadCommentText.SetActive(true);
                StressGoodCommentText.SetActive(false);
            }

            if (member.affection >= 80 && member.stressLevel <= 20)
            {
                GoodResultText.SetActive(true);
                BadResultText.SetActive(false);

                GoodResultMentText.SetActive(true);
                BadResultMentText.SetActive(false);
            }
            else
            {
                GoodResultText.SetActive(false);
                BadResultText.SetActive(true);

                GoodResultMentText.SetActive(false);
                BadResultMentText.SetActive(true);
            }
        }
        else
        {
            Debug.Log("����� ���� �ҷ����� ����");

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetMemberInfo());
            }
            else
            {
                SceneManager.LoadScene("ProfileScene");
            }
        }
    }

    public GameObject DayAffectionElement;
    public GameObject AffectionRecordElemnt;
    public GameObject AffectionContent;

    IEnumerator GetAffectionList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/affection/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            affections =
                JsonUtility.FromJson<APIResponse<List<AffectionResponseDtoClass>>>(response.downloadHandler.text).data;

            if (affections.Count == 0)
            {
                AffectionEmptyGroup.SetActive(true);
                AffectionScrollView.SetActive(false);

                AffectionText.text = $"���� ���� �϶���\n<b>0ȸ</b> �־����.";
            } 
            else
            {
                AffectionEmptyGroup.SetActive(false);
                AffectionScrollView.SetActive(true);

                List<List<AffectionResponseDtoClass>> affectionsByCreateTime = new List<List<AffectionResponseDtoClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int decreasingCount = 0;
                int index = -1;

                for (int i = 0; i < affections.Count; i++)
                {
                    if (affections[i].changeAmount < 0)
                    {
                        decreasingCount++;
                    }

                    AffectionResponseDtoClass affection = affections[i];
                    DateTime affectionDate = Convert.ToDateTime(DateTime.Parse(affection.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, affectionDate) != 0)
                    {
                        date = affectionDate;
                        affectionsByCreateTime.Add(new List<AffectionResponseDtoClass>());
                        index++;
                    }

                    affectionsByCreateTime[index].Add(affection);
                }

                AffectionText.text = $"���� ���� �϶���\n<b>{decreasingCount}ȸ</b> �־����.";

                for (int i = 0; i < affectionsByCreateTime.Count; i++)
                {
                    GameObject dayAffectionElement = Instantiate(DayAffectionElement);
                    dayAffectionElement.name = $"{DateTime.Parse(affectionsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayAffectionElement.transform.SetParent(AffectionContent.transform);
                    dayAffectionElement.transform.localScale = scale;

                    dayAffectionElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(affectionsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // ��¥ ����

                    for (int j = 0; j < affectionsByCreateTime[i].Count; j++)
                    {
                        AffectionResponseDtoClass affection = affectionsByCreateTime[i][j];

                        GameObject affectionRecordElemnt = Instantiate(AffectionRecordElemnt);
                        affectionRecordElemnt.transform.SetParent(dayAffectionElement.transform.GetChild(1).transform);
                        affectionRecordElemnt.transform.localScale = scale;

                        affectionRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(affection.createTime):HH:mm}";
                        affectionRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetCommunicationTypeString(affection.type, affection.changeAmount)}";
                    }
                }
            }
        }
        else
        {
            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetAffectionList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    public GameObject DayStressElement;
    public GameObject StressRecordElemnt;
    public GameObject StressContent;

    private string GetStressTypeString(string type, int changeAmount)
    {
        string value = "";

        switch (type)
        {
            case "POOP":
                value = "�躯 Ȱ���� �Ǽ��߾��.";
                break;
            case "STOMACHACHE":
                value = "�����̳� �̹����� ������ ��Ż�� �����.";
                break;
            case "ANXIETY":
                value = "�ٴ��� ���� �Ǵ� ���׸� �� �������.";
                break;
            case "DEPRESSION":
                value = "��Ʈ������ �޾� �Ļ縦 �ź��߾��.";
                break;
            case "CRUSH":
                value = "��å�� ���� ������ ������ ���� ������.";
                break;
            case "BITE":
                value = "���� ��� �߻��߾��.";
                break;
            case "BARKING":
                value = "�и� �Ҿ����� ���� ¢�����.";
                break;
            case "VOMITING":
                value = "���� �� ������ �߻��߾��.";
                break;
            case "BRUSHING_TEETH":
                value = "��ġ�� ���� ���� �ʾ� ��ġ�� ������.";
                break;
            case "FEED":
                value = "���� ���� ���� �ʾ� ��Ʈ������ �޾Ҿ��.";
                break;
            case "BATH":
                value = "����� ���� �ʾ� �ǰ��� ��ȭ�ƾ��.";
                break;
            case "WALK":
                if (changeAmount > 0)
                {
                    value = "��å�� ���� ��Ʈ������ �޾Ҿ��.";
                }
                else
                {
                    value = "��å�� �ϸ� ��Ʈ������ ������.";
                }
                break;
            case "COMMUNICATION":
                value = "������ �ϸ� ��Ʈ������ ������.";
                break;
        }

        return value;
    }

    IEnumerator GetStressList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/stress/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            stresses =
                JsonUtility.FromJson<APIResponse<List<AffectionResponseDtoClass>>>(response.downloadHandler.text).data;

            if (stresses.Count == 0)
            {
                StressEmptyGroup.SetActive(true);
                StressScrollView.SetActive(false);
                StressText.text = $"��Ʈ���� ���� �����\n<b>0ȸ</b> �־����!";
            }
            else
            {
                StressEmptyGroup.SetActive(false);
                StressScrollView.SetActive(true);

                List<List<AffectionResponseDtoClass>> stressesByCreateTime = new List<List<AffectionResponseDtoClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int increasingCount = 0;
                int index = -1;

                for (int i = 0; i < stresses.Count; i++)
                {
                    if (stresses[i].changeAmount > 0)
                    {
                        increasingCount++;
                    }

                    AffectionResponseDtoClass stress = stresses[i];
                    DateTime stressDate = Convert.ToDateTime(DateTime.Parse(stress.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, stressDate) != 0)
                    {
                        date = stressDate;
                        stressesByCreateTime.Add(new List<AffectionResponseDtoClass>());
                        index++;
                    }

                    stressesByCreateTime[index].Add(stress);
                }

                StressText.text = $"��Ʈ���� ���� �����\n<b>{increasingCount}ȸ</b> �־����!";

                for (int i = 0; i < stressesByCreateTime.Count; i++)
                {
                    GameObject dayStressElement = Instantiate(DayStressElement);
                    dayStressElement.name = $"{DateTime.Parse(stressesByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayStressElement.transform.SetParent(StressContent.transform);
                    dayStressElement.transform.localScale = scale;

                    dayStressElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text 
                        = $"{DateTime.Parse(stressesByCreateTime[i][0].createTime):yyyy.MM.dd}"; // ��¥ ����

                    for (int j = 0; j < stressesByCreateTime[i].Count; j++)
                    {
                        AffectionResponseDtoClass stress = stressesByCreateTime[i][j];

                        GameObject stressRecordElemnt = Instantiate(StressRecordElemnt);
                        stressRecordElemnt.transform.SetParent(dayStressElement.transform.GetChild(1).transform);
                        stressRecordElemnt.transform.localScale = scale;

                        stressRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(stress.createTime):HH:mm}";
                        stressRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetStressTypeString(stress.type, stress.changeAmount)}";
                    }
                }
            }
        }
        else
        {
            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetStressList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    public GameObject DayWalkElement;
    public GameObject WalkRecordElemnt;
    public GameObject WalkContent;

    public GameObject WalkEmptyGroup;
    public GameObject WalkScrollView;


    IEnumerator GetWalkList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/walk/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            walks = JsonUtility.FromJson<APIResponse<List<WalkClass>>>(response.downloadHandler.text).data;

            if (walks.Count == 0)
            {
                WalkScrollView.SetActive(false);
                WalkEmptyGroup.SetActive(true);

                WalkBodyText.text = "0km";

                WalkGoodCommentText.SetActive(false);
                WalkBadCommentText.SetActive(true);
            } else
            {
                WalkScrollView.SetActive(true);
                WalkEmptyGroup.SetActive(false);

                List<List<WalkClass>> walksByCreateTime = new List<List<WalkClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                double totalDistance = 0;

                for (int i = 0; i < walks.Count; i++)
                {
                    totalDistance += walks[i].distance;

                    WalkClass walk = walks[i];

                    DateTime walkDate = Convert.ToDateTime(DateTime.Parse(walk.startTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, walkDate) != 0)
                    {
                        date = walkDate;
                        walksByCreateTime.Add(new List<WalkClass>());
                        index++;
                    }

                    walksByCreateTime[index].Add(walk);
                }

                WalkBodyText.text = $"{Math.Round(totalDistance, 2)}km";

                if (totalDistance < 120)
                {
                    WalkGoodCommentText.SetActive(false);
                    WalkBadCommentText.SetActive(true);
                }

                for (int i = 0; i < walksByCreateTime.Count; i++)
                {
                    GameObject dayWalkElement = Instantiate(DayWalkElement);
                    dayWalkElement.name = $"{DateTime.Parse(walksByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayWalkElement.transform.SetParent(WalkContent.transform);
                    dayWalkElement.transform.localScale = scale;

                    dayWalkElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(walksByCreateTime[i][0].startTime):yyyy.MM.dd}"; // ��¥ ����

                    for (int j = 0; j < walksByCreateTime[i].Count; j++)
                    {
                        WalkClass walk = walksByCreateTime[i][j];

                        GameObject walkRecordElemnt = Instantiate(WalkRecordElemnt);
                        walkRecordElemnt.transform.SetParent(dayWalkElement.transform.GetChild(1).transform);
                        walkRecordElemnt.transform.localScale = scale;

                        walkRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(walk.createTime):HH:mm}";
                        walkRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetTimeFormat(walk.startTime, walk.endTime)} ����\n{Math.Round(walk.distance,2)}km�� �ɾ����.";
                    }
                }
            }
        }
        else
        {
            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetWalkList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    private string GetCommunicationTypeString(string type, int changeAmount)
    {
        string value = "";

        switch (type)
        {
            case "WAIT":
                value = "��ٷ� �Ʒ��� �߾��.";
                break;
            case "SIT":
                value = "�ɾ� �Ʒ��� �߾��.";
                break;
            case "HAND":
                value = "�� �Ʒ��� �߾��.";
                break;
            case "PAT":
                value = "���ٵ�⸦ �߾��.";
                break;
            case "TUG":
                value = "�ͱ� ���̸� �߾��.";
                break;
            case "WALK":
                if (changeAmount > 0)
                    value = "��å�� �߾��.";
                else
                    value = "��å�� ���� �ʾ� ���� ������ ���������.";
                break;
        }

        return value;
    }

    public GameObject DayCommunicationElement;
    public GameObject CommunicationRecordElemnt;
    public GameObject CommunicationContent;

    public GameObject CommunicationEmptyGroup;
    public GameObject CommunicationScrollView;

    IEnumerator GetCommunicationList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/communication/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            Debug.Log(response.downloadHandler.text);

            communications = JsonUtility.FromJson<APIResponse<List<CommunicationClass>>>(response.downloadHandler.text).data;
            CommunicationBodyText.text = $"{communications.Count}ȸ";

            if (communications.Count < 280)
            {
                CommunicationGoodCommentText.SetActive(false);
                CommunicationBadCommentText.SetActive(true);
            }

            if (communications.Count == 0)
            {
                CommunicationScrollView.SetActive(false);
                CommunicationEmptyGroup.SetActive(true);
            }
            else
            {
                CommunicationScrollView.SetActive(true);
                CommunicationEmptyGroup.SetActive(false);

                List<List<CommunicationClass>> communicationsByCreateTime = new List<List<CommunicationClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                for (int i = 0; i < communications.Count; i++)
                {
                    CommunicationClass communication = communications[i];

                    DateTime communicationDate = Convert.ToDateTime(DateTime.Parse(communication.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, communicationDate) != 0)
                    {
                        date = communicationDate;
                        communicationsByCreateTime.Add(new List<CommunicationClass>());
                        index++;
                    }

                    communicationsByCreateTime[index].Add(communication);
                }

                for (int i = 0; i < communicationsByCreateTime.Count; i++)
                {
                    GameObject dayCommunicationElement = Instantiate(DayCommunicationElement);
                    dayCommunicationElement.name = $"{DateTime.Parse(communicationsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayCommunicationElement.transform.SetParent(CommunicationContent.transform);
                    dayCommunicationElement.transform.localScale = scale;

                    dayCommunicationElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(communicationsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // ��¥ ����

                    for (int j = 0; j < communicationsByCreateTime[i].Count; j++)
                    {
                        CommunicationClass communication = communicationsByCreateTime[i][j];

                        GameObject communicationRecordElemnt = Instantiate(CommunicationRecordElemnt);
                        communicationRecordElemnt.transform.SetParent(dayCommunicationElement.transform.GetChild(1).transform);
                        communicationRecordElemnt.transform.localScale = scale;

                        communicationRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(communication.createTime):HH:mm}";
                        communicationRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{GetCommunicationTypeString(communication.type, 5)}";
                    }
                }
            }
        }
        else
        {
            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetCommunicationList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    public GameObject DayPoopElement;
    public GameObject PoopRecordElemnt;
    public GameObject PoopContent;

    public GameObject PoopEmptyGroup;
    public GameObject PoopScrollView;

    IEnumerator GetPoopList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/feed/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        { 
            poops = JsonUtility.FromJson<APIResponse<List<PoopClass>>>(response.downloadHandler.text).data;

            if (poops.Count == 0)
            {
                PoopScrollView.SetActive(false);
                PoopEmptyGroup.SetActive(true);

                PoopBodyText.text = "-";

                PoopGoodCommentText.SetActive(false);
                PoopBadCommentText.SetActive(true);
            } 
            else
            {
                PoopScrollView.SetActive(true);
                PoopEmptyGroup.SetActive(false);

                List<List<PoopClass>> poopsByCreateTime = new List<List<PoopClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                for (int i = 0; i < poops.Count; i++)
                {
                    PoopClass poop = poops[i];

                    if (Convert.ToDateTime(poop.createTime).AddHours(1) > DateTime.Now)
                    {
                        continue;
                    }

                    DateTime poopDate = Convert.ToDateTime(DateTime.Parse(poop.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, poopDate) != 0)
                    {
                        date = poopDate;
                        poopsByCreateTime.Add(new List<PoopClass>());
                        index++;
                    }

                    poopsByCreateTime[index].Add(poop);
                }

                if (poopsByCreateTime.Count == 0)
                {
                    PoopScrollView.SetActive(false);
                    PoopEmptyGroup.SetActive(true);

                    PoopBodyText.text = "-";

                    PoopGoodCommentText.SetActive(false);
                    PoopBadCommentText.SetActive(true);
                } 
                else
                {
                    int totalTimeMinute = 0;

                    for (int i = 0; i < poopsByCreateTime.Count; i++)
                    {
                        GameObject dayPoopElement = Instantiate(DayPoopElement);
                        dayPoopElement.name = $"{DateTime.Parse(poopsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                        dayPoopElement.transform.SetParent(PoopContent.transform);
                        dayPoopElement.transform.localScale = scale;

                        dayPoopElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(poopsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // ��¥ ����

                        for (int j = 0; j < poopsByCreateTime[i].Count; j++)
                        {
                            PoopClass poop = poopsByCreateTime[i][j];

                            GameObject poopRecordElemnt = Instantiate(PoopRecordElemnt);
                            poopRecordElemnt.transform.SetParent(dayPoopElement.transform.GetChild(1).transform);
                            poopRecordElemnt.transform.localScale = scale;

                            poopRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(poop.createTime).AddHours(1):HH:mm}";

                            if (poop.cleanYn)
                            {
                                poopRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = GetDiffTimeString(DateTime.Parse(poop.createTime).AddHours(1).ToString(), poop.updateTime);
                                totalTimeMinute += GetTimeDiff(DateTime.Parse(poop.createTime).AddHours(1).ToString(), poop.updateTime);
                            }
                            else
                            {
                                poopRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "���� ġ���� �ʾҾ��!";
                                totalTimeMinute += GetTimeDiff(DateTime.Parse(poop.createTime).AddHours(1).ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                    }

                    PoopBodyText.text = $"{GetTotalTimeString(totalTimeMinute / poops.Count)}";

                    if (totalTimeMinute / poops.Count > 30)
                    {
                        PoopGoodCommentText.SetActive(false);
                        PoopBadCommentText.SetActive(true);
                    }
                    else
                    {
                        PoopGoodCommentText.SetActive(true);
                        PoopBadCommentText.SetActive(false);
                    }
                    
                }
            }
        }
        else
        {
            Debug.Log("��ü �躯 ���� �ҷ����� ���� - �ڵ�:" + response.responseCode);

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetPoopList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    public GameObject DayFeedElement;
    public GameObject FeedRecordElemnt;
    public GameObject FeedContent;

    public GameObject FeedEmptyGroup;
    public GameObject FeedScrollView;

    IEnumerator GetFeedList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/feed/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            feeds = JsonUtility.FromJson<APIResponse<List<FeedClass>>>(response.downloadHandler.text).data;
            FeedBodyText.text = $"{feeds.Count}ȸ";

            if (feeds.Count < 100)
            {
                FeedBadCommentText.SetActive(true);
                FeedGoodCommentText.SetActive(false);
            }

            if (feeds.Count == 0)
            {
                FeedScrollView.SetActive(false);
                FeedEmptyGroup.SetActive(true);
            }
            else
            {
                FeedScrollView.SetActive(true);
                FeedEmptyGroup.SetActive(false);

                List<List<FeedClass>> feedsByCreateTime = new List<List<FeedClass>>();
                DateTime date = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy.MM.dd"));

                int index = -1;
                for (int i = 0; i < feeds.Count; i++)
                {
                    FeedClass feed = feeds[i];

                    DateTime feedDate = Convert.ToDateTime(DateTime.Parse(feed.createTime).ToString("yyyy.MM.dd"));

                    if (DateTime.Compare(date, feedDate) != 0)
                    {
                        date = feedDate;
                        feedsByCreateTime.Add(new List<FeedClass>());
                        index++;
                    }

                    feedsByCreateTime[index].Add(feed);
                }

                for (int i = 0; i < feedsByCreateTime.Count; i++)
                {
                    GameObject dayFeedElement = Instantiate(DayFeedElement);
                    dayFeedElement.name = $"{DateTime.Parse(feedsByCreateTime[i][0].createTime):yyyy.MM.dd}";
                    dayFeedElement.transform.SetParent(FeedContent.transform);
                    dayFeedElement.transform.localScale = scale;

                    dayFeedElement.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(feedsByCreateTime[i][0].createTime):yyyy.MM.dd}"; // ��¥ ����

                    for (int j = 0; j < feedsByCreateTime[i].Count; j++)
                    {
                        FeedClass feed = feedsByCreateTime[i][j];

                        GameObject feedRecordElemnt = Instantiate(FeedRecordElemnt);
                        feedRecordElemnt.transform.SetParent(dayFeedElement.transform.GetChild(1).transform);
                        feedRecordElemnt.transform.localScale = scale;

                        feedRecordElemnt.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = $"{DateTime.Parse(feed.createTime):HH:mm}";
                        feedRecordElemnt.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = $"{PlayerPrefs.GetString("dogName")}{GetFeedVerb(PlayerPrefs.GetString("dogName"))} ���� ����!";
                    }
                }
            }
        }
        else
        {
            Debug.Log("��ü ��� ���� �ҷ����� ���� - �ڵ� : " + response.responseCode);

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetFeedList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    IEnumerator GetBathList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/bath/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            baths = JsonUtility.FromJson<APIResponse<List<BathResponseDtoClass>>>(response.downloadHandler.text).data;

            bool bathFlag = true;

            string prevBathTime = PlayerPrefs.GetString("createTime");

            for (int i = 0; i < baths.Count; i++)
            {
                int dayDIff = GetDayDiff(prevBathTime, baths[i].createTime);
                prevBathTime = baths[i].createTime;

                if (dayDIff > 35)
                {
                    bathFlag = false;
                    break;
                }
            }

            if (!bathFlag)
            {
                HealthBadGroup.SetActive(true);
                HealthGoodGroup.SetActive(false);
                HealthBadCommentText.SetActive(true);
                HealthGoodCommentText.SetActive(false);
            }
        }
        else
        {
            Debug.Log("��ü ��� ���� �ҷ����� ���� - �ڵ�:" + response.responseCode);

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetBathList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    IEnumerator GetBrushingTeethList()
    {
        string api_url = $"{ServerSettings.SERVER_URL}/api/brushing-teeth/list";

        UnityWebRequest response = UnityWebRequest.Get(api_url);

        string accessToken = PlayerPrefs.GetString("accessToken");
        string jwtToken = $"Bearer {accessToken}";

        response.SetRequestHeader("Authorization", jwtToken);

        yield return response.SendWebRequest();

        if (response.error == null)
        {
            brushingTeeths = JsonUtility.FromJson<APIResponse<List<BathResponseDtoClass>>>(response.downloadHandler.text).data;

            bool brushingTeethFlag = true;

            string prevBrushingTeethTime = PlayerPrefs.GetString("createTime");

            for (int i = 0; i < brushingTeeths.Count; i++)
            {
                int dayDIff = GetDayDiff(prevBrushingTeethTime, brushingTeeths[i].createTime);
                prevBrushingTeethTime = brushingTeeths[i].createTime;

                if (dayDIff > 7)
                {
                    brushingTeethFlag = false;
                    break;
                }
            }

            if (!brushingTeethFlag)
            {
                HealthBadGroup.SetActive(true);
                HealthGoodGroup.SetActive(false);
                HealthBadCommentText.SetActive(true);
                HealthGoodCommentText.SetActive(false);
            }
        }
        else
        {
            Debug.Log("��ü ��ġ ���� �ҷ����� ���� - �ڵ�:" + response.responseCode);

            if (response.responseCode == 403)
            {
                RefreshTokenManager.Instance.ReIssueRefreshToken();

                StartCoroutine(GetBrushingTeethList());
            }
            else
            {
                SceneManager.LoadScene("ReportScene");
            }
        }
    }

    private string GetDiffTimeString(string startDate, string endDate)
    {
        DateTime StartDate = DateTime.Parse(startDate);
        DateTime EndDate = DateTime.Parse(endDate);
        TimeSpan dateDiff = EndDate - StartDate;

        int diffDay = dateDiff.Days;
        int diffHour = dateDiff.Hours;
        int diffMinute = dateDiff.Minutes;

        if (diffDay > 0)
        {
            if (diffHour > 0 || diffMinute > 0)
            {
                diffDay++;
            }

            return $"{diffDay}�� �� �躯 �е� ����";
        }

        if (diffHour > 0)
        {
            if (diffMinute > 0)
            {
                diffHour++;
            }

            if (diffHour == 24)
            {
                return $"1�� �� �躯 �е� ����";
            }

            return $"{diffHour}�ð� �� �躯 �е� ����";
        }

        return $"{diffMinute}�� �� �躯 �е� ����";
    }

    string GetFeedVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // �ѱ��� ���� ó���� ���� ���� ���� ��� 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "����";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "�̿���" : "����";
    }

    string GetTimeFormat(string startTime, string endTime)
    {
        DateTime StartTime = DateTime.Parse(startTime);
        DateTime EndTime = DateTime.Parse(endTime);
        TimeSpan dateDiff = EndTime - StartTime;

        int time = (int)dateDiff.TotalMinutes;

        int hours = time / 60;
        int minutes = time % 60;

        if (hours > 0 && minutes > 0)
        {
            return $"{hours}�ð� {minutes}��";
        }

        if (hours > 0)
        {
            return $"{hours}�ð�";
        }

        return $"{minutes}��";
    }

    string GetTotalTimeString(int totalTimeMinute)
    {
        int hours = totalTimeMinute / 60;
        int minutes = totalTimeMinute % 60;

        if (hours > 0 && minutes > 0)
        {
            return $"{hours}�ð� {minutes}��";
        }

        if (hours > 0)
        {
            return $"{hours}�ð�";
        }

        return $"{minutes}��";
    }

    int GetTimeDiff(string startTime, string endTime)
    {
        DateTime StartTime = DateTime.Parse(startTime);
        DateTime EndTime = DateTime.Parse(endTime);
        TimeSpan dateDiff = EndTime - StartTime;

        int time = (int)dateDiff.TotalMinutes;

        return time;
    }

    int GetDayDiff(string startTime, string endTime)
    {
        DateTime StartTime = DateTime.Parse(startTime);
        DateTime EndTime = DateTime.Parse(endTime);
        TimeSpan dateDiff = EndTime - StartTime;

        int dayDiff = (int)dateDiff.TotalDays;

        return dayDiff;
    }

    string GetEuiVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // �ѱ��� ���� ó���� ���� ���� ���� ��� 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "��";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "����" : "��";
    }

    string GetVerb(string dogName)
    {
        char lastLetter = dogName.ElementAt(dogName.Length - 1);

        // �ѱ��� ���� ó���� ���� ���� ���� ��� 
        if (lastLetter < 0xAC00 || lastLetter > 0xD7A3)
        {
            return "�� ����";
        }

        return (lastLetter - 0xAC00) % 28 > 0 ? "�̿� ����" : "�� ����";
    }

    public void OnClickStressButton()
    {
        StressModalPanel.SetActive(true);
    }

    public void OnClickStressCancelButton()
    {
        StressModalPanel.SetActive(false);
    }

    public void OnClickAffectionButton()
    {
        AffectionModalPanel.SetActive(true);
    }

    public void OnClickAffectionCancelButton()
    {
        AffectionModalPanel.SetActive(false);
    }

    public void OnClickPoopButton()
    {
        PoopModalPanel.SetActive(true);
    }

    public void OnClickPoopCancelButton()
    {
        PoopModalPanel.SetActive(false);
    }

    public void OnClickFeedButton()
    {
        FeedModalPanel.SetActive(true);
    }

    public void OnClickFeedCancelButton()
    {
        FeedModalPanel.SetActive(false);
    }

    public void OnClickWalkButton()
    {
        WalkModalPanel.SetActive(true);
    }

    public void OnClickWalkCancelButton()
    {
        WalkModalPanel.SetActive(false);
    }

    public void OnClickCommunicationButton()
    {
        CommunicationModalPanel.SetActive(true);
    }

    public void OnClickCommunicationCancelButton()
    {
        CommunicationModalPanel.SetActive(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerPrefs.SetString("exitTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));

            DateTime enterTime = DateTime.Parse(PlayerPrefs.GetString("enterTime"));
            DateTime exitTime = DateTime.Parse(PlayerPrefs.GetString("exitTime"));
            TimeSpan timeDiff = exitTime - enterTime;

            int diffMinute = timeDiff.Days * 24 * 60 + timeDiff.Hours * 60 + timeDiff.Minutes;

            RefreshTokenManager.Instance.UpdateTotalTime(diffMinute);
        }
        else
        {
            PlayerPrefs.SetString("enterTime", DateTime.Now.ToString("yyyy.MM.dd.HH:mm"));
        }
    }
}
