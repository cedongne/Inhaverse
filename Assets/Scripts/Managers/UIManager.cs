using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("===== UI 오브젝트 참조 =====")]
    [Space]
    public GameObject loginUI;
    public GameObject hudUI;
    public GameObject classWindow;
    public GameObject classMakingWindow;
    public GameObject classListWindow;
    public GameObject conferenceWindow;
    public GameObject playerInfoWindow;
    public GameObject openFileWindow;
    public GameObject classStudentListWindow;
    public GameObject dontHaveAuthority;

    [Header("===== HUD 채팅 UI")]
    [Space]
    public InputField hudChatInputField;
    public Text hudChatText;
    public Scrollbar hudChatScrollbar;
    public GameObject hudInputField;
    public GameObject hudSendButton;

    [Header("===== 회의채팅 UI")]
    [Space]
    public InputField conferenceChatInputField;
    public Text conferenceChatText;
    public Scrollbar conferenceChatScrollbar;
    public GameObject conferenceInputField;
    public GameObject conferenceSendButton;

    [Header("===== 객체 참조 =====")]
    [Space]
    public PlayerContoller playerController;
    public CameraController cameraController;

    [Header("===== 강의 데이터 UI =====")]
    [Space]
    public InputField classInstructor;

    public InputField classNameInput;

    public InputField classIdInput;
    public InputField classNumberInput;

    public Dropdown firstDayOfWeekInput;
    public InputField firstStartTimeInput;
    public InputField firstEndTimeInput;

    public Dropdown secondDayOfWeekInput;
    public InputField secondStartTimeInput;
    public InputField secondEndTimeInput;

    public InputField studentIdInput;
    public Text studentListText;
    public GameObject classButton;

    [Space]
    List<ClassList> buttons = new List<ClassList>();
    List<UserInfo> studentsList = new List<UserInfo>();

    [Header("===== 회의 UI =====")]
    [Space]
    public Text ConferenceMemberText;
    public bool isOpenWindow;

    [Header("=====권한 경고 메세지 UI =====")]
    [Space]
    public Image Authoritybackground;
    public Text Authoritytext;

    [Header("===== 플레이어 인포 UI =====")]
    [Space]
    public InputField playerName;
    public InputField playerSchoolId;
    public GameObject classListContent;
    public List<Text> classAttendanceList = new List<Text>();

    List<GameObject> playerClassList = new List<GameObject>();
    Vector3 classListInitPosition = new Vector2(20, -20);
    Vector3 classListOffset = new Vector2(0, -160);

    public delegate void EventFunction(int num);

    EventFunction eventFunction;

    public int playerCount;
    private UIManager() { }

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<UIManager>();
                if (obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<UIManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UIInitFromPlayfabLogin(string _playerName, string _playerSchoolId)
    {
        playerName.text = _playerName;
        playerSchoolId.text = _playerSchoolId;
        if(playerController != null)
            playerController.name = _playerName;
        
    }
    #region HUD Icons
    public void ChangeViewBtn()
    {
        cameraController.isChangeCameraModeDown = true;
        cameraController.ChangeCameraMode();
    }

    public void ChangeRunBtn()
    {
        playerController.isRunDown = true;
        playerController.WalkToRun();
    }

    public void CamOnOffBtn()
    {
        playerController.isCamDown = true;
        playerController.TurnWebCam();
    }

    public void VoiceOnOffBtn()
    {
        VoiceManager.Instance.isVoiceDown = true;
        VoiceManager.Instance.VoiceOnOff();
    }

    public void InfoBtn()
    {
        foreach(var box in playerClassList)
        {
            Destroy(box);
        }
        Debug.Log("Btn");
        playerName.text = PlayfabManager.Instance.playerName;
        playerSchoolId.text = PlayfabManager.Instance.playerSchoolId;

        PlayfabManager.Instance.GetGroupList("ShowClassInfo");

        OpenWindow(Define.UI.PLAYERINFO);
    }

    public void ShowClassInfoBtnCallBack(List<PlayFab.GroupsModels.GroupWithRoles> groups)
    {
        for (int count = 0; count < groups.Count; count++)
        {
            PlayfabManager.Instance.GetUserData(groups[count].GroupName, "InstantiateClassInfo");
        }
    }

    public void InstantiateClassInfo(string groupName, string classInfo)
    {
        GameObject newClassInfo = Instantiate(Resources.Load<GameObject>("UIPrefabs/ClassInfo Box"),
            classListContent.transform.position + classListInitPosition + classListOffset * playerClassList.Count, Quaternion.identity, classListContent.transform);
        newClassInfo.name = groupName;

        string[] splitedClassInfo = UtilityMethods.SplitTimeTableUserData(classInfo);
        Transform newInfoTransform = newClassInfo.transform;

        // 0 : Class ID
        // 1 : Class instructor
        // 2 : First class day of week
        // 3 : First class start time
        // 4 : First class end time
        // 5 : Second class Day of week
        // 6 : Second class start time
        // 7 : Second class end time
        splitedClassInfo[2] = UtilityMethods.ConvertDayOfWeekToKorean(splitedClassInfo[2]);
        if(splitedClassInfo.Length == 8)
            splitedClassInfo[5] = UtilityMethods.ConvertDayOfWeekToKorean(splitedClassInfo[5]);
        newInfoTransform.GetChild(0).GetComponent<Text>().text = groupName;
        for (int count = 0; count < splitedClassInfo.Length; count++)
        {
            if(count == 5)
            {
                newInfoTransform.GetChild(13).GetComponent<Text>().gameObject.SetActive(true);
                newInfoTransform.GetChild(14).GetComponent<Text>().gameObject.SetActive(true);
            }
            newInfoTransform.GetChild(count + 1).GetComponent<Text>().gameObject.SetActive(true);
            newInfoTransform.GetChild(count + 1).GetComponent<Text>().text = splitedClassInfo[count].ToString();
        }
        Button deleteButton = newClassInfo.GetComponentInChildren<Button>();
        GameObject tmpClassInfo = newClassInfo;
        deleteButton.onClick.AddListener(delegate { DeleteClassBtn(tmpClassInfo); });

        playerClassList.Add(newClassInfo);
        newClassInfo.transform.position = classListContent.transform.position + classListInitPosition + classListOffset * playerClassList.IndexOf(newClassInfo);
        PlayfabManager.Instance.GetLeaderBoardForTotalAttendanceUI(splitedClassInfo[0] + "Attendance", playerName.text, newInfoTransform.GetChild(11).GetComponent<Text>());
    }

    public void DeleteClassBtn(GameObject classInfo)
    {
        PlayfabManager.Instance.DeleteUserData(classInfo.name);
//        PlayfabManager.Instance.GetGroupList(Define.GROUPLISTUSING.FINDSPECIFICGROUP);
    }
#endregion

    public void ShowUI(Define.UI showingUi)
    {
        loginUI.SetActive(false);
        hudUI.SetActive(false);
        conferenceWindow.SetActive(false);

        CloseWindow();

        if (showingUi.Equals(Define.UI.LOGIN))
        {
            loginUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.HUD))
        {
            hudUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.CONFERENCE))
        {
            conferenceWindow.SetActive(true);
        }
    }

    public void OpenWindow(Define.UI showingWindow)
    {
        Cursor.lockState = CursorLockMode.None;

        classWindow.SetActive(false);
        classMakingWindow.SetActive(false);
        classListWindow.SetActive(false);
        playerInfoWindow.SetActive(false);
        openFileWindow.SetActive(false);

        if (showingWindow.Equals(Define.UI.CLASS))
        {
            classWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.CLASSMAKING))
        {
            classMakingWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.CLASSLIST))
        {
            classListWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.PLAYERINFO))
        {
            playerInfoWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.OPENFILE))
        {
            openFileWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.CLASSSTUDENTLIST))
        {
            classStudentListWindow.SetActive(true);
        }
        isOpenWindow = true;
    }

    public void CloseWindow()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        classWindow.SetActive(false);
        classMakingWindow.SetActive(false);
        classListWindow.SetActive(false);
        conferenceWindow.SetActive(false);
        playerInfoWindow.SetActive(false);
        openFileWindow.SetActive(false);
        classStudentListWindow.SetActive(false);

        ClearClassMakingWindow();
        eventFunction = null;
        isOpenWindow = false;

        ChatManager.Instance.SetHUDChatUI();
    }

