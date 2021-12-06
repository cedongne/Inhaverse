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
        PlayfabManager.Instance.GetLeaderBoard(class_name, PlayfabManager.Instance.playerName, "CountStudentNumber");
    }

    public void CountStudentNumber(int _students_count)
    {
        student_count = _students_count;
    }

    public void StartClass()
    {
        CheckAttendance();
    }

    public void EndClass()
    {
        PlayfabManager.Instance.UpdateLeaderBoard(class_name + UtilityMethods.GetWeekOfSemester().ToString() + DateTime.Now.DayOfWeek.ToString()
            , attendance_count);
        photonView.RPC("StopCheckAttend", RpcTarget.AllBuffered, attendance_count);

        GameObject.Find("Camera Arm").GetComponent<CameraController>().playerTransform = playerContoller.transform;
    }
    #endregion


    void CheckAttendance()
    {
        photonView.RPC("CheckAttendanceRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void CheckAttendanceRPC()
    {
        classState = Define.CLASSSTATE.START;
        CheckAttendancePeriodically();
    }

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

    public void JoinClass()
    {
        if(classState.Equals(Define.CLASSSTATE.START))
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("########Someone is comming");
    }

    public void LoadAttendanceCount(int _attendance_count)
    {
        attendance_count = _attendance_count;
    }
}
