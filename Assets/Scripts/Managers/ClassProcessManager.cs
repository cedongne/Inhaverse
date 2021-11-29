using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using Photon.Pun;
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
    Dictionary<UserInfo, int> studentAttendanceList = new Dictionary<UserInfo, int>();
    string class_name;
    public Define.CLASSSTATE classState;

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
    public void StartClass()
    {
        classState = Define.CLASSSTATE.START;
        CheckAttendance();
    }

    public void EndClass()
    {
        classState = Define.CLASSSTATE.END;
        CancelInvoke("CheckAttendancePeriodically");
        photonView.RPC("SendAttendanceToInstructor", RpcTarget.AllBuffered);

        Invoke("PrintPlayerList", 5f);
    }
    #endregion

    void CheckAttendance()
    {
        CheckAttendancePeriodically();
        photonView.RPC("CheckAttendanceRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void CheckAttendanceRPC()
    {
        CheckAttendancePeriodically();
    }

    void CheckAttendancePeriodically()
    {
        attendance_count++;
        Debug.Log(attendance_count);
        Invoke("CheckAttendancePeriodically", 3);
    }

    [PunRPC]
    void SendAttendanceToInstructor()
    {
        studentAttendanceList.Add( new UserInfo { name = PlayfabManager.Instance.playerName, schoolId = PlayfabManager.Instance.playerSchoolId }, attendance_count );
    }

    void PrintPlayerList()
    {
        Debug.Log(studentAttendanceList.Count);
        foreach(var a in studentAttendanceList)
        {
            Debug.Log(a.Key + " " + a.Value);
        }
    }
    public void JoinClass()
    {
        if(classState.Equals(Define.CLASSSTATE.START))
            CheckAttendancePeriodically();
    }

    private void OnEnable()
    {
        class_name = NetworkManager.Instance.room_name;
        Debug.Log(NetworkManager.Instance.room_name);
        PlayfabManager.Instance.GetLeaderBoardUserValue(class_name + UtilityMethods.GetWeekOfSemester().ToString() + DateTime.Now.DayOfWeek.ToString()
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