#region Class Interaction
    public void EnterClassBtn()
    {
        OpenWindow(Define.UI.CLASSLIST);

        eventFunction += OnClickGetUserData;
        PlayfabManager.Instance.GetGroupList("OpenClassListWindow");
        // Go to "OpenClassListWindowCallBack"
    }

    public void OpenClassMakingWindow()
    {
        OpenWindow(Define.UI.CLASSMAKING);
        classInstructor.text = PlayfabManager.Instance.playerName;
    }

    public void MakeClassBtn()
    {
        ClassData classData = new ClassData();

        classData.className = classNameInput.text;

        classData.classId = classIdInput.text;
        classData.classNumber = classNumberInput.text;

        classData.classInstructor = classInstructor.text;

        classData.firstDayOfWeek = UtilityMethods.ConvertToDayOfWeek(firstDayOfWeekInput.value);
        classData.firstStartTime = firstStartTimeInput.text;
        classData.firstEndTime = firstEndTimeInput.text;

        classData.secondDayOfWeek = UtilityMethods.ConvertToDayOfWeek(secondDayOfWeekInput.value);
        classData.secondStartTime = secondStartTimeInput.text;
        classData.secondEndTime = secondEndTimeInput.text;

        classData.students = studentsList.ToList();

        PlayfabManager.Instance.CreateGroup(classNameInput.text + classNumberInput.text, "ClassData", classData);
        CloseWindow();
    }

    public void SetClassDataForStudent(string playfabId)
    {
        Debug.Log(playfabId);
        PlayfabManager.Instance.SetUserData(classIdInput.text, classNameInput.text);
    }

    void ClearClassMakingWindow()
    {
        classNameInput.text = "";

        classIdInput.text = "";
        classNumberInput.text = "";

        firstDayOfWeekInput.value = 0;
        firstStartTimeInput.text = "";
        firstEndTimeInput.text = "";

        secondDayOfWeekInput.value = 0;
        secondStartTimeInput.text = "";
        secondEndTimeInput.text = "";

        studentIdInput.text = "";

        studentsList.Clear();
        studentListText.text = "";
    }

    public void OpenClassListWindow()
    {
        classInstructor.text = PlayfabManager.Instance.playerName;
        OpenWindow(Define.UI.CLASSLIST);
        PlayfabManager.Instance.GetGroupList("OpenClassListWindow");
        eventFunction += OnClickModifyingClass;
    }

    public void OpenClassListWindowCallBack(List<PlayFab.GroupsModels.GroupWithRoles> groups)
    {
        for(int count = 0; count < buttons.Count; count++)
        {
            Destroy(buttons[count].button);
        }
        buttons.Clear();
        Vector2 initButtonPosition = new Vector2(0, 200);
        Vector2 buttonOffset = new Vector2(0, -60);
        for (int count = 0; count < groups.Count; count++)
        {
            buttons.Add(new ClassList(count, Instantiate(classButton, new Vector2(0, 0), Quaternion.identity, GameObject.Find("Class List Window").transform), 
                groups[count].Group.Id, groups[count].Group.Type));
            buttons[count].button.transform.localPosition = initButtonPosition + (buttonOffset * count);
            buttons[count].button.name = groups[count].GroupName;
            buttons[count].button.GetComponentInChildren<Text>().text = groups[count].GroupName;

            int tmpCount = count;
            buttons[count].button.GetComponent<Button>().onClick.AddListener(delegate () { eventFunction(tmpCount); });
        }
    }

    public void OnClickModifyingClass(int btnNum)
    {
        PlayfabManager.Instance.GetObjectData("ClassData", buttons[btnNum].entityId, buttons[btnNum].entityType, "ClassData");
    }

    public void OnClickGetUserData(int btnNum)
    {
        eventFunction -= OnClickGetUserData;
        PlayfabManager.Instance.GetUserData(buttons[btnNum].button.name, "JoinToClass");
    }

    public void LoadModifyingClass(object classObject)
    {
        ClassData classData = JsonUtility.FromJson<ClassData>(classObject.ToString());
        classNameInput.text = classData.className;

        classIdInput.text = classData.classId;
        classNumberInput.text = classData.classNumber;
        
        firstDayOfWeekInput.value = UtilityMethods.ConvertToDayCode(classData.firstDayOfWeek);
        firstStartTimeInput.text = classData.firstStartTime;
        firstEndTimeInput.text = classData.firstEndTime;

        secondDayOfWeekInput.value = UtilityMethods.ConvertToDayCode(classData.secondDayOfWeek);
        secondStartTimeInput.text = classData.secondStartTime;
        secondEndTimeInput.text = classData.secondEndTime;

        studentsList = classData.students.ToList();
        for(int count = 0; count < studentsList.Count; count++)
        {
            studentListText.text += studentsList[count].schoolId + " " + studentsList[count].name + "\n";
        }

        OpenWindow(Define.UI.CLASSMAKING);
    }

    public void AddStudentInClassButton()
    {
        PlayfabManager.Instance.getPlayerInfoEvent += AddStudentInClass;
        PlayfabManager.Instance.GetPlayerInfoUsingStudentId(studentIdInput.text);
    }

    public void AddStudentInClass(string studentId, string studentName)
    {
        for (int count = 0; count < studentsList.Count; count++)
        {
            if (studentsList[count].name.Equals(studentName))
            {
                Debug.Log("이미 수강 중인 학생입니다.");
                return;
            }
        }
        studentsList.Add(new UserInfo(studentId, studentName));
        studentListText.text += studentId + " " + studentName + "\n";
        PlayfabManager.Instance.getPlayerInfoEvent -= AddStudentInClass;
    }

    public void DeleteStudentInClassButton()
    {
        for (int count = 0; count < studentsList.Count; count++)
        {
            if (studentsList[count].schoolId.Equals(studentIdInput.text))
            {
                studentListText.text = studentListText.text.Replace(studentsList[count].schoolId + " " + studentsList[count].name + "\n", "");
                studentsList.RemoveAt(count);
            }
        }
    }

    public IEnumerator FadeOutDontHaveAuthority()
    {
        dontHaveAuthority.SetActive(true);
        yield return StartCoroutine(FadeOutCoroutine());
        dontHaveAuthority.SetActive(false);
    }

    IEnumerator FadeOutCoroutine()
    {
        float fadeCount = 1.0f;
        while(fadeCount > 0)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            Authoritybackground.color = new Color(Authoritybackground.color.r, Authoritybackground.color.g, Authoritybackground.color.b, fadeCount);
            Authoritytext.color = new Color(Authoritytext.color.r, Authoritytext.color.g, Authoritytext.color.b, fadeCount);
        }
    }
    #endregion

#region Conference Interaction

#endregion
}
