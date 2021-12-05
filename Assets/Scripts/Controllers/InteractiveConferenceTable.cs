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

    private void Start()
    {
        webBrowser = GameObject.Find("InworldBrowser").GetComponent<SimpleWebBrowser.WebBrowser>();
    }

    public override void Interaction()
    {
        ChatControl();
        VoiceControl();
        UIManager.Instance.ShowUI(Define.UI.CONFERENCE);
        ChatManager.Instance.SetConferenceChatUI();
        StartCoroutine("WebBrowserNavigate");

        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator WebBrowserNavigate()
    {
        webBrowser.OnNavigate();
        yield return new WaitForSeconds(3.0f);
        webBrowser.DialogResult(true);
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
