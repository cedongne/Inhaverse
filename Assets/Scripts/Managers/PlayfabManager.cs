using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using PlayFab.DataModels;
using PlayFab.AuthenticationModels;

using Photon;
using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

using System.Linq;

public class PlayfabManager : MonoBehaviourPunCallbacks
{
    private PlayfabManager() { }

    private static PlayfabManager instance;

    public static PlayfabManager Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<PlayfabManager>();
                if(obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    public PlayerLeaderboardEntry MyPlayFabInfo;

    public NetworkManager networkManager;

    public InputField emailInput, passwordInput, usernameInput;
    public Toggle studentToggle, instructorToggle;

    public PlayFab.AuthenticationModels.EntityKey playerEntity;
    public string tmpEntityId;
    public string tmpEntityType;

    string myPlayfabId;
    public string playerJob;
    public string playerName;
    public string playerSchoolId;
    bool isAuthenticate;

    #region Events
    public delegate void GetPlayerInfoEvent(string studentId, string studentName);
    public delegate void GetPlayfabIdEvent(string playfabId);

    public UnityEvent<string> getPlayfabIdEventArg1;
    public UnityEvent<string, string> getPlayfabIdEventArg2;
    public UnityEvent<string> getLeaderBoardUserIDEvent;
    public UnityEvent<int> getLeaderBoardValueEvent;

    public UnityEvent<string, string> getUserDataEvent;
    public UnityEvent invitingGroupEvent;
    public event GetPlayerInfoEvent getPlayerInfoEvent;
    #endregion

    void Awake()
    {
        if(instance == null)
        {
            instance = gameObject.GetComponent<PlayfabManager>();
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        networkManager = NetworkManager.Instance;
    }

    public void ClickStudentToggle()
    {
        instructorToggle.isOn = false;
        studentToggle.isOn = true;
    }
    public void ClickInstructorToggle()
    {
        studentToggle.isOn = false;
        instructorToggle.isOn = true;
    }

#region PlayFab Login, Register
    public void LoginBtn()
    {
        if (!emailInput.text.Contains("@inha.edu"))
        {
            emailInput.text += "@inha.edu";
        }
        var request = new LoginWithEmailAddressRequest { Email = emailInput.text, Password = passwordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { OnLoginSuccess(result);  }, (error) => OnLoginFailure(error));
    }

    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = emailInput.text, Password = passwordInput.text, Username = emailInput.text.Substring(0, 8), DisplayName = usernameInput.text };
        SelectJob();
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { OnRegisterSuccess(result); }, (error) => OnRegisterFailure(error));
    }

    public void LoginMaster()
    {
        SelectJob();

        if (playerJob == "")
        {
            Debug.Log("직업을 선택하세요.");
            return;
        }
        if (playerJob.Equals("학생"))
        {
            emailInput.text = "11111111@inha.edu";
            passwordInput.text = "master1234";
        }
        else
        {
            emailInput.text = "22222222@inha.edu";
            passwordInput.text = "master4321";
        }
        LoginBtn();
    }

    private void OnLoginSuccess(LoginResult result)
    {
        GetMyInfo();
        GetUserJob(); 
        myPlayfabId = result.PlayFabId;

        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(),
            (result) =>
            {
                playerEntity = result.Entity;
                AcceptGroupInvitationWithUpdateData();
                Debug.Log("엔터티 로드 성공" + playerEntity.Id);
            },
            (error) => Debug.Log("엔터티 로드 실패"));

        invitingGroupEvent.AddListener(AcceptGroupInvitationWithUpdateData);

        Debug.Log("로그인 성공, 서버에 연결합니다.");

        emailInput.text = "";
        passwordInput.text = "";
        usernameInput.text = "";

        //        PN.ConnectUsingSettings();
        PN.ConnectUsingSettings();
        UpdateLeaderBoard("CSE1111001Attendance", 24511);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("로그인 실패" + error);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SetUserData("Job", playerJob);
        UpdateLeaderBoard("IDInfo", int.Parse(emailInput.text.Substring(0, 8)));

        Debug.Log("회원가입 성공");
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("회원가입 실패" + error);
    }
