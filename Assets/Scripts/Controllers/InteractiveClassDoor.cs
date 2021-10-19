using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

[System.Serializable]
public class ClassData
{
    public string className;

    public string firstDayOfWeek;
    public string firstStartTime;
    public string firstEndTime;

    public string secondDayOfWeek;
    public string secondStartTime;
    public string secondEndTime;
}

public class InteractiveClassDoor : InteractiveObject
{
    public InputField classInstructor;
    public InputField classNameInput;
    public InputField classId;

    public InputField firstDayOfWeekInput;
    public InputField firstStartTimeInput;
    public InputField firstEndTimeInput;

    public InputField secondDayOfWeekInput;
    public InputField secondStartTimeInput;
    public InputField secondEndTimeInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //       Debug.Log(DateTime.Now.Hour + " " + DateTime.Now.Minute);
        //    Debug.Log(DateTime.Now.DayOfWeek.);
    }

    public override void Interaction()
    {
        if (PlayfabManager.Instance.playerJob == "학생")
        {

        }
        else if (PlayfabManager.Instance.playerJob == "교수")
        {
            UIManager.Instance.OpenWindow(Define.UI.CLASS);
        }
    }

    public void EnterClass()
    {

    }

    public void OpenClassMakingWindow()
    {
        UIManager.Instance.CloseWindow();
        UIManager.Instance.OpenWindow(Define.UI.CLASSMAKING);
        classInstructor.text = PlayfabManager.Instance.name;
    }

    public void MakeClass()
    {
        ClassData classData = new ClassData();

        classData.className = classNameInput.text;

        classData.firstDayOfWeek = firstDayOfWeekInput.text;
        classData.firstStartTime = firstStartTimeInput.text;
        classData.firstEndTime = firstEndTimeInput.text;

        classData.secondDayOfWeek = secondDayOfWeekInput.text;
        classData.secondStartTime = secondStartTimeInput.text;
        classData.secondEndTime = secondEndTimeInput.text;

        PlayfabManager.Instance.CreateGroup(classId.text, "ClassData", classData);
    }

    public void ModifyClass()
    {

    }
}
