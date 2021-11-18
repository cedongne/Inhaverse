using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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

    public void UpdateConferenceState(string channelName)
    {
        if (ChatManager.Instance.chatClient.PublicChannels.ContainsKey(channelName))
        {
            UIManager.Instance.ConferenceMemberText.text = "[" + "»∏¿«¿Â" + "] " +
           ChatManager.Instance.chatClient.PublicChannels[channelName].Subscribers.Count + " / "
           + ChatManager.Instance.chatClient.PublicChannels[channelName].MaxSubscribers;
        }
    }
}
