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

    string myId;
    public static string job;
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
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetData(); myId = result.PlayFabId; Debug.Log("myId : " +myId);  OnLoginSuccess(result);  }, (error) => OnLoginFailure(error));
    }

    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = emailInput.text, Password = passwordInput.text, Username = emailInput.text.Substring(0, 8), DisplayName = usernameInput.text };
        SelectJob();
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { OnRegisterSuccess(result); SetData("Job", job); }, (error) => OnRegisterFailure(error));
    }

    public void LoginMaster()
    {
        SelectJob();
        if (job.Equals("학생"))
        {
            var request = new LoginWithEmailAddressRequest { Email = "11111111@inha.edu", Password = "master1234" };
            PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetData(); OnLoginSuccess(result); }, (error) => OnLoginFailure(error));
        }
        else
        {
            var request = new LoginWithEmailAddressRequest { Email = "22222222@inha.edu", Password = "master4321" };
            PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { GetData(); OnLoginSuccess(result); }, (error) => OnLoginFailure(error));
        }
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공");
        PN.ConnectUsingSettings();

        PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(),
            (result) =>
            {
                playerEntity = result.Entity;
            },
            (error) => Debug.Log("엔터티 로드 실패"));
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("로그인 실패" + error);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("회원가입 성공");
    }

    void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("회원가입 실패" + error);
    }

    #endregion
    void SetData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { key, value } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("값 저장 성공"); }, (error) => Debug.Log("값 저장 실패"));
    }

    void GetData()
    {
        var request = new GetUserDataRequest { PlayFabId = myId };
        PlayFabClientAPI.GetUserData(request, (result) =>
        {
            foreach (var eachData in result.Data)
            {
                if (eachData.Key == "Job")
                {
                    if (eachData.Value.Value == "학생")
                    {
                        job = "학생";
                    }
                    else if (eachData.Value.Value == "교수")
                    {
                        job = "교수";
                    }
                    break;
                }
            }
            Debug.Log("데이터 로드 성공. Job : " + job);
        }, (error) => Debug.Log("데이터 로드 실패" + error));
    }

    void GetLeaderboard(string myID)
    {
        PlayFabUserList.Clear();
    }

    void SelectJob()
    {
        if (studentToggle.isOn)
        {
            job = "학생";
        }
        else if (instructorToggle.isOn)
        {
            job = "교수";
        }
    }

    void CreateGroup(string groupName, PlayFab.GroupsModels.EntityKey playerEntity, ClassData data)
    {
        
        PlayFab.GroupsModels.EntityKey entity = new PlayFab.GroupsModels.EntityKey { Id = myId, Type = "title_player_account" };
        Debug.Log("Create Group : " + entity.Id + " " + entity.Type);

        var request = new CreateGroupRequest { GroupName = "CSE" + Random.Range(0, 1000), Entity = entity };
        PlayFabGroupsAPI.CreateGroup(request, 
            (result) => 
            { 
                Debug.Log("그룹 생성 성공, id : " + result.Group.Id + " name : " + result.GroupName);
                entity.Id = result.Group.Id;
            }, 
            (error) => Debug.Log("그룹 생성 실패" + error)) ;
      //  PlayFab.GroupsModels.EntityKey groupEntity = new PlayFab.GroupsModels.EntityKey { Id = myId, Type = "title_player_account" };


    }

    void UpdateData(string entityId, string entityType, string jsonData)
    {
        SetObject setObject;
        var request = new SetObjectsRequest { Entity = { Id = entityId, Type = entityType } };
        PlayFabDataAPI.SetObjects(request, (result) => Debug.Log("그룹 업데이트 성공"), (error) => Debug.Log("그룹 업데이트 실패" + error));

    }
}
