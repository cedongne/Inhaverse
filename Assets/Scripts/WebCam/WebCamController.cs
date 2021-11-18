
namespace OpenCvSharp.Demo
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using OpenCvSharp.Demo;
    using UnityEngine.UI;
    using UnityEngine.Diagnostics;

    public class WebCamController : MonoBehaviour
    {
        float timer = 0f;
        public float delayTime = 10f;
        public bool isDelay = false;
        int circle_x = 0, circle_y = 0;
        public RawImage display;
        WebCamTexture camTexture;
        private int currentIndex = 0;
        String filenameFaceCascade =
            "Assets/Resources/haarcascade_frontalface_default.xml";
        CascadeClassifier faceCascade = new CascadeClassifier();

        // Start is called before the first frame update
        void Start()
        {
            if (camTexture != null)
            {
                display.texture = null;
                camTexture.Stop();
                camTexture = null;
            }
            //VideoCapture capture = new VideoCapture(0);

            WebCamDevice device = WebCamTexture.devices[currentIndex];
            camTexture = new WebCamTexture(device.name);
          //  display.texture = camTexture;
            camTexture.Play();

            if (!faceCascade.Load(filenameFaceCascade))
            {
                Debug.Log("Video Load Error");
            }
        }

        // Update is called once per frame
        void Update()
        {
            Mat image = new Mat();
            Mat dst = new Mat();
            Texture2D destTexture = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, false);
            //Texture2D tmpTexture = new Texture2D();
            image = Unity.TextureToMat(camTexture);

            //Color[] textureData = camTexture.GetPixels();
            //destTexture.SetPixels(textureData);

            //Mat grayMat = new Mat();
            //Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);
                      
            //detect Rect
            if (!isDelay)
            {
                OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(image);
                foreach (var item in faces)
                {
                    //Cv2.Rectangle(image, item, Scalar.Red); // add rectangle to the image

                    circle_x = item.Left + (item.Width / 2);
                    circle_y = item.Top + (item.Height / 2);

                    //   dst = image.SubMat(item);
                }
                isDelay = true;
            }
         //   Debug.Log(circle_x + " " + circle_y);
            if(circle_x != 0 && circle_y != 0)
            {
                Cv2.Circle(image, new Point(circle_x, circle_y), 200, Scalar.Green, 3, LineTypes.AntiAlias);
            }
            destTexture = Unity.MatToTexture(image);

            //camTexture = new WebCamTexture(destTexture.height, destTexture.width);

            //if(!dst.Empty())
            //{
            //    destTexture = Unity.MatToTexture(dst);
            //}
            display.texture = destTexture;

            if (isDelay)
            {
                timer += Time.deltaTime;
                if (timer >= delayTime)
                {
                    timer = 0f;
                    isDelay = false;
                }
            }
        }
    }
}