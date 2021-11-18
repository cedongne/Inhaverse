using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;

public class ConferenceManager : MonoBehaviourPun
{
    
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

    public string channelName;

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
            foreach (var channel in client.PublicChannels.Values)
            {
                Debug.Log(channel.Subscribers + ", " + channel.MaxSubscribers);
            }
            UIManager.Instance.ConferenceMemberText.text = "[" + "»∏¿«¿Â" + "] " +
           client.PublicChannels[ChatManager.Instance.currentChannelName].Subscribers.Count + " / " + 
           client.PublicChannels[ChatManager.Instance.currentChannelName].MaxSubscribers;
        }
    }
}
