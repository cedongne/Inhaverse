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

    private VoiceManager() { }
    private static VoiceManager instance;

    public static VoiceManager Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<VoiceManager>();
                if(obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
            instance = GetComponent<VoiceManager>();
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        voiceNetwork.Client.GlobalAudioGroup = 255;
        voiceRecorder.TransmitEnabled = true;
        onVoice = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if (!UIManager.Instance.isOpenWindow)
            VoiceOnOff();
    }

    void GetInput()
    {
            isVoiceDown = Input.GetKeyDown(KeyCode.V);
    }

    public void VoiceOnOff()
    {
        if (isVoiceDown)
        {
            if (onVoice)
            {
                onVoice = false;
                voiceRecorder.TransmitEnabled = false;
                voiceButton.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                onVoice = true;
                voiceRecorder.TransmitEnabled = true;
                voiceButton.GetComponent<Image>().color = Color.white;
            }
        }
    }

    [System.Obsolete]
    public void ChangeVoiceChannel(byte channelNum)
    {
        //voiceRecorder.InterestGroup = channelNum;
        voiceNetwork.Client.GlobalAudioGroup = channelNum;
    }

    public void EnterLobbyChannel()
    {
        voiceNetwork.Client.GlobalAudioGroup = 255;
    }


}