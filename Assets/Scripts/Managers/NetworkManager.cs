using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using PN = Photon.Pun.PhotonNetwork;

public class NetworkManager : MonoBehaviourPunCallbacks
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
                if (obj == null)
                    instance = obj;
            }

            return instance;
        }
    }

    public GameObject cameraArm;
    public GameObject chatController;
    GameObject player;

    string network_state;
    string room_name;


    private void Awake()
    {
        if(instance == null)
        {
            instance = gameObject.GetComponent<NetworkManager>();
            DontDestroyOnLoad(gameObject);
        }
        room_name = "Lobby";
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

        string[] splitedTimeTableData = UtilityMethods.SplitTimeTableData(classData);

        PN.LeaveRoom();
        chatController.GetComponent<ChatController>().LeaveChat();

        SceneManager.LoadScene("ClassroomScene");
        if(UtilityMethods.DetermineAllowClassEnter(splitedTimeTableData))
            room_name = className;
        else
        {
            room_name = "OpenClass";
            Debug.Log(className + " 수업 시간이 아닙니다. 공개 수업방으로 입장합니다.");
        }
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
        Debug.Log("플레이어가 퇴장했습니다.");
    }

    void GameStart()
    {
        SpawnPlayer();
        UIManager.Instance.ShowUI(Define.UI.HUD);
        cameraArm.GetComponent<CameraController>().enabled = true;
        chatController.GetComponent<ChatController>().ChatStart();
    }

    void SpawnPlayer()
    {
        if (player == null)
        {
            player = PN.Instantiate("Player", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            DontDestroyOnLoad(player);
        }
        else
        {
            player.transform.position = new Vector3(0, 0, 0);
            player.transform.rotation = new Quaternion(0, 0, 0, 0);
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
