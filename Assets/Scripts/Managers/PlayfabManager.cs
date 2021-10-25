using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using PlayFab.DataModels;
using PlayFab.AuthenticationModels;
using UnityEngine.UI;

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

    string myId;
    public string playerJob;
    public string playerName;
    public string playerSchoolId;
    bool isAuthenticate;

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

    #region PlayFab Login, Register {
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = emailInput.text, Password = passwordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetUserJob(); myId = result.PlayFabId; GetPlayerInfo();  OnLoginSuccess(result);  }, (error) => OnLoginFailure(error));
    }

    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = emailInput.text, Password = passwordInput.text, Username = emailInput.text.Substring(0, 8), DisplayName = usernameInput.text };
        SelectJob();
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { OnRegisterSuccess(result); SetUserData("Job", playerJob); }, (error) => OnRegisterFailure(error));
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
        Debug.Log("�α��� ����");
        PN.ConnectUsingSettings();

        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(),
            (result) =>
            {
                playerEntity = result.Entity;
            },
            (error) => Debug.Log("����Ƽ �ε� ����"));
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("�α��� ����" + error);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("ȸ������ ����");
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("ȸ������ ����" + error);
    }

    #endregion
    void SetUserData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { key, value } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("�� ���� ����"); }, (error) => Debug.Log("�� ���� ����"));
    }

    void GetUserJob()
    {
        var request = new GetUserDataRequest { PlayFabId = myId };
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

    void GetPlayerInfo()
    {
        var request = new GetAccountInfoRequest { PlayFabId = myId };
        PlayFabClientAPI.GetAccountInfo(request,
            (result) =>
            {
                name = result.AccountInfo.TitleInfo.DisplayName;
                playerSchoolId = result.AccountInfo.Username;
                Debug.Log("�÷��̾� ���� �ε� ����, �̸� : " + name + ", �й� : " + playerSchoolId);
            }, (error) => Debug.Log("�÷��̾� ���� �ε� ����"));
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
                UpdateData(result.Group.Id, result.Group.Type, dataKey, dataValue);
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
                            UpdateData(result.Group.Id, result.Group.Type, dataKey, dataValue);
                            Debug.Log("�׷� ��ü ������Ʈ ����");
                        }, (error) => { });
                }
            });
    }

    public void GetGroupList()
    {
        var groups = new List<GroupWithRoles>();
        var request = new ListMembershipRequest { Entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type } };
        PlayFabGroupsAPI.ListMembership(request,
            (result) =>
            {
                groups = result.Groups.ToList();
                Debug.Log("�׷� ��� �ҷ����� ����. Count = " + groups.Count);
                UIManager.Instance.LoadMyClasses(groups);
            },
            (error) => { Debug.Log("�׷� ��� �ҷ����� ���� " + error); groups = null; });
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

    public void UpdateData(string entityId, string entityType, string key, object value)
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
                returnObject = result.Objects[key].DataObject;
                if (use.Equals("ClassData"))
                {
                    UIManager.Instance.LoadModifyingClass(returnObject);
                }
            },
            (error) => { Debug.LogError(error.GenerateErrorReport()); });
    }
    
}
