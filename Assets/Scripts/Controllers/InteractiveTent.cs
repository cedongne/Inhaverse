using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class InteractiveTent : InteractiveObject
{
    private byte conferenceNum;
    public bool isEntered;
    public GameObject board;
    public BoxCollider collider;

    public void SetTriggerOnOff()
    {
        if(board.GetComponent<InteractiveTentBoard>().hostID == "")
        {
            collider.isTrigger = false;
        }
        else
        {
            collider.isTrigger = true;
        }
    }

    void ChatControl()
    {
        ChatManager.Instance.LeaveChat();
        ChatManager.Instance.EnterConferenceChat(transform.parent.name);
    }

    void VoiceControl()
    {
        SetConferenceNum(transform.parent.name);
        VoiceManager.Instance.ChangeVoiceChannel(conferenceNum);
    }

    public void SetConferenceNum(string str)
    {
        string strTmp = Regex.Replace(str, @"\D", "");
        conferenceNum = byte.Parse(strTmp);
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if(!isEntered)
            {
                ChatControl();
                VoiceControl();
                isEntered = true;
            }
            else
            {
                ChatManager.Instance.LeaveChat();
                ChatManager.Instance.EnterLobbyChat();
                isEntered = false;
            }
        }
    }
}
