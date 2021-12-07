using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Photon.Chat;

public class InteractiveConferenceTable : InteractiveObject
{
    public bool[] chairs;

    private byte conferenceNum;


    private void Start()
    {
    }

    public override void Interaction()
    {
        ChatControl();
        VoiceControl();
        UIManager.Instance.ShowUI(Define.UI.CONFERENCE);
        ChatManager.Instance.SetConferenceChatUI();

        ConferenceManager.Instance.table = gameObject;

        MineManager.Instance.playerController.OnKinematic(true); // 물리엔진 배제 ON
        MineManager.Instance.playerController.canMove = false;
        MineManager.Instance.playerController.canDetectInteractive = false;
        MineManager.Instance.playerController.canGetInput = false;
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
