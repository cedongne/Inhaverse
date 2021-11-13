using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerWebCamUIController : MonoBehaviourPunCallbacks
{
    public GameObject webCamImage;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            UIManager.Instance.webcamImage = webCamImage;
        }
    }
}
