using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class InteractiveTable : InteractiveObject
{
    // Start is called before the first frame update
    public ChatController chatController;
    public VoiceController voiceController;

    private byte conferenceNum;

    // Update is called once per frame
    void start()
    {
    }

    public override void Interaction()
    {
        chatController.LeaveChat();
        chatController.EnterConferenceChat(this.transform.parent.name);
        SetConferenceNum(this.transform.parent.name);
        Debug.Log("channelNum: " + conferenceNum);
        voiceController.ChangeVoiceChannel(conferenceNum);
    }

    public void SetConferenceNum(string str)
    {
        string strTmp = Regex.Replace(str, @"\D", "");
        conferenceNum = byte.Parse(strTmp);
    }
}
