//namespace OpenCvSharp.Demo
//{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenCvSharp;
    using OpenCvSharp.Demo;
    using UnityEngine.UI;
    using UnityEngine.Diagnostics;
    using Photon.Pun;
    using Photon.Realtime;

public class PlayerWebCamUIController : MonoBehaviourPunCallbacks
{
    public GameObject webCamImage;

    public bool isWebCamDown;

    bool key_flag = false;
    Mat dst = new Mat();
    float timer = 0f;
    public float delayTime = 0.5f;
    public bool isDelay = false;
    //int circle_x = 0, circle_y = 0;
    public RawImage display;
    WebCamTexture camTexture;
    private int currentIndex = 0;
    String filenameFaceCascade =
        "Assets/Resources/haarcascade_frontalface_default.xml";
    CascadeClassifier faceCascade = new CascadeClassifier();

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
            GetInput();
            TurnWebCam();
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

            if (key_flag == false)
            {
                key_flag = true;

                if (camTexture != null)
                {
                    display.texture = null;
                    camTexture.Stop();
                    camTexture = null;
                }
                WebCamDevice device = WebCamTexture.devices[currentIndex];
                camTexture = new WebCamTexture(device.name);
                display.texture = camTexture;
                camTexture.Play();

                //        if (!faceCascade.Load(filenameFaceCascade))
                //        {
                //            Debug.Log("Video Load Error");
                //        }

                //        Mat image = new Mat();
                //        Texture2D destTexture = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, false);
                //        image = Unity.TextureToMat(camTexture);

                //        if (!isDelay)
                //        {
                //            OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(image);
                //            foreach (var item in faces)
                //            {
                //                dst = image.SubMat(item);
                //            }
                //            isDelay = true;
                //        }
                //        destTexture = Unity.MatToTexture(dst);

                //        display.texture = destTexture;

                //        if (isDelay)
                //        {
                //            timer += Time.deltaTime;
                //            if (timer >= delayTime)
                //            {
                //                timer = 0f;
                //                isDelay = false;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        key_flag = false;

                //        camTexture.Stop();
                //    }
                //}
            }
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

//}