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
            camTexture = new WebCamTexture(device.name);
            nowDisplay = headDisplay;
            destTexture = Texture2D.blackTexture;
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
                //if (!isDelay)
                //{
                Mat dst = new Mat();
                OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(image);
                foreach (var item in faces)
                {
                    Debug.Log("test");
                    //int circle_x = item.Left + (item.Width / 2);
                    //int circle_y = item.Top + (item.Height / 2);
                    dst = image.SubMat(item);
                    //Cv2.Circle(image, new Point(circle_x, circle_y), 250, Scalar.Green, 3, LineTypes.AntiAlias);
                }
                if (dst.Empty())
                {
                    destTexture = Unity.MatToTexture(image);
                }
                else
                {
                    destTexture = Unity.MatToTexture(dst);
                }

                nowDisplay.texture = destTexture;
                    isDelay = true;
                //}

                //if (isDelay)
                //{
                //    if (timer >= delayTime)
                //    {
                //        timer = 0f;
                //        isDelay = false;
                //    }
                //}
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
                stream.SendNext(destTexture.ToString());
            }
            else
            {
                Debug.Log((string)stream.ReceiveNext());
                destTexture.LoadImage(Convert.FromBase64String((string)stream.ReceiveNext()));
                Debug.Log("RE");
            }
        }
    }
}