using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Photon.Chat;

public class InteractiveConferenceTable : InteractiveObject
{
    public bool[] chairs;

    private byte conferenceNum;

    public override void Interaction()
    {
        ChatControl();
        VoiceControl();
        UIManager.Instance.ShowUI(Define.UI.CONFERENCE);
        ChatManager.Instance.SetConferenceChatUI();

        Cursor.lockState = CursorLockMode.None;
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
}
