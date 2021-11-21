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
            camTexture = new WebCamTexture(device.name);
            nowDisplay = headDisplay;
            conferenceDisplay = RpcUIManager.Instance.webCamImageList[0].GetComponent<RawImage>();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                SetWebCamDisplay();
                ShowWebCam();
            }
            else
            {
                ShowOtherWebCam();
            }
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
            if (nowDisplay.gameObject.activeSelf)
            {
                camTexture.Play();
                image = Unity.TextureToMat(camTexture);
                destTexture = Unity.MatToTexture(image);

                nowDisplay.texture = destTexture;
            }
            else
            {
                camTexture.Pause();
            }
        }

        void ShowOtherWebCam()
        {
            if (nowDisplay.gameObject.activeSelf)
            {
                nowDisplay.texture = destTexture;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(destTexture);
            }
            else
            {
                destTexture = (Texture2D)stream.ReceiveNext();
            }
        }
    }
}