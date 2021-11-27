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

    string class_name;
    public Define.CLASSSTATE classState;

    public int attendance_count = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = GetComponent<ClassProcessManager>();
        }
    }

    #region Instructor's Function
    public void StartClass()
    {
        CheckAttendance();
        classState = Define.CLASSSTATE.START;
    }

    public void EndClass()
    {
        classState = Define.CLASSSTATE.END;
        CancelInvoke("CheckAttendancePeriodically");
    }
#endregion

    public void CheckAttendance()
    {
        photonView.RPC("CheckAttendancePeriodically", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void CheckAttendancePeriodically()
    {
        attendance_count++;
        Invoke("CheckAttendance", 300);
    }

    public void JoinClass()
    {
        if(classState.Equals(Define.CLASSSTATE.START))
            CheckAttendancePeriodically();
    }

    public override void OnJoinedRoom()
    {
        class_name = NetworkManager.Instance.room_name;
        Debug.Log(NetworkManager.Instance.room_name);
        PlayfabManager.Instance.GetLeaderBoardUserValue(class_name + "" + UtilityMethods.GetWeekOfSemester().ToString() + "" + UtilityMethods.ConvertDayOfWeekToKorean(DateTime.Now.DayOfWeek.ToString())
            , PlayfabManager.Instance.playerName, "LoadAttendanceCount");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("ClassManager");
        class_name = NetworkManager.Instance.room_name;
        PlayfabManager.Instance.UpdateLeaderBoard(class_name + "" + UtilityMethods.GetWeekOfSemester().ToString() + "" + UtilityMethods.ConvertDayOfWeekToKorean(DateTime.Now.DayOfWeek.ToString())
            , attendance_count);
    }

    public void LoadAttendanceCount(int _attendance_count)
    {
        attendance_count = _attendance_count;
    }
}
