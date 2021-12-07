namespace OpenCvSharp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    using Photon.Pun;

    public class WebCamManager : MonoBehaviourPunCallbacks
    {
        private static WebCamManager instance;
        public static WebCamManager Instance
        {
            get
            {
                if(instance == null)
                {
                    var obj = FindObjectOfType<WebCamManager>();
                    if(obj != null)
                    {
                        instance = obj;
                    }
                }
                return instance;
            }
        }

        float timer = 0f;
        public float delayTime = 5f;
        bool isDelay = false;
        bool detect_flag = false;
        Rect before_image;

        WebCamTexture camTexture;
        public RawImage headDisplay;
        public RawImage conferenceDisplay;
        public RawImage nowDisplay;
        public Texture defaultTexture;


        Mat image = new Mat();
        Texture2D destTexture;

        private int currentIndex = 0;

        string filenameFaceCascade =
            "Assets/Resources/haarcascade_frontalface_default.xml";
        CascadeClassifier faceCascade = new CascadeClassifier();

        bool isWebCamDown;

        private void Awake()
        {
            if (instance == null)
                instance = GetComponent<WebCamManager>();
            else
                Destroy(gameObject);
        }
        void Start()
        {
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
                SetWebCamDisplay();
                ShowWebCam();
                //if (WebCamTexture.devices.Length != 0)
                //{
                //}
            }
            else
            {
                ShowOtherWebCam();
            }
        }

        void SetWebCamDisplay()
        {
            //if (ChatManager.Instance.currentChannelName.Contains("Conference"))
            //{
            //    detect_flag = false;
            //    nowDisplay = conferenceDisplay;
            //}
            //else
            //{
            //    detect_flag = true;
                nowDisplay = headDisplay;
            //}
        }

        void FaceDetect()
        {
            Mat dst = new Mat();
            if (!isDelay)
            {
                Rect[] faces = faceCascade.DetectMultiScale(image);
                foreach (var item in faces)
                {
                    before_image = item;
                    dst = image.SubMat(before_image);
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
                if (before_image.Top != 0 && before_image.Left != 0)
                {
                    dst = image.SubMat(before_image);
                    destTexture = Unity.MatToTexture(dst);
                }
                else
                {
                    destTexture = Unity.MatToTexture(image);
                }
            }
        }
        void ShowWebCam()
        {
            if (nowDisplay.gameObject.activeSelf)
            {
                camTexture.Play();
                image = Unity.TextureToMat(camTexture);
                destTexture = Unity.MatToTexture(image);

                //if (detect_flag)
                //{
                    FaceDetect();
                //}
                //else
                //{
                //    destTexture = Unity.MatToTexture(image);
                //}

                nowDisplay.texture = destTexture;
            }
            else
            {
                camTexture.Stop();
            }
        }

        private void OnDisable()
        {
            camTexture.Stop();
            nowDisplay.texture = defaultTexture;
        }

        void ShowOtherWebCam()
        {
            if (nowDisplay.gameObject.activeSelf)
            {
                nowDisplay.texture = destTexture;
            }
        }

        //byte[] receivedImage;
        //string receivedImageStr;
        /*
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                Debug.Log("Send : " + PlayfabManager.Instance.playerName);
                if (nowDisplay.gameObject.activeSelf)
                {
                    //                stream.SendNext(destTexture.EncodeToPNG());
                    //                Debug.Log(destTexture.EncodeToPNG().Length);
                    stream.SendNext(Convert.ToBase64String(destTexture.EncodeToPNG()));
                    Debug.Log(Convert.ToBase64String(destTexture.EncodeToPNG()));
                }
            }
            else
            {
                receivedImageStr = (string)stream.ReceiveNext();
                Debug.Log(receivedImageStr);
                destTexture.LoadImage(Convert.FromBase64String(receivedImageStr));
    //               receivedImage = (byte[])stream.ReceiveNext();
    //               destTexture.LoadRawTextureData(receivedImage);
                Debug.Log("RE");
            }
        }
        */

        private void OnDestroy()
        {
            if (photonView.IsMine)
            {
                if (camTexture != null)
                    camTexture.Stop();
            }
        }
    }
}
