using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerWebCamUIController : MonoBehaviourPunCallbacks
{
    public GameObject webCamImage;

    public bool isWebCamDown;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
            UIManager.Instance.webCamUIController = GetComponent<PlayerWebCamUIController>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!UIManager.Instance.isOpenWindow)
            {
                GetInput();
                TurnWebCam();
            }
        }
    }

    void GetInput()
    {
        isWebCamDown = Input.GetKeyDown(KeyCode.C);
    }

    public void TurnWebCam()
    {
        if (isWebCamDown)
        {
            photonView.RPC("TurnWebCamRPC", RpcTarget.AllBuffered);
        }
    }
        [PunRPC]
        public void TurnWebCamRPC()
        {
            if (webCamImage.activeSelf)
                webCamImage.SetActive(false);
            else
                webCamImage.SetActive(true);
        }
}