using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Photon.Chat;

public class InteractiveConferenceTable : InteractiveObject
{
    public bool[] chairs;

    private byte conferenceNum;

    public SimpleWebBrowser.WebBrowser webBrowser;
    public Camera mainCamera;
    public Camera UICamera;

    private void Start()
    {
    }

    public override void Interaction()
    {
        ChatControl();
        VoiceControl();
        UIManager.Instance.ShowUI(Define.UI.CONFERENCE);
        ChatManager.Instance.SetConferenceChatUI();

        Cursor.lockState = CursorLockMode.None;
<<<<<<< HEAD
=======

        mainCamera.enabled = false;
        UICamera.enabled = true;
        Application.OpenURL("https://owake.me/");
//        webBrowser.OnNavigate();
        ConferenceManager.Instance.table = gameObject;
>>>>>>> 1b99e1e0aa369fb276e5e169779ba6c8148c8b1d
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
