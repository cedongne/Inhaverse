using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;

public class ConferenceManager : MonoBehaviourPunCallbacks
{
    private ConferenceManager() { }
    private static ConferenceManager instance;

    public static ConferenceManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<ConferenceManager>();
                if (obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<ConferenceManager>();
        }
        else
            Destroy(gameObject);
        conferenceState = Define.VIDEOCONFERENCESTATE.END;

        currentIndex = -1;
        players = new GameObject[4];
    }

    public Define.VIDEOCONFERENCESTATE conferenceState;

    public string channelName;
    public string conferenceChannelName;

    //    public List<GameObject> players;
    public GameObject[] players;
    public int currentIndex;

    public List<string> playerUrl;
    public List<RawImage> webCamImages;
    public Transform conferenceWorldTransform;
    public Vector3 conferenceWorldOffset;

    public GameObject table;

    public void StartVideoConferenceBtn()
    {
        conferenceState = Define.VIDEOCONFERENCESTATE.READY;
        photonView.RPC("ReadyVideoConference", RpcTarget.AllBuffered, channelName, conferenceState);
        Application.OpenURL("https://owake.me/");

        UIManager.Instance.conferenceChannelNameInputObject.SetActive(true);
    }

    public void EnterVideoConferenceChannelNameBtn()
    {
        conferenceChannelName = UIManager.Instance.conferenceChannelNameInputField.text;
        conferenceState = Define.VIDEOCONFERENCESTATE.START;
        photonView.RPC("StartVideoConference", RpcTarget.AllBuffered, channelName, conferenceChannelName);

        UIManager.Instance.conferenceChannelNameInputObject.SetActive(false);
    }

    [PunRPC]
    public void ReadyVideoConference(string sender_channel_name, Define.VIDEOCONFERENCESTATE sender_conference_state)
    {
        Debug.Log("Ready Receiver " + sender_channel_name + " " + sender_conference_state);
        if (sender_channel_name.Equals(channelName))
        {
            if (sender_conference_state.Equals(Define.VIDEOCONFERENCESTATE.READY))
            {
                Debug.Log("ReadyVideoConference");
                UIManager.Instance.videoConferenceButton.interactable = false;
                UIManager.Instance.videoConferenceText.text = "회의 생성 중...";
            }
        }
    }

    [PunRPC]
    public void StartVideoConference(string sender_channel_name, string conference_channel_name)
    {
        Debug.Log("Start Receiver " + sender_channel_name + " " + conference_channel_name);
        if (sender_channel_name.Equals(channelName))
        {
            Debug.Log("StartVideoConference");
            UIManager.Instance.conferenceChannelNameText.text = conference_channel_name;
            UIManager.Instance.conferenceChannelNameObject.SetActive(true);

            UIManager.Instance.videoConferenceButton.interactable = true;
            UIManager.Instance.videoConferenceText.text = "화상회의 참여";
        }
    }

    [PunRPC]
    public void EndVideoConference(string sender_channel_name)
    {
        Debug.Log("End Receiver " + sender_channel_name);
        if (sender_channel_name.Equals(channelName))
        {
            Debug.Log("EndVideoConference");
            conferenceChannelName = "";
            UIManager.Instance.conferenceChannelNameText.text = "";
            UIManager.Instance.conferenceChannelNameObject.SetActive(false);
            UIManager.Instance.conferenceChannelNameInputField.text = "";
            UIManager.Instance.conferenceChannelNameInputObject.SetActive(false);

            UIManager.Instance.videoConferenceButton.interactable = true;
            UIManager.Instance.videoConferenceText.text = "화상회의 시작";
        }
    }

    public void UpdateConferenceState()
    {
        photonView.RPC("UpdateConferenceStateRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void UpdateConferenceStateRPC()
    {
        ChatClient client = ChatManager.Instance.chatClient;
        if (client.PublicChannels.ContainsKey(ChatManager.Instance.currentChannelName))
        {
            UIManager.Instance.conferenceMemberText.text = "[" + "회의실" + "] " +
                client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers.Count + " / " + 
                client.PublicChannels[ChatManager.Instance.currentChannelName].MaxSubscribers;



            Vector3 conferencePos = GameObject.Find(ChatManager.Instance.currentChannelName).transform.position - GameObject.Find("Conference001").transform.position;
            conferenceWorldTransform.position = conferenceWorldOffset + conferencePos;
            if (client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers.Count == 1)
            {
                Debug.Log("AASDASDA");
                currentIndex = 0;
                players[currentIndex++] = MineManager.Instance.player;
                MineManager.Instance.player.transform.position = conferenceWorldTransform.position + ((currentIndex < 2) ? new Vector3(-0.5f + currentIndex, 0, 0) : new Vector3(0, 0, -0.5f + currentIndex % 2));
                MineManager.Instance.player.transform.LookAt(conferenceWorldTransform);
            }
            else if (currentIndex != -1)
            {
                photonView.RPC("PlayersArrayRPC", RpcTarget.AllBuffered, channelName, players, currentIndex);
            }

            /*
            if (photonView.IsMine)
            {
                for (int idx = 0; idx < players.Count; idx++)
                {
                    Debug.Log(idx + " " + players.Count + " " + players[idx].name);

                    players[idx].transform.position = conferenceWorldTransform.position;
                    if (idx == 0)
                    {
                        //IT_Chair4
                        players[idx].transform.position += new Vector3(-0.5f, 0, 0);
                    }
                    else if (idx == 1)
                    {
                        //IT_Chair3
                        players[idx].transform.position += new Vector3(0.5f, 0, 0);
                    }
                    else if (idx == 2)
                    {
                        //IT_Chair2
                        players[idx].transform.position += new Vector3(0, 0, -0.5f);
                    }
                    else if (idx == 3)
                    {
                        //IT_Chair1
                        players[idx].transform.position += new Vector3(0, 0, 0.5f);
                    }
                    players[idx].transform.LookAt(conferenceWorldTransform);
                }
            }
            */

            if (!conferenceChannelName.Equals(""))
            {
                if(conferenceState.Equals(Define.VIDEOCONFERENCESTATE.READY))
                    photonView.RPC("ReadyVideoConference", RpcTarget.AllBuffered, channelName, conferenceState);
                else if (conferenceState.Equals(Define.VIDEOCONFERENCESTATE.START))
                    photonView.RPC("StartVideoConference", RpcTarget.AllBuffered, channelName, conferenceChannelName);
                else
                    photonView.RPC("EndVideoConference", RpcTarget.AllBuffered, channelName);

            }
        }
    }

    [PunRPC]
    public void PlayersArrayRPC(string sender_channel_name, GameObject[] _players, int _currentIndex)
    {
        Debug.Log(sender_channel_name + " " + currentIndex);
        if (sender_channel_name.Equals(channelName) && currentIndex == -1)
        {
            Debug.Log("A");
            _players[_currentIndex] = MineManager.Instance.player;
            MineManager.Instance.player.transform.position = conferenceWorldTransform.position + ((currentIndex < 2) ? new Vector3(-0.5f + currentIndex, 0, 0) : new Vector3(0, 0, -0.5f + currentIndex % 2));
            MineManager.Instance.player.transform.LookAt(conferenceWorldTransform);
            for (int count = 0; count < 4; count++)
            {
                if (_players[count] == null)
                {
                    currentIndex = count;
                }
            }
            photonView.RPC("PlayersArrayRPC", RpcTarget.AllBuffered, sender_channel_name, _players, currentIndex);
        }
        else if (sender_channel_name.Equals(channelName))
        {
            players = _players;
            currentIndex = _currentIndex;
        }
    }

    public void ExitConference()
    {
        /*
        players.Clear();
        */
        photonView.RPC("ExitConferenceRPC", RpcTarget.AllBuffered, MineManager.Instance.player.name);
        UIManager.Instance.ShowUI(Define.UI.HUD);
        ChatManager.Instance.ExitConference();
        VoiceManager.Instance.EnterLobbyChannel();

        photonView.RPC("EndVideoConference", RpcTarget.AllBuffered, channelName);

        MineManager.Instance.playerController.OnKinematic(false);
        MineManager.Instance.playerController.canMove = true;
        MineManager.Instance.playerController.canDetectInteractive = true;
        MineManager.Instance.playerController.canGetInput = true;
    }

    [PunRPC]
    public void ExitConferenceRPC(string playerName)
    {
        ChatClient client = ChatManager.Instance.chatClient;
        var obj = GameObject.Find(playerName);
        /*
        if (players.Contains(obj))
            players.Remove(obj);
        */
    }
}