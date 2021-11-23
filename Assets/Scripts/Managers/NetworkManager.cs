using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PN = Photon.Pun.PhotonNetwork;

public class NetworkManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    private NetworkManager() { }

    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<NetworkManager>();
                if (obj != null)
                    instance = obj;
            }

            return instance;
        }
    }

    public GameObject cameraArm;
    GameObject player;
    GameObject playerNameTextUI;

    string network_state;
    public string room_name;
    Vector3 lastPosition = Vector3.zero;

    private void Awake()
    {
        AttendanceTable table = new AttendanceTable("Table", Define.ATTENDANCE.ATTENDANCE);
        if(instance == null)
        {
            instance = gameObject.GetComponent<NetworkManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        room_name = "Campus";
    } 

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        PN.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PN.JoinOrCreateRoom(room_name, new RoomOptions { MaxPlayers = 10 }, null);
    }

    public void JoinToClass(string className, string classData)
    {
        PlayfabManager.Instance.getUserDataEvent.RemoveListener(JoinToClass);

        string[] splitedTimeTableData = UtilityMethods.SplitTimeTableUserData(classData);

        PN.LeaveRoom();
        ChatManager.Instance.LeaveChat();
        lastPosition = player.transform.position;
        SceneManager.LoadScene("ClassroomScene");
        if(UtilityMethods.DetermineAllowClassEnter(splitedTimeTableData))
            room_name = className;
        else
        {
            room_name = "OpenClass";
            Debug.Log(className + " 수업 시간이 아닙니다. 공개 수업방으로 입장합니다.");
        }
    }

    public void JoinToCampus()
    {
        Destroy(ChatManager.Instance.gameObject);
        PN.LeaveRoom();
        ChatManager.Instance.LeaveChat();

        SceneManager.LoadScene("SampleScene");
        room_name = "Lobby";
        player.transform.position = lastPosition;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PN.CurrentRoom);
        GameStart();
    }

    public void OnLeaveLobby()
    {
        PN.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        PlayfabManager.Instance.UpdateLeaderBoard("Login", 0);
        Debug.Log("플레이어가 퇴장했습니다.");
    }

    void GameStart()
    {
        SpawnPlayer();
        UIManager.Instance.ShowUI(Define.UI.HUD);
        cameraArm.GetComponent<CameraController>().enabled = true;
        ChatManager.Instance.ChatStart();
    }

    void SpawnPlayer()
    {
        if (player == null)
        {
            if (room_name != "Lobby")
            {
                player = PN.Instantiate("Player", Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.Log("Lobby" + lastPosition);
                player = PN.Instantiate("Player", lastPosition, Quaternion.identity);
            }
            player.name = PlayfabManager.Instance.playerName;
        }
        else
        {
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }
    }

    void Update()
    {
        string curnetwork_state = PN.NetworkClientState.ToString();
        if (network_state != curnetwork_state)
        {
            network_state = curnetwork_state;
//            print(network_state);
        }
    }
}