#endregion

    public void SetUserData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { key, value } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("값 저장 성공"); }, (error) => Debug.Log("값 저장 실패" + error));
    }

    public void GetUserData(string key, string use)
    {
        var request = new GetUserDataRequest { Keys = new List<string>() { key } };
        PlayFabClientAPI.GetUserData(request,
            (result) =>
            {
                if (use.Equals("JoinToClass"))
                {
                    NetworkManager.Instance.JoinToClass(key, result.Data[key].Value);
                }
                else if (use.Equals("InstantiateClassInfo"))
                {
                    UIManager.Instance.InstantiateClassInfo(key, result.Data[key].Value);
                }
            }, (error) => { Debug.Log(error); });
    }

    public void DeleteUserData(string key)
    {
        var request = new UpdateUserDataRequest
        {
            KeysToRemove = new List<string>() { key }
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("값 삭제 성공"); }, (error) => Debug.Log("값 삭제 실패" + error));
    }

    

    void GetUserJob()
    {
        var request = new GetUserDataRequest { PlayFabId = myPlayfabId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var eachData in result.Data)
            {
                if (eachData.Key == "Job")
                {
                    if (eachData.Value.Value == "학생")
                    {
                        playerJob = "학생";
                    }
                    else if (eachData.Value.Value == "교수")
                    {
                        playerJob = "교수";
                    }
                    break;
                }
            }

            
            Debug.Log("데이터 로드 성공. Job : " + playerJob);
        }, (error) => Debug.Log("데이터 로드 실패" + error));
    }

    public void GetMyInfo()
    {
        var request = new GetAccountInfoRequest { PlayFabId = myPlayfabId };
        PlayFabClientAPI.GetAccountInfo(request,
            (result) =>
            {
                playerName = result.AccountInfo.TitleInfo.DisplayName;
                playerSchoolId = result.AccountInfo.Username;

                UpdateLeaderBoard("Login", 1);

                UIManager.Instance.UIInitFromPlayfabLogin(playerName, playerSchoolId);
                ChatManager.Instance.ChatStart();

                ChatManager.Instance.enabled = true;
                Debug.Log("플레이어 정보 로드 성공, 이름 : " + playerName + ", 학번 : " + playerSchoolId);
            }, (error) => Debug.Log("플레이어 정보 로드 실패"));
    }

    public void GetPlayerInfoUsingPlayFabId(string playerId)
    {
        var request = new GetAccountInfoRequest { PlayFabId = playerId };
        PlayFabClientAPI.GetAccountInfo(request,
            (result) =>
            {
                if (getPlayerInfoEvent != null)
                {
                    getPlayerInfoEvent(result.AccountInfo.Username, result.AccountInfo.TitleInfo.DisplayName);
                }
                Debug.Log("플레이어 정보 로드 성공");
            }, (error) => Debug.Log("플레이어 정보 로드 실패"));
    }

    public void GetPlayerInfoUsingStudentId(string studentIdString)
    {
        int studentId = int.Parse(studentIdString);
        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "IDInfo",
            MaxResultsCount = 100,
            ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            string playerId = "";
            for (int count = 0; count < result.Leaderboard.Count; count++)
            {
                if (result.Leaderboard[count].StatValue.Equals(studentId))
                {
                    playerId = result.Leaderboard[count].PlayFabId;
                    break;
                }
            }
            if (playerId.Equals(""))
            {
                Debug.Log("존재하지 않는 사용자입니다.");
                return;
            }
            var request = new GetAccountInfoRequest { PlayFabId = playerId };
            PlayFabClientAPI.GetAccountInfo(request,
                (result) =>
                {
                    if (getPlayerInfoEvent != null)
                    {
                        getPlayerInfoEvent(result.AccountInfo.Username, result.AccountInfo.TitleInfo.DisplayName);
                    }
                    Debug.Log("플레이어 정보 로드 성공");
                }, (error) => Debug.Log("플레이어 정보 로드 실패"));
        }, (error) => Debug.Log(error.ErrorMessage));
    }

    public void GetPlayfabIdUsingStudentId(string studentIdString)
    {
        int studentId = int.Parse(studentIdString);
        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "IDInfo",
            MaxResultsCount = 100,
            ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            for (int count = 0; count < result.Leaderboard.Count; count++)
            {
                if (result.Leaderboard[count].StatValue.Equals(studentId))
                {
                    getPlayfabIdEventArg1.Invoke(result.Leaderboard[count].PlayFabId);
                    break;
                }
            }
        }, (error) => Debug.Log(error.ErrorMessage));
    }

    public void UpdateLeaderBoard(string statisticName, int statisticValue)
    {

        Debug.Log(statisticName + statisticValue);
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = statisticName, Value = statisticValue } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => Debug.Log("리더보드 업데이트 성공"), (error) => Debug.Log("리더보드 업데이트 실패")); ;
    }

    public void UpdateLeaderBoardStudentOnClass(string groupId, string groupType)
    {
        var request = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = groupId, Type = groupType } };
        PlayFabDataAPI.GetObjects(request,
            (objectResult) =>
            {
                ClassData classData = JsonUtility.FromJson<ClassData>(objectResult.Objects["ClassData"].DataObject.ToString());
                Debug.Log(classData.classId + classData.classNumber);
                UpdateLeaderBoard(classData.classId + classData.classNumber + "Attendance", 0);
            },
            (error) => { Debug.Log("시간표 갱신 실패" + error); });
    }

    public void GetLeaderBoard(string statisticName, string userName, string callbackMethodName)
    {
        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = statisticName,
            MaxResultsCount = 100,
            ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            if (callbackMethodName.Equals("CountStudentNumber"))
            {
                Debug.Log(statisticName + " " + result.Leaderboard.Count);
                ClassProcessManager.Instance.CountStudentNumber(result.Leaderboard.Count);
                return;
            }
            for (int count = 0; count < result.Leaderboard.Count; count++)
            {
                if (result.Leaderboard[count].DisplayName.Equals(userName))
                {
                    if (callbackMethodName.Equals("LoadAttendanceCount"))
                    {
                        ClassProcessManager.Instance.LoadAttendanceCount(result.Leaderboard[count].StatValue);
                        return;
                    }
                    else if (callbackMethodName.Equals("UpdateAttendance"))
                    {
                        ClassProcessManager.Instance.UpdateAttendance(result.Leaderboard[count].StatValue);
                        return;
                    }
                }
            }
            if (callbackMethodName.Equals("UpdateAttendance"))
            {
                Debug.Log("UP");
                ClassProcessManager.Instance.UpdateAttendance(0);
                return;
            }

        }, (error) => Debug.Log(error.ErrorMessage));
    }

    public void GetLeaderBoardForTotalAttendanceUI(string statisticName, string userName, Text textComponent)
    {
        Debug.Log(statisticName);
        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = statisticName,
            MaxResultsCount = 100,
            ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetLeaderboard(request,
            (result) =>
            {
                for (int count = 0; count < result.Leaderboard.Count; count++)
                {
                    Debug.Log(result.Leaderboard[count].StatValue);
                    if (result.Leaderboard[count].DisplayName.Equals(userName))
                    {
                        string attendance = UtilityMethods.ReverseString(Convert.ToString(result.Leaderboard[count].StatValue, 2).ToString());
                        for(int attenCount = attendance.Length; attenCount < 16; attenCount++)
                        {
                            attendance = attendance + "0";
                        }
                        attendance = attendance.Replace("1", "O\t\t");
                        attendance = attendance.Replace("0", "X\t\t");
                        textComponent.text = attendance;
                        return;
                    }
                }
                textComponent.alignment = TextAnchor.MiddleCenter;
                textComponent.text = "출석 정보가 존재하지 않습니다.";
            }, (error) => { Debug.Log(error); });
    }

    void SelectJob()
    {
        if (studentToggle.isOn)
        {
            playerJob = "학생";
        }
        else if (instructorToggle.isOn)
        {
            playerJob = "교수";
        }
    }

    public void CreateGroup(string groupName)
    {
        PlayFab.GroupsModels.EntityKey entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type };

        var request = new CreateGroupRequest { GroupName = groupName, Entity = entity };
        PlayFabGroupsAPI.CreateGroup(request,
            (result) =>
            {
                Debug.Log("그룹 생성 성공, id : " + result.Group.Id + " name : " + result.GroupName);
            },
            (error) => Debug.Log("그룹 생성 실패" + error));
    }

    public void CreateGroup(string groupName, string dataKey, object dataValue)
    {
        PlayFab.GroupsModels.EntityKey entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type };

        var request = new CreateGroupRequest { GroupName = groupName, Entity = entity };
        PlayFabGroupsAPI.CreateGroup(request,
            (result) =>
            {
                Debug.Log("그룹 생성 성공, id : " + result.Group.Id + " name : " + result.GroupName);
                List<string> studentIds = UtilityMethods.ListUpInvitingStudents(dataValue);
                for(int count = 0; count < studentIds.Count; count++)
                {
                    InviteToGroup(groupName, studentIds[count]);
                    Debug.Log(studentIds[count] + "가 수업에 참여했습니다.");
                }
                UpdateObjectDataUsingEntity(result.Group.Id, result.Group.Type, dataKey, dataValue);
            },
            (error) =>
            {
                Debug.Log("그룹 생성 실패" + error.Error);
                if (error.Error.ToString().Equals("GroupNameNotAvailable"))
                {
                    var request = new GetGroupRequest { GroupName = groupName };
                    PlayFabGroupsAPI.GetGroup(request,
                        (result) =>
                        {
                            UpdateObjectDataUsingEntity(result.Group.Id, result.Group.Type, dataKey, dataValue);
                            Debug.Log("그룹 개체 업데이트 성공");
                        }, (error) => { });
                    List<string> studentIds = UtilityMethods.ListUpInvitingStudents(dataValue);
                    for (int count = 0; count < studentIds.Count; count++)
                    {
                        InviteToGroup(groupName, studentIds[count]);
                        Debug.Log(studentIds[count] + "가 수업에 참여했습니다.");
                    }
                }
            });
    }

    public void GetGroupList(string use)
    {
        var groups = new List<GroupWithRoles>();
        var request = new ListMembershipRequest { Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type } };
        PlayFabGroupsAPI.ListMembership(request,
            (result) =>
            {
                groups = result.Groups.ToList();
                Debug.Log("그룹 목록 불러오기 성공. Count = " + groups.Count);
                if (use.Equals("OpenClassListWindow"))
                    UIManager.Instance.OpenClassListWindowCallBack(groups);
                else if (use.Equals("ShowClassInfo"))
                    UIManager.Instance.ShowClassInfoBtnCallBack(groups);
            },
            (error) => { Debug.Log("그룹 목록 불러오기 실패 " + error); groups = null; });
    }

    public void InviteToGroup(string groupName, string studentIdString)
    {
        PlayFab.GroupsModels.EntityKey groupEntity = new PlayFab.GroupsModels.EntityKey();

        PlayFabGroupsAPI.GetGroup(new GetGroupRequest { GroupName = groupName },
            (result) =>
            {
                groupEntity = result.Group;
                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { Username = studentIdString },
                    (result) => {
                        var request = new InviteToGroupRequest
                        {
                            Group = groupEntity,
                            Entity = new PlayFab.GroupsModels.EntityKey { Id = result.AccountInfo.TitleInfo.TitlePlayerAccount.Id, Type = result.AccountInfo.TitleInfo.TitlePlayerAccount.Type },
                            RoleId = "members"
                        };
                        PlayFabGroupsAPI.InviteToGroup(request,
                            (invitationResult) =>
                            {
                                Debug.Log("Invited!");
                                invitingGroupEvent.Invoke();
                            }, (error) => { Debug.Log(error); });

                    }, (error) => { });
            }, (error) => Debug.Log(error.ErrorMessage));
    }

    public void AcceptGroupInvitation()
    {
        var request = new ListMembershipOpportunitiesRequest { Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type } };
        PlayFabGroupsAPI.ListMembershipOpportunities(request, 
            (result) => 
            { 
                for(int count = 0; count < result.Invitations.Count; count++)
                {
                    var request = new AcceptGroupInvitationRequest {
                        Group = result.Invitations[count].Group,
                        Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type }
                    };
                    PlayFabGroupsAPI.AcceptGroupInvitation(request, (result) => { Debug.Log("그룹 가입 성공"); }, (error) => { Debug.Log("그룹 가입 실패 " + error); });
                }
            }, (error) => { Debug.Log("리스트업 실패 " + error); });
    }

    public void AcceptGroupInvitationWithUpdateData()
    {
        var request = new ListMembershipOpportunitiesRequest { Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type } };
        PlayFabGroupsAPI.ListMembershipOpportunities(request,
            (listResult) =>
            {
                for (int count = 0; count < listResult.Invitations.Count; count++)
                {
                    var request = new AcceptGroupInvitationRequest
                    {
                        Group = listResult.Invitations[count].Group,
                        Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type }
                    };
                    PlayFabGroupsAPI.AcceptGroupInvitation(request, 
                        (acceptResult) =>
                        {
                            for (int count = 0; count < listResult.Invitations.Count; count++)
                            {
                                Debug.Log("그룹 가입 성공");
                                UpdateClassTimeTable(listResult.Invitations[count].Group.Id, listResult.Invitations[count].Group.Type);
                                UpdateLeaderBoardStudentOnClass(listResult.Invitations[count].Group.Id, listResult.Invitations[count].Group.Type);
                            }
                        }, (error) => { Debug.Log("그룹 가입 실패 " + error); });
                }
            }, (error) => { Debug.Log("리스트업 실패 " + error); });
    }

    public void RemoveMemberFromGroup(string groupName, string playerName)
    {
        Debug.Log("Remove " + playerName + " from " + groupName);
        var getGroupRequest = new GetGroupRequest { GroupName = groupName };
        PlayFabGroupsAPI.GetGroup(getGroupRequest,
            (getGroupResult) =>
            {
                var getUserRequest = new GetAccountInfoRequest { TitleDisplayName = playerName };
                PlayFabClientAPI.GetAccountInfo(getUserRequest,
                    (getUserResult) =>
                    {
                        Debug.Log(getUserResult.AccountInfo.TitleInfo.TitlePlayerAccount.Id + " " + getUserResult.AccountInfo.TitleInfo.TitlePlayerAccount.Type);
                        var removeMemberRequest = new RemoveMembersRequest
                        {
                            Group = { Id = getGroupResult.Group.Id, Type = getGroupResult.Group.Type },
                            Members = new List<PlayFab.GroupsModels.EntityKey>
                            {
                                new PlayFab.GroupsModels.EntityKey { Id = getUserResult.AccountInfo.TitleInfo.TitlePlayerAccount.Id, Type = getUserResult.AccountInfo.TitleInfo.TitlePlayerAccount.Type }
                            }
                        };
                        PlayFabGroupsAPI.RemoveMembers(removeMemberRequest,
                            (result) =>
                            {
                                Debug.Log("그룹 내 플레이어 삭제 성공");
                            }, (error) => { Debug.Log(error.Error); });
                    }, (error) => { Debug.Log(error.Error); });
            }, (error) => { Debug.Log("그룹 불러오기 실패" + error.Error); });
        
    }

    void UpdateClassTimeTable(string entityId, string entityType)
    {
        var request = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType } };
        PlayFabDataAPI.GetObjects(request,
            (objectResult) =>
            {
                ClassData classData = JsonUtility.FromJson<ClassData>(objectResult.Objects["ClassData"].DataObject.ToString());
                if (!classData.secondEndTime.Equals(""))
                    SetUserData(classData.className + classData.classNumber,
                        classData.classNumber + "," + classData.classId + "," + classData.classInstructor + "," + 
                        classData.firstDayOfWeek + "," + classData.firstStartTime + "~" + classData.firstEndTime + "," +
                        classData.secondDayOfWeek + "," + classData.secondStartTime + "~" + classData.secondEndTime);
                else
                    SetUserData(classData.className + classData.classNumber,
                        classData.classNumber + "," + classData.classId + "," + classData.classInstructor + "," + 
                        classData.firstDayOfWeek + "," + classData.firstStartTime + "~" + classData.firstEndTime);
                Debug.Log("시간표 갱신 성공");
            },
            (error) => { Debug.Log("시간표 갱신 실패" + error); });
    }

    public GroupWithRoles FindSpecificGroup(List<GroupWithRoles> groups, string groupName)
    {
        GroupWithRoles result = new GroupWithRoles();
        for(int count = groups.Count; count > 0; count--)
        {
            if(groups[count].GroupName == groupName)
            {
                result = groups[count];
            }
        }
        return result;
    }

    public void UpdateObjectDataUsingEntity(string entityId, string entityType, string key, object value)
    {
        List<SetObject> setObjectsList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = key,
                DataObject = value
            }
        };

        var request = new SetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType }, Objects = setObjectsList };
        PlayFabDataAPI.SetObjects(request, 
            (result) => {
                Debug.Log("데이터 업데이트 성공");
                UpdateClassTimeTable(entityId, entityType);
            }, (error) => Debug.Log("데이터 업데이트 실패" + error));
    }

    public void GetObjectData(string use, string entityId, string entityType, string key)
    {
        object returnObject = new object();
        var request = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType } };
        PlayFabDataAPI.GetObjects(request,
            (result) =>
            {
                if (use.Equals("ClassData"))
                {
                    UIManager.Instance.LoadModifyingClass(result.Objects[key].DataObject);
                }
            },
            (error) => { Debug.LogError(error.GenerateErrorReport()); });
    } 
}