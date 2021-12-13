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

    public bool isSittedChair = false;

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
        classState = Define.CLASSSTATE.START;
        photonView.RPC("ReadyCLASS", RpcTarget.AllBuffered, classState);
        Application.OpenURL("https://owake.me/");

        UIManager.Instance.ClassChannelNameInputObject.SetActive(true);
    }

    public void JoinClassBtn()
    {
        Application.OpenURL("https://owake.me/");
    }

    [PunRPC]
    public void ReadyCLASS(Define.CLASSSTATE sender_conference_state)
    {
        Debug.Log("Ready Receiver " + sender_conference_state);
        if (sender_conference_state.Equals(Define.CLASSSTATE.READY))
        {
            Debug.Log("ReadyCLASS");
  //          UIManager.Instance.videoConferenceButton.interactable = false;
  //          UIManager.Instance.videoConferenceText.text = "수업 생성 중...";
        }
    }
    public void EnterCLASSChannelNameBtn()
    {
        classChannelName = UIManager.Instance.ClassChannelNameInputField.text;
        classState = Define.CLASSSTATE.START;
        photonView.RPC("StartCLASS", RpcTarget.AllBuffered, classChannelName);
        photonView.RPC("NoticeClassStart", RpcTarget.AllBuffered, classState);

        UIManager.Instance.ClassChannelNameInputObject.SetActive(false);
        UIManager.Instance.ClassChannelNameInputField.text = "";
        UIManager.Instance.classhannelNameObject.SetActive(true);
        UIManager.Instance.startClassButton.interactable = false;
        UIManager.Instance.classChannelNameText.text = classChannelName;
    }


    [PunRPC]
    public void StartCLASS(string classChannelName)
    {
        if (PlayfabManager.Instance.playerJob.Equals("학생"))
        {
            Debug.Log("Start Receiver " + " " + classChannelName);
            Debug.Log("StartCLASS");
            UIManager.Instance.studentClasshannelNameText.text = classChannelName;
            UIManager.Instance.studentClasshannelNameObject.SetActive(true);

            UIManager.Instance.classJoinButton.interactable = true;
            UIManager.Instance.classJoinText.text = "화상 수업 참여";
        }
    }

    [PunRPC]
    public void EndCLASS()
    {
        classState = Define.CLASSSTATE.END;
        UIManager.Instance.startClassButton.interactable = true;
        Debug.Log("CLASS");
        UIManager.Instance.classChannelNameText.text = "";
        UIManager.Instance.classhannelNameObject.SetActive(false);
        UIManager.Instance.ClassChannelNameInputField.text = "";
        UIManager.Instance.ClassChannelNameInputObject.SetActive(false);
        UIManager.Instance.studentClasshannelNameText.text = "";
        UIManager.Instance.studentClasshannelNameObject.SetActive(false);

        UIManager.Instance.classJoinButton.interactable = false;
        UIManager.Instance.classJoinText.text = "화상 수업 시작 전..";
    }

    #region Instructor's Function
    public void ReadyClass()
    {
        classState = Define.CLASSSTATE.READY;
        UpdateStudentNumberInClassroom();
        UIManager.Instance.ShowSubUI(Define.UI.CLASSREADY);
        MineManager.Instance.playerController.canDetectInteractive = false;
        PlayfabManager.Instance.GetLeaderBoard(class_name + "Attendance", PlayfabManager.Instance.playerName, "CountStudentNumber");
    }

    public void StopReadyClassBtn()
    {
        classState = Define.CLASSSTATE.END;
        MineManager.Instance.playerController.canDetectInteractive = true;
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
        UIManager.Instance.studentNumberInClassText.text = "[강의실 인원] " + now_student_number + " / " + all_student_number;
    }

    public void EndClass()
    {
        PlayfabManager.Instance.UpdateLeaderBoard(class_name + UtilityMethods.GetWeekOfSemester().ToString() + DateTime.Now.DayOfWeek.ToString()
            , attendance_count);
        photonView.RPC("StopCheckAttend", RpcTarget.AllBuffered, attendance_count);
        photonView.RPC("EndCLASS", RpcTarget.AllBuffered);
        UIManager.Instance.HideSubUI();

        MineManager.Instance.playerController.canDetectInteractive = true;
        MineManager.Instance.cameraController.playerTransform = playerContoller.transform;
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
        ClassProcessManager.Instance.isSittedChair = false;
        UIManager.Instance.HideSubUI();

        MineManager.Instance.playerController.OnKinematic(false);
        MineManager.Instance.playerController.canMove = true;
        MineManager.Instance.playerController.canDetectInteractive = true;
        MineManager.Instance.playerController.canGetInput = true;
        MineManager.Instance.playerController.isSitted = false;

        MineManager.Instance.cameraController.cameraArmPositionOffset.y -= 0.3f;
    }
}