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

        private int currentIndex = 0;

        void Start()
        {
            WebCamDevice device = WebCamTexture.devices[0];
            camTexture = new WebCamTexture(device.name);
            camTexture.Play();
        }

        void Update()
        {
            Mat image = new Mat();
            image = Unity.TextureToMat(camTexture);
            Texture2D destTexture = Unity.MatToTexture(image);

            display.texture = destTexture;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}