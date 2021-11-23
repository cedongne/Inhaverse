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
    public GameObject player;
    GameObject playerNameTextUI;

    string network_state;
    public string room_name;
    Vector3 lastPosition = Vector3.zero;

    public bool connection = false;

    private void Awake()
    {
        AttendanceTable table = new AttendanceTable("Table", Define.ATTENDANCE.ATTENDANCE);
        if(instance == null)
        {
            instance = gameObject.GetComponent<NetworkManager>();
            gameObject.SetActive(true);
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
        player.transform.position = Vector3.zero;

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
        Destroy(ConferenceManager.Instance.gameObject);
//        Destroy(FileManager.Instance.gameObject);
        room_name = "Campus";
        PN.LeaveRoom();
        ChatManager.Instance.LeaveChat();
        Debug.Log("JoinToCampus");
        player.transform.position = lastPosition;


        SceneManager.LoadScene("SampleScene");
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
        Debug.Log("GameStart");
        UIManager.Instance.ShowUI(Define.UI.HUD);
        cameraArm.GetComponent<CameraController>().enabled = true;
        ChatManager.Instance.ChatStart();
    }

    void SpawnPlayer()
    {
        player = PN.Instantiate("Player", Vector3.zero, Quaternion.identity);
        /*
        if (!connection)
        {
//            DontDestroyOnLoad(player);
        }
        else
        {
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }
        connection = true;
        */
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
