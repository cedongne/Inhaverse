using System.Collections;
using System.Collections.Generic;
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

#region Instructor's Function
    public void StartClass()
    {
        CheckAttendancePeriodically();
        classState = Define.CLASSSTATE.START;
    }

    public void EndClass()
    {
        classState = Define.CLASSSTATE.END;
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
        Invoke("CheckAttendancePeriodically", 300);
    }

    public void JoinClass()
    {
        if(classState.Equals(Define.CLASSSTATE.START))
            CheckAttendancePeriodically();
    }

    public override void OnJoinedRoom()
    {
        PlayfabManager.Instance.GetLeaderBoardUserValue(class_name + "" + UtilityMethods.GetWeekOfSemester().ToString(), PlayfabManager.Instance.playerName, "LoadAttendanceCount");
    }

    public override void OnLeftRoom()
    {
        PlayfabManager.Instance.UpdateLeaderBoard(class_name, attendance_count);
    }

    public void LoadAttendanceCount(int _attendance_count)
    {
        attendance_count = _attendance_count;
    }
}
