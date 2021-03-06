using System.Collections;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
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

    public string room_name;
    Vector3 lastPosition = Vector3.zero;
    public string characterName = "Player";

    public bool connection = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<NetworkManager>();
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!room_name.Equals("Campus") && PlayfabManager.Instance.playerJob.Equals("????"))
        {
            ClassProcessManager.Instance.SomeoneJoinToClass();
        }
    }

    public void JoinToClass(string className, string classData)
    {
        PN.LeaveRoom();

        string[] splitedClassData = UtilityMethods.SplitTimeTableUserData(classData);

        if (UtilityMethods.DetermineAllowClassEnter(splitedClassData))
        {
            room_name = splitedClassData[1] + splitedClassData[0];
            Debug.Log(" ???? ?ð??Դϴ?. " + room_name + " ?????? ?????մϴ?.");
        }
        else
        {
            room_name = "OpenClass";
            Debug.Log(className + " ???? ?ð??? ?ƴմϴ?. ???? ?????????? ?????մϴ?.");
        }
        StartCoroutine("LoadClassRoomScene", "ClassroomScene");

        ChatManager.Instance.LeaveChat();
        lastPosition = player.transform.position;
        player.transform.position = Vector3.zero;
    }

    IEnumerator LoadClassRoomScene(string scene_name)
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(scene_name);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
 //           Debug.Log(asyncLoadScene.progress);
        }
        ClassProcessManager.Instance.enabled = true;
    }

    public void JoinToCampus()
    {
        room_name = "Campus";
        PN.LeaveRoom();
        ChatManager.Instance.LeaveChat();

        Destroy(ConferenceManager.Instance.gameObject);
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

    public void LeaveGame()
    {
        lastPosition = Vector3.zero;
        PN.Disconnect();
    }

    void GameStart()
    {
        SpawnPlayer();
        ChatManager.Instance.currentChannelName = room_name;
        ChatManager.Instance.ChatStart();
        UIManager.Instance.ShowUI(Define.UI.HUD);
    }

    void SpawnPlayer()
    {
        if (room_name.Equals("Campus"))
            player = PN.Instantiate(characterName, lastPosition, Quaternion.identity);
        else
            player = PN.Instantiate(characterName, Vector3.zero, Quaternion.identity);
        ConferenceManager.Instance.photonView.RequestOwnership();
    }
}
