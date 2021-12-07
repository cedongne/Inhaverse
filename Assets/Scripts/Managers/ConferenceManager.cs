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
    }

    public Define.VIDEOCONFERENCESTATE conferenceState;

    public string channelName;
    public string conferenceChannelName;

    public List<GameObject> players;
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
        for (int idx = 0; idx < 4; idx++)
        {
            GameObject.Find(ChatManager.Instance.currentChannelName).transform.Find($"IT_chair{4 - idx}").GetComponent<MeshCollider>().isTrigger = true;
        }
        GameObject.Find(ChatManager.Instance.currentChannelName).transform.Find("table").GetComponent<MeshCollider>().isTrigger = true;
        photonView.RPC("UpdateConferenceStateRPC", RpcTarget.AllBuffered);

        Vector3 conferencePos = GameObject.Find(ChatManager.Instance.currentChannelName).transform.position - GameObject.Find("Conference001").transform.position;
        conferenceWorldTransform.position = conferenceWorldOffset + conferencePos;
        for (int idx = 0; idx < players.Count; idx++)
        {
            if (players[idx].Equals(MineManager.Instance.player))
            {
                Debug.Log(idx);
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
                break;
            }
        }
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

            players.Clear();
            foreach (var name in client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers)
            {
                var obj = GameObject.Find(name);
                players.Add(obj);
            }
            players.Sort();

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

    public void ExitConference()
    {
        players.Clear();

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
        if (players.Contains(obj))
            players.Remove(obj);
    }
}