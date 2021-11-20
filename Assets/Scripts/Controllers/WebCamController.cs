namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    using Photon.Pun;

    using OpenCvSharp;

    public class WebCamController : MonoBehaviourPunCallbacks, IPunObservable
    {
        WebCamTexture camTexture;
        public RawImage headDisplay;
        public RawImage conferenceDisplay;
        public RawImage nowDisplay;

        Mat image = new Mat();
        Texture2D destTexture;

        private int currentIndex = 0;

        void Start()
        {
            WebCamDevice device = WebCamTexture.devices[0];
            Debug.Log(device.name);
            camTexture = new WebCamTexture(device.name);
            camTexture.Play();
            nowDisplay = headDisplay;
            conferenceDisplay = RpcUIManager.Instance.webCamImageList[0].GetComponent<RawImage>();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                SetWebCamDisplay();
            }
            ShowWebCam();
        }

        void SetWebCamDisplay()
        {
            if (ChatManager.Instance.currentChannelName.Contains("Conference"))
            {
                nowDisplay = conferenceDisplay;
            }
            else
            {
                nowDisplay = headDisplay;
            }

        }

        void ShowWebCam()
        {
            image = Unity.TextureToMat(camTexture);
            destTexture = Unity.MatToTexture(image);

            nowDisplay.texture = destTexture;

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        
        }
    }
}