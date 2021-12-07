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
    public InputField browserChannelNameInputField;

    public void StartVideoConferenceBtn()
    {
        conferenceState = Define.VIDEOCONFERENCESTATE.READY;
        photonView.RPC("UpdateStateVideoConference", RpcTarget.AllBuffered, channelName, conferenceState);
        Application.OpenURL("https://owake.me/");
    }

    [PunRPC]
    public void UpdateStateVideoConference(string sender_channel_name, Define.VIDEOCONFERENCESTATE sender_conference_state)
    {
        Debug.Log("Receiver " + sender_channel_name + " " + sender_conference_state);
        if (sender_channel_name.Equals(channelName))
        {
            if (sender_conference_state.Equals(Define.VIDEOCONFERENCESTATE.READY))
            {
                Debug.Log("ReadyVideoConference");
                UIManager.Instance.videoConferenceButton.interactable = false;
                UIManager.Instance.videoConferenceText.text = "회의 생성 중...";
            }
            else if (sender_conference_state.Equals(Define.VIDEOCONFERENCESTATE.START))
            {
                Debug.Log("StartVideoConference");
                UIManager.Instance.conferenceChannelNameText.text = sender_channel_name;
                UIManager.Instance.conferenceChannelNameObject.SetActive(true);

                UIManager.Instance.videoConferenceButton.interactable = true;
                UIManager.Instance.videoConferenceText.text = "화상회의 참여";
            }
            else
            {
                Debug.Log("EndVideoConference");
                UIManager.Instance.conferenceChannelNameText.text = "";
                UIManager.Instance.conferenceChannelNameObject.SetActive(false);

                UIManager.Instance.videoConferenceButton.interactable = true;
                UIManager.Instance.videoConferenceText.text = "화상회의 시작";

            }
        }
    }

    public void UpdateConferenceState()
    {
        for (int idx = 0; idx < 4; idx++)
        {
            GameObject.Find(ChatManager.Instance.currentChannelName).transform.Find($"IT_chair{4 - idx}").GetComponent<MeshCollider>().isTrigger = true;
        }
        GameObject.Find(ChatManager.Instance.currentChannelName).transform.Find("table").GetComponent<MeshCollider>().isTrigger = true;
        photonView.RPC("UpdateConferenceStateRPC", RpcTarget.AllBuffered, browserChannelNameInputField.text);
    }

    [PunRPC]
    public void UpdateConferenceStateRPC(string browserChannelName)
    {
        ChatClient client = ChatManager.Instance.chatClient;
        if (client.PublicChannels.ContainsKey(ChatManager.Instance.currentChannelName))
        {
            UIManager.Instance.conferenceMemberText.text = "[" + "회의실" + "] " +
                client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers.Count + " / " + 
                client.PublicChannels[ChatManager.Instance.currentChannelName].MaxSubscribers;

            foreach(var name in client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers)
            {
                if(!players.Contains(GameObject.Find(name)))
                {
                    players.Add(GameObject.Find(name));
                }
            }

            if(browserChannelName != "")
            {
                browserChannelNameInputField.text = browserChannelName;
            }

            Vector3 conferencePos = GameObject.Find(ChatManager.Instance.currentChannelName).transform.position - GameObject.Find("Conference001").transform.position;
            conferenceWorldTransform.position = conferenceWorldOffset + conferencePos;

            for (int idx = 0; idx < players.Count; idx++)
            {
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

            if (!conferenceChannelName.Equals(""))
            {
                Debug.Log("Sender");
                photonView.RPC("UpdateStateVideoConference", RpcTarget.AllBuffered, channelName, conferenceState);
            }
        }
    }

    public void ExitConference()
    {
        photonView.RPC("ExitConferenceRPC", RpcTarget.AllBuffered);
        UIManager.Instance.ShowUI(Define.UI.HUD);
        ChatManager.Instance.ExitConference();
        VoiceManager.Instance.EnterLobbyChannel();

    }

    [PunRPC]
    public void ExitConferenceRPC()
    {
        if (players.Contains(GameObject.Find(PlayfabManager.Instance.playerName)))
        {
            players.Remove(GameObject.Find(PlayfabManager.Instance.playerName));
            browserChannelNameInputField.text = "";
        }
    }

    public void ChangeInputField()
    {
        photonView.RPC("ChangeInputFieldRPC", RpcTarget.AllBuffered, browserChannelNameInputField.text);
    }

    [PunRPC]
    public void ChangeInputFieldRPC(string browserChannelName)
    {
        if (browserChannelName != "")
        {
            browserChannelNameInputField.text = browserChannelName;
        }
    }

    public void OnButtonOpenBrowser()
    {
        Application.OpenURL("https://owake.me/");
    }
}
