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
    public GameObject classModifyingListWindow;
    public GameObject conferenceWindow;
    public GameObject playerInfoWindow;

    [Header("===== 객체 참조 =====")]
    [Space]
    public PlayerContoller playerController;
    public CameraController cameraController;
    public VoiceController voiceController;
    public PlayerWebCamUIController webCamUIController;

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
    List<StudentInfo> studentsList = new List<StudentInfo>();

    [Header("===== 회의 UI =====")]
    [Space]
    public Text ConferenceMemberText;
    public bool isOpenWindow;

    [Header("===== 플레이어 정보 UI =====")]
    [Space]
    public InputField playerName;
    public InputField playerSchoolId;

    public delegate void EventFunction(int num);

    EventFunction eventFunction;

    private UIManager() { }

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<UIManager>();
                if (obj == null)
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
        webCamUIController.isWebCamDown = true;
        webCamUIController.TurnWebCam();
    }

    public void VoiceOnOffBtn()
    {
        voiceController.isVoiceDown = true;
        voiceController.VoiceOnOff();
    }

    public void InfoBtn()
    {
        playerName.text = PlayfabManager.Instance.playerName;
        playerSchoolId.text = PlayfabManager.Instance.playerSchoolId;

        OpenWindow(Define.UI.PLAYERINFO);
    }
#endregion

    public void ShowUI(Define.UI showingUi)
    {
        loginUI.SetActive(false);
        hudUI.SetActive(false);

        CloseWindow();

        if (showingUi.Equals(Define.UI.LOGIN))
        {
            loginUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.HUD))
        {
            hudUI.SetActive(true);
        }
    }

    public void OpenWindow(Define.UI showingWindow)
    {
        Cursor.lockState = CursorLockMode.None;

        classWindow.SetActive(false);
        classMakingWindow.SetActive(false);
        classModifyingListWindow.SetActive(false);
        conferenceWindow.SetActive(false);
        playerInfoWindow.SetActive(false);

        if (showingWindow.Equals(Define.UI.CLASS))
        {
            classWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.CLASSMAKING))
        {
            classMakingWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.CLASSMODIFYINGLIST))
        {
            classModifyingListWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.CONFERENCE))
        {
            conferenceWindow.SetActive(true);
        }
        else if (showingWindow.Equals(Define.UI.PLAYERINFO))
        {
            playerInfoWindow.SetActive(true);
        }
        isOpenWindow = true;
    }

    public void CloseWindow()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        classWindow.SetActive(false);
        classMakingWindow.SetActive(false);
        classModifyingListWindow.SetActive(false);
        conferenceWindow.SetActive(false);
        playerInfoWindow.SetActive(false);

        ClearClassMakingWindow();
        eventFunction = null;
        isOpenWindow = false;
    }

#region Class Interaction
    public void EnterClassBtn()
    {
        OpenWindow(Define.UI.CLASSMODIFYINGLIST);

        eventFunction += OnClickGetUserData;
        PlayfabManager.Instance.getUserDataEvent.AddListener(NetworkManager.Instance.JoinToClass);
        PlayfabManager.Instance.GetGroupList();

        
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

        firstDayOfWeekInput.value = 1;
        firstStartTimeInput.text = "";
        firstEndTimeInput.text = "";

        secondDayOfWeekInput.value = 1;
        secondStartTimeInput.text = "";
        secondEndTimeInput.text = "";

        studentIdInput.text = "";

        studentsList.Clear();
        studentListText.text = "";
    }

    public void OpenClassModifyingWindow()
    {
        OpenWindow(Define.UI.CLASSMODIFYINGLIST);
        PlayfabManager.Instance.GetGroupList();
        eventFunction += OnClickModifyingClass;
    }

    public void LoadMyClasses(List<PlayFab.GroupsModels.GroupWithRoles> groups)
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
        PlayfabManager.Instance.GetUserData(buttons[btnNum].button.name, "ClassTimeTable");
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
            studentListText.text += studentsList[count].studentId + " " + studentsList[count].studentName + "\n";
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
            if (studentsList[count].studentName.Equals(studentName))
            {
                Debug.Log("이미 수강 중인 학생입니다.");
                return;
            }
        }
        studentsList.Add(new StudentInfo(studentId, studentName));
        studentListText.text += studentId + " " + studentName + "\n";
        PlayfabManager.Instance.getPlayerInfoEvent -= AddStudentInClass;
    }

    public void DeleteStudentInClassButton()
    {
        for (int count = 0; count < studentsList.Count; count++)
        {
            if (studentsList[count].studentId.Equals(studentIdInput.text))
            {
                studentListText.text = studentListText.text.Replace(studentsList[count].studentId + " " + studentsList[count].studentName + "\n", "");
                studentsList.RemoveAt(count);
            }
        }
    }
    #endregion

#region Conference Interaction

#endregion
}
