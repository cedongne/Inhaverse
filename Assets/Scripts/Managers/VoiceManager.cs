using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Voice;
using UnityEngine.UI;

public class VoiceManager : MonoBehaviour
{
    public Recorder voiceRecorder;
    public PhotonVoiceNetwork voiceNetwork;
    public bool onVoice;
    public GameObject voiceButton;

    public bool isVoiceDown;

    public GameObject playerUIObject;

    private VoiceManager() { }
    private static VoiceManager instance;

    public static VoiceManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<VoiceManager>();
                if (obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = GetComponent<VoiceManager>();
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        EnterLobbyChannel();
        onVoice = false;
        voiceRecorder.TransmitEnabled = false;
        voiceButton.GetComponent<Image>().color = Color.gray;
    }

    public void VoiceOnOff()
    {
        if (onVoice)
        {
            onVoice = false;
            voiceRecorder.TransmitEnabled = false;
            voiceButton.GetComponent<Image>().color = Color.gray;
            playerUIObject.GetComponentInChildren<UnityEngine.UI.Outline>().effectColor = Color.white;
        }
        else
        {
            onVoice = true;
            voiceRecorder.TransmitEnabled = true;
            voiceButton.GetComponent<Image>().color = Color.white;
        }
    }

    [System.Obsolete]
    public void ChangeVoiceChannel(byte channelNum)
    {
        //voiceRecorder.InterestGroup = channelNum;
        voiceNetwork.Client.GlobalAudioGroup = channelNum;
    }

    [System.Obsolete]
    public void EnterLobbyChannel()
    {
        voiceNetwork.Client.GlobalAudioGroup = 255;
    }


}