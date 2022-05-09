
//namespace OpenCvSharp.Demo
//{
//    using System;
//    using System.Linq;
//    using System.Text;
//    using System.Threading.Tasks;
//    using System.Collections;
//    using System.Collections.Generic;
//    using UnityEngine;
//    using OpenCvSharp;
//    using OpenCvSharp.Demo;
//    using UnityEngine.UI;
//    using UnityEngine.Diagnostics;

//    public class PlayerCamController : MonoBehaviour
//    {
//        bool key_flag = false;
//        Mat dst = new Mat();
//        float timer = 0f;
//        public float delayTime = 0.5f;
//        public bool isDelay = false;
//        //int circle_x = 0, circle_y = 0;
//        public RawImage display;
//        WebCamTexture camTexture;
//        private int currentIndex = 0;
//        String filenameFaceCascade =
//            "Assets/Resources/haarcascade_frontalface_default.xml";
//        CascadeClassifier faceCascade = new CascadeClassifier();

//        // Start is called before the first frame update
//        void Start()
//        {
            
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            Debug.Log(Input.GetKeyDown(KeyCode.C));
//            if (Input.GetKeyDown(KeyCode.C))
//            {
//                if (key_flag == false)
//                {
//                    key_flag = true;

//                    if (camTexture != null)
//                    {
//                        display.texture = null;
//                        camTexture.Stop();
//                        camTexture = null;
//                    }

//                    WebCamDevice device = WebCamTexture.devices[currentIndex];
//                    camTexture = new WebCamTexture(device.name);
                    
//                    camTexture.Play();

//                    if (!faceCascade.Load(filenameFaceCascade))
//                    {
//                        Debug.Log("Video Load Error");
//                    }

//                    Mat image = new Mat();
//                    Texture2D destTexture = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, false);
//                    image = Unity.TextureToMat(camTexture);

//                    if (!isDelay)
//                    {
//                        OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(image);
//                        foreach (var item in faces)
//                        {
//                            dst = image.SubMat(item);
//                        }
//                        isDelay = true;
//                    }
//                    destTexture = Unity.MatToTexture(dst);

//                    display.texture = destTexture;

//                    if (isDelay)
//                    {
//                        timer += Time.deltaTime;
//                        if (timer >= delayTime)
//                        {
//                            timer = 0f;
//                            isDelay = false;
//                        }
//                    }
//                }
//                else
//                {
//                    key_flag = false;

//                    camTexture.Stop();
//                }
//            }
//        }
//    }
//}