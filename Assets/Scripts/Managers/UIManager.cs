using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public InputField classInstructor;
    public InputField classNameInput;
    public InputField classIdInput;

    public InputField firstDayOfWeekInput;
    public InputField firstStartTimeInput;
    public InputField firstEndTimeInput;

    public InputField secondDayOfWeekInput;
    public InputField secondStartTimeInput;
    public InputField secondEndTimeInput;

    public GameObject classButton;

    List<ClassList> buttons = new List<ClassList>();

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

    public GameObject loginUI;
    public GameObject hudUI;
    public GameObject classWindow;
    public GameObject classMakingWindow;
    public GameObject classModifyingListWindow;

    public bool isOpenWindow;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<UIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowUI(Define.UI showingUi)
    {
        loginUI.SetActive(false);
        hudUI.SetActive(false);
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
        classWindow.SetActive(false);
        classMakingWindow.SetActive(false);

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
        isOpenWindow = true;
    }

    public void CloseWindow()
    {
        classWindow.SetActive(false);
        classMakingWindow.SetActive(false);
        classModifyingListWindow.SetActive(false);

        isOpenWindow = false;
    }

#region classInteraction
    public void EnterClassBtn()
    {

    }

    public void OpenClassMakingWindow()
    {
        CloseWindow();
        OpenWindow(Define.UI.CLASSMAKING);
        classInstructor.text = PlayfabManager.Instance.name;
    }

    public void MakeClassBtn()
    {
        ClassData classData = new ClassData();

        classData.className = classNameInput.text;
        classData.classId = classIdInput.text;

        classData.firstDayOfWeek = firstDayOfWeekInput.text;
        classData.firstStartTime = firstStartTimeInput.text;
        classData.firstEndTime = firstEndTimeInput.text;

        classData.secondDayOfWeek = secondDayOfWeekInput.text;
        classData.secondStartTime = secondStartTimeInput.text;
        classData.secondEndTime = secondEndTimeInput.text;

        PlayfabManager.Instance.CreateGroup(classIdInput.text, "ClassData", classData);

        classNameInput.text = "";
        classIdInput.text = "";
        firstDayOfWeekInput.text = "";
        firstStartTimeInput.text = "";
        firstEndTimeInput.text = "";
        secondDayOfWeekInput.text = "";
        secondStartTimeInput.text = "";
        secondEndTimeInput.text = "";

        CloseWindow();
    }

    public void ModifyClassBtn()
    {

    }

    public void OpenClassModifyingWindow()
    {
        OpenWindow(Define.UI.CLASSMODIFYINGLIST);
        PlayfabManager.Instance.GetGroupList();
    }

    public void LoadMyClasses(List<PlayFab.GroupsModels.GroupWithRoles> groups)
    {
        buttons.Clear();
        Vector2 initButtonPosition = new Vector2(0, 200);
        Vector2 buttonOffset = new Vector2(0, -60);
        for (int count = 0; count < groups.Count; count++)
        {
            buttons.Add(new ClassList(count, Instantiate(classButton, new Vector2(0, 0), Quaternion.identity, GameObject.Find("Class Modifying List Window").transform), 
                groups[count].Group.Id, groups[count].Group.Type));
            buttons[count].button.transform.localPosition = initButtonPosition + (buttonOffset * count);
            buttons[count].button.name = groups[count].GroupName;
            buttons[count].button.GetComponentInChildren<Text>().text = groups[count].GroupName;
            int tmpCount = count;
            buttons[count].button.GetComponent<Button>().onClick.AddListener(delegate () { classButtonOnClick(tmpCount); });
        }
    }

    public void classButtonOnClick(int btnNum)
    {
        PlayfabManager.Instance.GetObjectData("ClassData", buttons[btnNum].entityId, buttons[btnNum].entityType, "ClassData");
    }

    public void LoadModifyingClass(object classObject)
    {
        ClassData classData = JsonUtility.FromJson<ClassData>(classObject.ToString());
        Debug.Log(classData.className);
        classNameInput.text = classData.className;
        classIdInput.text = classData.classId;

        firstDayOfWeekInput.text = classData.firstDayOfWeek;
        firstStartTimeInput.text = classData.firstStartTime;
        firstEndTimeInput.text = classData.firstEndTime;

        secondDayOfWeekInput.text = classData.secondDayOfWeek;
        secondStartTimeInput.text = classData.secondStartTime;
        secondEndTimeInput.text = classData.secondEndTime;

        CloseWindow();
        OpenWindow(Define.UI.CLASSMAKING);
    }
    #endregion

}
