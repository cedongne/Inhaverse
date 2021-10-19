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
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetUserData(); myId = result.PlayFabId; GetPlayerInfo();  OnLoginSuccess(result);  }, (error) => OnLoginFailure(error));
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
        /*
        if (job.Equals("�л�"))
        {
            var request = new LoginWithEmailAddressRequest { Email = "11111111@inha.edu", Password = "master1234" };
            PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetData(); OnLoginSuccess(result); }, (error) => OnLoginFailure(error));
        }
        else
        {
            var request = new LoginWithEmailAddressRequest { Email = "22222222@inha.edu", Password = "master4321" };
            PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetData(); OnLoginSuccess(result); }, (error) => OnLoginFailure(error));
        }
        */
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

    void GetUserData()
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

    void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();
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

    public void CreateGroup(string groupName, string dataKey, object dataValue)
    {
        PlayFab.GroupsModels.EntityKey entity = new PlayFab.GroupsModels.EntityKey { Id = playerEntity.Id, Type = playerEntity.Type };

        var request = new CreateGroupRequest { GroupName = groupName, Entity = entity };
        PlayFabGroupsAPI.CreateGroup(request, 
            (result) => 
            { 
                Debug.Log("�׷� ���� ����, id : " + result.Group.Id + " name : " + result.GroupName);
                if (!dataKey.Equals("null"))
                {
                    UpdateData(dataKey, result.Group.Id, result.Group.Type, dataValue);
                }
            }, 
            (error) => Debug.Log("�׷� ���� ����" + error)) ;


    }

    public void UpdateData(string key, string entityId, string entityType, object value)
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
        PlayFabDataAPI.SetObjects(request, (result) => Debug.Log("�׷� ������Ʈ ����"), (error) => Debug.Log("�׷� ������Ʈ ����" + error));
    }

    public void GetObjectData()
    {
        var request = new GetObjectsRequest { Entity = { Id = playerEntity.Id, Type = playerEntity.Type } };
        PlayFabDataAPI.GetObjects(request,
            (result) =>
            {
//                string jsonData = (result.Objects);
            },
    (error) => { Debug.LogError(error.GenerateErrorReport()); });
    }
}
