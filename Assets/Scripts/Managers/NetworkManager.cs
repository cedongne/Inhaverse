using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

using UnityEngine.UI;

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

    private void Awake()
    {
        if(instance == null)
        {
            instance = gameObject.GetComponent<NetworkManager>();
            DontDestroyOnLoad(gameObject);
        }
    }
    public GameObject cameraArm;
    

    string networkState;

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");
        PN.JoinLobby();
    }

    public override void OnJoinedLobby() =>
        PN.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 4 }, null);

    public override void OnCreatedRoom()
    {
        Debug.Log("Create");
    }
    public override void OnJoinedRoom()
    {
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
    }

    void SpawnPlayer()
    {
        GameObject player = PN.Instantiate("Player", new Vector3(0, 2, 0), new Quaternion(0, 0, 0, 0));
    }

    void Update()
    {
        string curNetworkState = PN.NetworkClientState.ToString();
        if (networkState != curNetworkState)
        {
            networkState = curNetworkState;
            print(networkState);
        }
    }
}
