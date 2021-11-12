using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Voice;
public class VoiceController : MonoBehaviour
{
    public Recorder voiceRecorder;
    public PhotonVoiceNetwork voiceNetwork;
    public bool onVoice;
    // Start is called before the first frame update
    void Start()
    {
        voiceRecorder.TransmitEnabled = true;
        onVoice = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (onVoice)
            {
                onVoice = false;
                voiceRecorder.TransmitEnabled = false;
            }
            else
            {
                onVoice = true;
                voiceRecorder.TransmitEnabled = true;
            }
        }
    }

    [System.Obsolete]
    public void ChangeVoiceChannel(byte channelNum)
    {
        //voiceRecorder.InterestGroup = channelNum;
        voiceNetwork.Client.GlobalAudioGroup = channelNum;
    }


}