using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Voice;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VoiceManager : MonoBehaviour
{
    public Recorder voiceRecorder;
    public PhotonVoiceNetwork voiceNetwork;
    public bool onVoice;
    public GameObject voiceButton;

    public bool isVoiceDown;

    public GameObject playerUIObject;

    public AudioMixer speakerAudioMixer;
    public Slider speakerSlider;

    [SerializeField]
    public float volume;

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
            SetVoiceOn();
        }
        else
        {
            SetVoiceOff();
        }
    }

    public void SetVoiceOn()
    {
        onVoice = false;
        voiceRecorder.TransmitEnabled = false;
        voiceButton.GetComponent<Image>().color = Color.gray;
        playerUIObject.GetComponentInChildren<UnityEngine.UI.Outline>().effectColor = Color.white;
    }
    public void SetVoiceOff()
    {
        onVoice = true;
        voiceRecorder.TransmitEnabled = true;
        voiceButton.GetComponent<Image>().color = Color.white;
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

    public void SpeakerBtn()
    {
        if (speakerSlider.gameObject.activeSelf)
            ShowSpeakerSlider(false);
        else
            ShowSpeakerSlider(true);
    }

    public void ShowSpeakerSlider(bool set)
    {
        speakerSlider.gameObject.SetActive(set);
    }

    public void OnSliderControl()
    {
        volume = speakerSlider.value;
        speakerAudioMixer.SetFloat("SpeakerAudioMixer", volume);
        if (volume == -20)
        {
            UIManager.Instance.curSpeakerIcon.color = Color.gray;
        }
        else
        {
            UIManager.Instance.curSpeakerIcon.color = Color.white;
        }
    }

}