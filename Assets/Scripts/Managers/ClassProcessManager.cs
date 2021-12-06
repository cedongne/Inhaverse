using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ClassProcessManager : MonoBehaviourPunCallbacks
{
    private static ClassProcessManager instance;

    public static ClassProcessManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<ClassProcessManager>();
                if(obj != null)
                {
                    instance = obj;
                }
            }
            return instance;
        }
        
    }

    public PlayerContoller playerContoller;

    Dictionary<UserInfo, int> studentAttendanceList = new Dictionary<UserInfo, int>();
    string class_name;
    public Define.CLASSSTATE classState;

    public Transform professorViewObjectTransform;

    public int student_count;
    public int attendance_count = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = GetComponent<ClassProcessManager>();
        }

        classState = Define.CLASSSTATE.END;
    }

    #region Instructor's Function

    public void ReadyClass()
    {
        UIManager.Instance.ShowSubUI(Define.UI.CLASSREADY);
        PlayfabManager.Instance.GetLeaderBoard(class_name, PlayfabManager.Instance.playerName, "CountStudentNumber");
    }

    public void StopReadyClass()
    {
        UIManager.Instance.HideSubUI();
    }

    public void CountStudentNumber(int _students_count)
    {
        student_count = _students_count;
    }

    public void StartClassBtn()
    {
        photonView.RPC("NoticeClassStart", RpcTarget.AllBuffered, Define.CLASSSTATE.START);
    }

    public void ShowSeatsBtn()
    {
        UIManager.Instance.OpenWindow(Define.UI.CLASSSTUDENTLIST);
    }

    public void SomeoneJoinToClass()
    {
        photonView.RPC("NoticeClassStart", RpcTarget.AllBuffered, classState);
    }

    [PunRPC]
    public void NoticeClassStart(Define.CLASSSTATE nowClassState)
    {
        Debug.Log(classState + " " + classState.Equals(Define.CLASSSTATE.END));
        if (classState.Equals(Define.CLASSSTATE.END) && nowClassState.Equals(Define.CLASSSTATE.START))
        {
            Debug.Log("ClassStarted");
            classState = Define.CLASSSTATE.START;
            Invoke("CheckAttendancePeriodically", 3);
        }
    }

    public void EndClass()
    {
        PlayfabManager.Instance.UpdateLeaderBoard(class_name + UtilityMethods.GetWeekOfSemester().ToString() + DateTime.Now.DayOfWeek.ToString()
            , attendance_count);
        photonView.RPC("StopCheckAttend", RpcTarget.AllBuffered, attendance_count);
        UIManager.Instance.HideSubUI();

        GameObject.Find("Camera Arm").GetComponent<CameraController>().playerTransform = playerContoller.transform;
    }
    #endregion

    void CheckAttendancePeriodically()
    {
        Debug.Log(attendance_count);
        attendance_count++;
        Invoke("CheckAttendancePeriodically", 3);
    }

    [PunRPC]
    void StopCheckAttend(int total_attendance_count)
    {
        CancelInvoke("CheckAttendancePeriodically");
        classState = Define.CLASSSTATE.END;

        Debug.Log(total_attendance_count);
//        if(total_attendance_count - attendance_count < 3)
 //       {
            PlayfabManager.Instance.GetLeaderBoard(class_name + "Attendance", PlayfabManager.Instance.playerName, "UpdateAttendance");
  //      }
    }
    public void UpdateAttendance(int attendances)
    {
        Debug.Log(attendances + " " + UtilityMethods.Exponential(2, UtilityMethods.GetWeekOfSemester() - 1));
        Debug.Log((attendances & UtilityMethods.Exponential(2, UtilityMethods.GetWeekOfSemester() - 1)));
        if((attendances & UtilityMethods.Exponential(2, UtilityMethods.GetWeekOfSemester() - 1)) == 0)
        {
            Debug.Log("UPDATE");
            PlayfabManager.Instance.UpdateLeaderBoard(class_name + "Attendance", attendances + UtilityMethods.Exponential(2, UtilityMethods.GetWeekOfSemester() - 1));
        }
        else
        {
            Debug.Log("AlreadyAttend");
        }
    }

    public void JoinToClass(string room_name)
    {
        if (classState.Equals(Define.CLASSSTATE.START))
            CheckAttendancePeriodically();
    }

    public void SetProfessorView(Transform viewObjectTransform)
    {
        professorViewObjectTransform = viewObjectTransform;
    }

    private void OnEnable()
    {
        class_name = NetworkManager.Instance.room_name;
        PlayfabManager.Instance.GetLeaderBoard(class_name + UtilityMethods.GetWeekOfSemester().ToString() + DateTime.Now.DayOfWeek.ToString()
            , PlayfabManager.Instance.playerName, "LoadAttendanceCount");
    }

    private void OnDisable()
    {
        PlayfabManager.Instance.UpdateLeaderBoard(class_name + UtilityMethods.GetWeekOfSemester().ToString() + DateTime.Now.DayOfWeek.ToString()
            , attendance_count);
        instance.enabled = false;
    }

    public void LoadAttendanceCount(int _attendance_count)
    {
        attendance_count = _attendance_count;
    }
}
