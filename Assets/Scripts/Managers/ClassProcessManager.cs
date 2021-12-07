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

    string classChannelName;

    private void Awake()
    {
        if(instance == null)
        {
            instance = GetComponent<ClassProcessManager>();
        }

        classState = Define.CLASSSTATE.END;
    }

    public void StartClassBtn()
    {
        Debug.Log("test////////////////////");
        classState = Define.CLASSSTATE.START;
        photonView.RPC("ReadyCLASS", RpcTarget.AllBuffered, classState);
        Application.OpenURL("https://owake.me/");

        UIManager.Instance.ClassChannelNameInputObject.SetActive(true);
    }

    [PunRPC]
    public void ReadyCLASS(Define.CLASSSTATE sender_conference_state)
    {
        Debug.Log("Ready Receiver " + sender_conference_state);
        if (sender_conference_state.Equals(Define.CLASSSTATE.READY))
        {
            Debug.Log("ReadyCLASS");
            UIManager.Instance.videoConferenceButton.interactable = false;
            UIManager.Instance.videoConferenceText.text = "���� ���� ��...";
        }
    }
    public void EnterCLASSChannelNameBtn()
    {
        classChannelName = UIManager.Instance.conferenceChannelNameInputField.text;
        classState = Define.CLASSSTATE.START;
        photonView.RPC("StartCLASS", RpcTarget.AllBuffered, classState);

        UIManager.Instance.conferenceChannelNameInputObject.SetActive(false);
    }


    [PunRPC]
    public void StartCLASS(string sender_channel_name, string conference_channel_name)
    {
        Debug.Log("Start Receiver " + sender_channel_name + " " + conference_channel_name);
        Debug.Log("StartCLASS");
        UIManager.Instance.conferenceChannelNameText.text = conference_channel_name;
        UIManager.Instance.conferenceChannelNameObject.SetActive(true);

        UIManager.Instance.videoConferenceButton.interactable = true;
    }

    [PunRPC]
    public void EndCLASS(string sender_channel_name)
    {
        Debug.Log("End Receiver " + sender_channel_name);
        Debug.Log("CLASS");
        UIManager.Instance.conferenceChannelNameText.text = "";
        UIManager.Instance.conferenceChannelNameObject.SetActive(false);
        UIManager.Instance.conferenceChannelNameInputField.text = "";
        UIManager.Instance.conferenceChannelNameInputObject.SetActive(false);

        UIManager.Instance.videoConferenceButton.interactable = true;
        UIManager.Instance.videoConferenceText.text = "���� ����";
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
        UIManager.Instance.studentNumberInClassText.text = "[���ǽ� �ο�] " + now_student_number + " / " + all_student_number;
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
        UIManager.Instance.HideSubUI();

        MineManager.Instance.playerController.OnKinematic(false);
        MineManager.Instance.playerController.canMove = true;
        MineManager.Instance.playerController.canDetectInteractive = true;
        MineManager.Instance.playerController.canGetInput = true;

        MineManager.Instance.cameraController.cameraArmPositionOffset.y -= 0.3f;
    }
}