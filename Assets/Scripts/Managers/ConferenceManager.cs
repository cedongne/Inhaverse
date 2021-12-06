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
    }

    public string channelName;

    public List<GameObject> players;
    public List<string> playerUrl;
    public List<RawImage> webCamImages;
    public Transform conferenceWorldTransform;
    public Vector3 conferenceWorldOffset;

    public GameObject table;

    public void UpdateConferenceState()
    {
        for (int idx = 0; idx < 4; idx++)
        {
            GameObject.Find(ChatManager.Instance.currentChannelName).transform.Find($"IT_chair{4 - idx}").GetComponent<MeshCollider>().isTrigger = true;
        }
        GameObject.Find(ChatManager.Instance.currentChannelName).transform.Find("table").GetComponent<MeshCollider>().isTrigger = true;
        photonView.RPC("UpdateConferenceStateRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void UpdateConferenceStateRPC()
    {
        ChatClient client = ChatManager.Instance.chatClient;
        if (client.PublicChannels.ContainsKey(ChatManager.Instance.currentChannelName))
        {
            UIManager.Instance.ConferenceMemberText.text = "[" + "ȸ�ǽ�" + "] " +
                client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers.Count + " / " + 
                client.PublicChannels[ChatManager.Instance.currentChannelName].MaxSubscribers;

            foreach(var name in client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers)
            {
                players.Add(GameObject.Find(name));
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
        }
    }

    public void ExitConference()
    {
        photonView.RPC("ExitConferenceRPC", RpcTarget.AllBuffered);
        UIManager.Instance.ShowUI(Define.UI.HUD);
        ChatManager.Instance.ExitConference();
        VoiceManager.Instance.EnterLobbyChannel();
        table.GetComponent<InteractiveConferenceTable>().mainCamera.enabled = true;
        table.GetComponent<InteractiveConferenceTable>().UICamera.enabled = false;

    }

    [PunRPC]
    public void ExitConferenceRPC()
    {
        if (players.Contains(GameObject.Find(PlayfabManager.Instance.playerName)))
        {
            players.Remove(GameObject.Find(PlayfabManager.Instance.playerName));
        }
    }
}
