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
using UnityEngine.EventSystems;

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
        UIManager.Instance.ShowUI(Define.UI.PORTRAIT);
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
            Debug.Log("������ �����ϼ���.");
            return;
        }
        if (playerJob.Equals("�л�"))
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

    public void OnToggleCharacter()
    {
        NetworkManager.Instance.characterName = EventSystem.current.currentSelectedGameObject.name;
    }

    public void SelectCharacterBtn()
    {
        if (!emailInput.text.Contains("@inha.edu"))
        {
            emailInput.text += "@inha.edu";
        }
        Debug.Log(emailInput.text + " " + passwordInput.text); 
        var request = new LoginWithEmailAddressRequest { Email = emailInput.text, Password = passwordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { OnLoginSuccess(result); }, (error) => OnLoginFailure(error));
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
            },
            (error) => Debug.Log("����Ƽ �ε� ����"));

        invitingGroupEvent.AddListener(AcceptGroupInvitationWithUpdateData);


        emailInput.text = "";
        passwordInput.text = "";
        usernameInput.text = "";

        PN.ConnectUsingSettings();
        UpdateLeaderBoard("CSE1111001Attendance", 4063);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("�α��� ����" + error);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SetUserData("Job", playerJob);
        UpdateLeaderBoard("IDInfo", int.Parse(emailInput.text.Substring(0, 8)));

        Debug.Log("ȸ������ ����");
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("ȸ������ ����" + error);
    }
#endregion

    public void SetUserData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { key, value } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("�� ���� ����"); }, (error) => Debug.Log("�� ���� ����" + error));
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
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("�� ���� ����"); }, (error) => Debug.Log("�� ���� ����" + error));
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
                    if (eachData.Value.Value.Equals("�л�"))
                    {
                        playerJob = "�л�";
                    }
                    else if (eachData.Value.Value == "����")
                    {
                        playerJob = "����";
                    }
                    break;
                }
            }

            
            Debug.Log("������ �ε� ����. Job : " + playerJob);
        }, (error) => Debug.Log("������ �ε� ����" + error));
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
                Debug.Log("�÷��̾� ���� �ε� ����, �̸� : " + playerName + ", �й� : " + playerSchoolId);
            }, (error) => Debug.Log("�÷��̾� ���� �ε� ����"));
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
                Debug.Log("�÷��̾� ���� �ε� ����");
            }, (error) => Debug.Log("�÷��̾� ���� �ε� ����"));
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
                Debug.Log("�������� �ʴ� ������Դϴ�?");
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
                    Debug.Log("�÷��̾� ���� �ε� ����");
                }, (error) => Debug.Log("�÷��̾� ���� �ε� ����"));
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
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => Debug.Log("�������� ������Ʈ ����"), (error) => Debug.Log("�������� ������Ʈ ����")); ;
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
            (error) => { Debug.Log("�ð�ǥ ���� ����" + error); });
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
                textComponent.text = "�⼮ ������ �������� �ʽ��ϴ�.";
            }, (error) => { Debug.Log(error); });
    }

    void SelectJob()
    {
        if (studentToggle.isOn)
        {
            playerJob = "�л�";
        }
        else if (instructorToggle.isOn)
        {
            playerJob = "����";
        }
    }

    public void CreateGroup(string groupName)
    {
        PlayFab.GroupsModels.EntityKey entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type };

        var request = new CreateGroupRequest { GroupName = groupName, Entity = entity };
        PlayFabGroupsAPI.CreateGroup(request,
            (result) =>
            {
                Debug.Log("�׷� ���� ����, id : " + result.Group.Id + " name : " + result.GroupName);
            },
            (error) => Debug.Log("�׷� ���� ����" + error));
    }

    public void CreateGroup(string groupName, string dataKey, object dataValue)
    {
        PlayFab.GroupsModels.EntityKey entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type };

        var request = new CreateGroupRequest { GroupName = groupName, Entity = entity };
        PlayFabGroupsAPI.CreateGroup(request,
            (result) =>
            {
                Debug.Log("�׷� ���� ����, id : " + result.Group.Id + " name : " + result.GroupName);
                List<string> studentIds = UtilityMethods.ListUpInvitingStudents(dataValue);
                for(int count = 0; count < studentIds.Count; count++)
                {
                    InviteToGroup(groupName, studentIds[count]);
                    Debug.Log(studentIds[count] + "�� ������ �����߽��ϴ�.");
                }
                UpdateObjectDataUsingEntity(result.Group.Id, result.Group.Type, dataKey, dataValue);
            },
            (error) =>
            {
                Debug.Log("�׷� ���� ����" + error.Error);
                if (error.Error.ToString().Equals("GroupNameNotAvailable"))
                {
                    var request = new GetGroupRequest { GroupName = groupName };
                    PlayFabGroupsAPI.GetGroup(request,
                        (result) =>
                        {
                            UpdateObjectDataUsingEntity(result.Group.Id, result.Group.Type, dataKey, dataValue);
                            Debug.Log("�׷� ��ü ������Ʈ ����");
                        }, (error) => { });
                    List<string> studentIds = UtilityMethods.ListUpInvitingStudents(dataValue);
                    for (int count = 0; count < studentIds.Count; count++)
                    {
                        InviteToGroup(groupName, studentIds[count]);
                        Debug.Log(studentIds[count] + "�� ������ �����߽��ϴ�.");
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
                Debug.Log("�׷� ���?�ҷ����� ����. Count = " + groups.Count);
                if (use.Equals("OpenClassListWindow"))
                    UIManager.Instance.OpenClassListWindowCallBack(groups);
                else if (use.Equals("ShowClassInfo"))
                    UIManager.Instance.ShowClassInfoBtnCallBack(groups);
            },
            (error) => { Debug.Log("�׷� ���?�ҷ����� ���� " + error); groups = null; });
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
                    PlayFabGroupsAPI.AcceptGroupInvitation(request, (result) => { Debug.Log("�׷� ���� ����"); }, (error) => { Debug.Log("�׷� ���� ���� " + error); });
                }
            }, (error) => { Debug.Log("����Ʈ�� ���� " + error); });
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
                                Debug.Log("�׷� ���� ����");
                                UpdateClassTimeTable(listResult.Invitations[count].Group.Id, listResult.Invitations[count].Group.Type);
                                UpdateLeaderBoardStudentOnClass(listResult.Invitations[count].Group.Id, listResult.Invitations[count].Group.Type);
                            }
                        }, (error) => { Debug.Log("�׷� ���� ���� " + error); });
                }
            }, (error) => { Debug.Log("����Ʈ�� ���� " + error); });
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
                                Debug.Log("�׷� �� �÷��̾� ���� ����");
                            }, (error) => { Debug.Log(error.Error); });
                    }, (error) => { Debug.Log(error.Error); });
            }, (error) => { Debug.Log("�׷� �ҷ����� ����" + error.Error); });
        
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
                Debug.Log("�ð�ǥ ���� ����");
            },
            (error) => { Debug.Log("�ð�ǥ ���� ����" + error); });
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
                Debug.Log("������ ������Ʈ ����");
                UpdateClassTimeTable(entityId, entityType);
            }, (error) => Debug.Log("������ ������Ʈ ����" + error));
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