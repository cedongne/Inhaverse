using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class InteractiveTable : InteractiveObject
{
    public ChatController chatController;
    public VoiceController voiceController;

    private byte conferenceNum;


    public override void Interaction()
    {
        ChatControl();
        VoiceControl();
        UIManager.Instance.OpenWindow(Define.UI.CONFERENCE);
    }

    void ChatControl()
    {
        chatController.LeaveChat();
        chatController.EnterConferenceChat(transform.parent.name);
    }

    void VoiceControl()
    {
        SetConferenceNum(transform.parent.name);
        Debug.Log("channelNum: " + conferenceNum);
        voiceController.ChangeVoiceChannel(conferenceNum);
    }

    public void SetConferenceNum(string str)
    {
        string strTmp = Regex.Replace(str, @"\D", "");
        conferenceNum = byte.Parse(strTmp);
    }
}
