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
        public RawImage display;

        Mat image = new Mat();
        Texture2D destTexture;

        private int currentIndex = 0;

        void Start()
        {
            WebCamDevice device = WebCamTexture.devices[0];
            camTexture = new WebCamTexture(device.name);
            camTexture.Play();
        }

        void Update()
        {
            if (display.gameObject.activeSelf)
            {
                image = Unity.TextureToMat(camTexture);
                destTexture = Unity.MatToTexture(image);

                display.texture = destTexture;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}