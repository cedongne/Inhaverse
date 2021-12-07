namespace OpenCvSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    using Photon.Pun;

    using OpenCvSharp;

    public class WebCamController : MonoBehaviourPunCallbacks, IPunObservable
    {
        float timer = 0f;
        public float delayTime = 1f;
        bool isDelay = false;
        bool detect_flag = false;
        Rect before_image;

        WebCamTexture camTexture;
        public RawImage headDisplay;
        public RawImage conferenceDisplay;
        public RawImage nowDisplay;

        Mat image = new Mat();
        Texture2D destTexture;
        Texture2D loadedTexture = new Texture2D(640, 360);

        private int currentIndex = 0;

        string filenameFaceCascade =
            "Assets/Resources/haarcascade_frontalface_default.xml";
        CascadeClassifier faceCascade = new CascadeClassifier();

        bool isWebCamDown;

        private void Awake()
        {
            MineManager.Instance.webCamController = GetComponent<WebCamController>();
        }

        void Start()
        {
            conferenceDisplay = RpcUIManager.Instance.webCamImageList[0].GetComponent<RawImage>();
            destTexture = Texture2D.blackTexture;

            if (WebCamTexture.devices.Length != 0)
            {
                WebCamDevice device = WebCamTexture.devices[0];
                camTexture = new WebCamTexture(device.name);
            }
            nowDisplay = headDisplay;
            conferenceDisplay = RpcUIManager.Instance.webCamImageList[0].GetComponent<RawImage>();

            if (!faceCascade.Load(filenameFaceCascade))
            {
                Console.WriteLine("error");
                return;
            }
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                if (WebCamTexture.devices.Length != 0)
                {
                    SetWebCamDisplay();
                    ShowWebCam();
                }
            }/*
            else
            {
                ShowOtherWebCam();
            }
            */
        }

        void SetWebCamDisplay()
        {
            if (ChatManager.Instance.currentChannelName.Contains("Conference"))
            {
                detect_flag = false;
                nowDisplay = conferenceDisplay;
            }
            else
            {
                detect_flag = true;
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

                var bytes = destTexture.EncodeToJPG();
                var str = Convert.ToBase64String(bytes);

                var abytes = Convert.FromBase64String(str);
                Texture2D Textures = new Texture2D(640, 360);
                
                Textures.LoadImage(abytes);

                nowDisplay.texture = Textures;
            }
            else
            {
                camTexture.Stop();
            }
        }

        void ShowOtherWebCam()
        {
            if (nowDisplay.gameObject.activeSelf)
            {
                nowDisplay.texture = destTexture;
            }
        }

        private void OnDestroy()
        {
            if (photonView.IsMine)
            {
                if(camTexture != null)
                    camTexture.Stop();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                var bytes = destTexture.EncodeToJPG();
                var str = Convert.ToBase64String(bytes);

                stream.SendNext(str);
            }
            else
            {
                var str = (string)stream.ReceiveNext();
                var bytes = Convert.FromBase64String(str);
                loadedTexture.LoadImage(bytes);

                nowDisplay.texture = loadedTexture;
            }
        }
    }
}