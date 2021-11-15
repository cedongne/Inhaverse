using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using PlayFab.DataModels;
using PlayFab.AuthenticationModels;
using UnityEngine.UI;
using UnityEngine.Events;

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
                if(obj == null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();

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
    public UnityEvent<string> getLeaderBoardEvent;
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
    }

    private void Start()
    {
        networkManager = NetworkManager.Instance;
    }

#region PlayFab Login, Register
    public void LoginBtn()
    {
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

    private void OnLoginSuccess(LoginResult result)
    {
        GetUserJob(); 
        GetMyInfo();
        myPlayfabId = result.PlayFabId;

        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(),
            (result) =>
            {
                playerEntity = result.Entity;
                AcceptGroupInvitationWithUpdateData();
                Debug.Log("����Ƽ �ε� ����");
            },
            (error) => Debug.Log("����Ƽ �ε� ����"));

        invitingGroupEvent.AddListener(AcceptGroupInvitationWithUpdateData);

        Debug.Log("�α��� ����, ������ �����մϴ�.");

        PN.ConnectUsingSettings();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("�α��� ����" + error);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SetUserData("Job", playerJob); 
        UpdateStatistics("IDInfo", int.Parse(emailInput.text.Substring(0, 8)));

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

    public void GetUserData(string key, Define.USERDATAUSING use)
    {
        var request = new GetUserDataRequest { Keys = new List<string>() { key } };
        PlayFabClientAPI.GetUserData(request,
            (result) =>
            {
                if (use.Equals(Define.USERDATAUSING.JOINTOCLASS))
                {
                    getUserDataEvent.Invoke(key, result.Data[key].Value);
                }
                if (use.Equals(Define.USERDATAUSING.LOADCLASSINFO))
                {
                    UIManager.Instance.InstantiateClassInfo(key, result.Data[key].Value);
                }
            }, (error) => { });
    }

    public void DeleteUserData(string key)
    {
        var request = new UpdateUserDataRequest
        {
            KeysToRemove = new List<string>() { key }
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("�� ���� ����"); }, (error) => Debug.Log("�� ���� ����" + error));
    }

    public void UpdateStatistics(string statisticName, int statisticValue)
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = statisticName, Value = statisticValue } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => Debug.Log("�������� ������Ʈ ����"), (error) => Debug.Log("�������� ������Ʈ ����")); ;
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
                    if (eachData.Value.Value == "�л�")
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

                UIManager.Instance.playerName.text = playerName;
                UIManager.Instance.playerSchoolId.text = playerSchoolId;
//                photonView.Owner.NickName = playerName;
//                Debug.Log("Owner : " + photonView.Owner.NickName);

                GameObject.Find("ChatController").GetComponent<ChatController>().enabled = true;
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
                Debug.Log("�������� �ʴ� ������Դϴ�.");
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
    
    public void GetLeaderBoard(string statisticName, string statisticValue)
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
            for (int count = 0; count < result.Leaderboard.Count; count++)
            {
                if (result.Leaderboard[count].StatValue.Equals(statisticValue))
                {
                    getLeaderBoardEvent.Invoke(result.Leaderboard[count].PlayFabId);
                    break;
                }
            }
        }, (error) => Debug.Log(error.ErrorMessage));
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
                UpdateClassTimeTable(result.Group.Id, result.Group.Type);
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
                            UpdateClassTimeTable(result.Group.Id, result.Group.Type);
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

    public void GetGroupList(Define.GROUPLISTUSING use)
    {
        var groups = new List<GroupWithRoles>();
        var request = new ListMembershipRequest { Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type } };
        PlayFabGroupsAPI.ListMembership(request,
            (result) =>
            {
                groups = result.Groups.ToList();
                Debug.Log("�׷� ��� �ҷ����� ����. Count = " + groups.Count);
                if (use.Equals(Define.GROUPLISTUSING.MAKEBUTTONS))
                    UIManager.Instance.OpenClassListWindowCallBack(groups);
                else if (use.Equals(Define.GROUPLISTUSING.GETGROUPNAMES))
                    UIManager.Instance.InfoBtnCallBack(groups);
            },
            (error) => { Debug.Log("�׷� ��� �ҷ����� ���� " + error); groups = null; });
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
                            }
                        }, (error) => { Debug.Log("�׷� ���� ���� " + error); });
                }
            }, (error) => { Debug.Log("����Ʈ�� ���� " + error); });
    }

    public void QuitFromGroup(string groupId, string groupType, string entityId, string entityType)
    {
        var request = new RemoveGroupApplicationRequest
        {
            Group = new PlayFab.GroupsModels.EntityKey { Id = groupId, Type = groupType },
            Entity = new PlayFab.GroupsModels.EntityKey { Id = entityId, Type = entityType }
        };
        PlayFabGroupsAPI.RemoveGroupApplication(request, (result) => { Debug.Log("�׷� Ż�� ����"); }, (error) => { Debug.Log("�׷� Ż�� ����"); });
    }
    void UpdateClassTimeTable(string entityId, string entityType)
    {
        var request = new GetGroupRequest { Group = new PlayFab.GroupsModels.EntityKey{ Id = entityId, Type = entityType } };
        PlayFabGroupsAPI.GetGroup(request,
            (groupResult) =>
            {
                var request = new GetObjectsRequest { Entity = new PlayFab.DataModels.EntityKey { Id = entityId, Type = entityType } };
                PlayFabDataAPI.GetObjects(request,
                    (objectResult) =>
                    {
                        ClassData classData = JsonUtility.FromJson<ClassData>(objectResult.Objects["ClassData"].DataObject.ToString());
                        if (!classData.secondEndTime.Equals(""))
                            SetUserData(classData.className + classData.classNumber,
                                classData.classId + "," + classData.classInstructor + "," +
                                classData.firstDayOfWeek + "," + classData.firstStartTime + "~" + classData.firstEndTime + "," + 
                                classData.secondDayOfWeek + "," + classData.secondStartTime + "~" + classData.secondEndTime);
                        else
                            SetUserData(classData.className + classData.classNumber, classData.firstDayOfWeek + "," + classData.firstStartTime + "~" + classData.firstEndTime);
                        Debug.Log("�ð�ǥ ���� ����");
                    },
                    (error) => { Debug.Log("�ð�ǥ ���� ����" + error); });
            }, (error) => { });
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
        PlayFabDataAPI.SetObjects(request, (result) => Debug.Log("������ ������Ʈ ����"), (error) => Debug.Log("������ ������Ʈ ����" + error));
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