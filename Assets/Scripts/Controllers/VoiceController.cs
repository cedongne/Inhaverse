using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class VoiceController : MonoBehaviour
{
    public Recorder voiceRecorder;
    public bool onVoice;
    // Start is called before the first frame update
    void Start()
    {
        voiceRecorder.TransmitEnabled = false;
        onVoice = false;
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
}
