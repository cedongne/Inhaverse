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
        OpenCvSharp.Rect tmp;

        WebCamTexture camTexture;
        public RawImage headDisplay;
        public RawImage conferenceDisplay;
        public RawImage nowDisplay;

        Mat image = new Mat();
        Texture2D destTexture;

        private int currentIndex = 0;

        String filenameFaceCascade =
            "Assets/Resources/haarcascade_frontalface_default.xml";
        CascadeClassifier faceCascade = new CascadeClassifier();


        void Start()
        {
            WebCamDevice device = WebCamTexture.devices[0];
            Debug.Log(device.name);
            camTexture = new WebCamTexture(device.name);
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
                SetWebCamDisplay();
                ShowWebCam();
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
                Mat dst = new Mat();
                if (!isDelay)
                {
                    OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(image);
                    foreach (var item in faces)
                    {
                        tmp = item;
                        dst = image.SubMat(tmp);
                    }                    
                    if (dst.Empty())
                    {
                        destTexture = Unity.MatToTexture(image);
                    }
                    else
                    {
                        destTexture = Unity.MatToTexture(dst);
                    }
                    isDelay = true;
                }
                else if (isDelay)
                {
                    timer += Time.deltaTime;
                    if (timer >= delayTime)
                    {
                        timer = 0f;
                        isDelay = false;
                    }
                    if(tmp.Top != 0)
                    {
                        dst = image.SubMat(tmp);
                        destTexture = Unity.MatToTexture(dst);
                    }
                    else
                    {
                        destTexture = Unity.MatToTexture(image);
                    }
                }
                nowDisplay.texture = destTexture;
            }
            else
            {
                camTexture.Pause();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        
        }
    }
}