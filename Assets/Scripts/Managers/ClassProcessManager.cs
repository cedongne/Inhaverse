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

    public int all_student_number;
    public int now_student_number;
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
        classState = Define.CLASSSTATE.READY;
        UpdateStudentNumberInClassroom();
        UIManager.Instance.ShowSubUI(Define.UI.CLASSREADY);
        PlayfabManager.Instance.GetLeaderBoard(class_name + "Attendance", PlayfabManager.Instance.playerName, "CountStudentNumber");
    }

    public void StopReadyClassBtn()
    {
        classState = Define.CLASSSTATE.END;
        UIManager.Instance.HideSubUI();
    }

    public void StartClassBtn()
    {
        photonView.RPC("NoticeClassStart", RpcTarget.AllBuffered, Define.CLASSSTATE.START);
    }

    [PunRPC]
    public void NoticeClassStart(Define.CLASSSTATE nowClassState)
    {
        if (classState.Equals(Define.CLASSSTATE.END) && nowClassState.Equals(Define.CLASSSTATE.START))
        {
            Debug.Log("ClassStarted");
            classState = Define.CLASSSTATE.START;
            Invoke("CheckAttendancePeriodically", 3);
        }
    }

    public void ShowSeatsBtn()
    {
        UIManager.Instance.OpenWindow(Define.UI.CLASSSTUDENTLIST);
    }

    public void UpdateStudentNumberInClassroom()
    {
        now_student_number = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        UIManager.Instance.studentNumberInClassText.text = "[강의실 인원] " + now_student_number + " / " + all_student_number;
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

#region Callback methods
    public void SomeoneJoinToClass()
    {
        UpdateStudentNumberInClassroom();
        photonView.RPC("NoticeClassStart", RpcTarget.AllBuffered, classState);
    }

    public void LoadAttendanceCount(int _attendance_count)
    {
        attendance_count = _attendance_count;
    }

    public void CountStudentNumber(int students_number)
    {
        all_student_number = students_number;
        UpdateStudentNumberInClassroom();
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

    public void SetProfessorView(Transform viewObjectTransform)
    {
        professorViewObjectTransform = viewObjectTransform;
    }

    public void GetUpFromChair()
    {
        MineManager.Instance.player.transform.GetComponent<Rigidbody>().isKinematic = false;
        UIManager.Instance.HideSubUI();
    }
}
