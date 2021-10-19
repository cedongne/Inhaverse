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
    public string firstDayOfWeek;
    public int firstStartTime;
    public int firstEndTime;
    public string secondDayOfWeek;
    public int secondStartTime;
    public int secondEndTime;
}

public class InteractiveClassDoor : InteractiveObject
{
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
        if(PlayfabManager.job == "학생")
        {

        }
        else if(PlayfabManager.job == "교수")
        {
            UIManager.Instance.OpenWindow(Define.UI.CLASS);
        }
    }

    void EnterClass()
    {

    }

    void MakeClass()
    {
        ClassData classData = new ClassData();
        
    }

    void ModifyClass()
    {

    }
}
