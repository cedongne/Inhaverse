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
    public RawImage display;
    WebCamTexture camTexture;
    private int currentIndex = 0;
    String filenameFaceCascade =
        "Assets/OpenCV+Unity/Resources/haarcascade_frontalface_default.xml";
    CascadeClassifier faceCascade = new CascadeClassifier();

    //if(!faceCascade.Load(filenameFaceCascade))
    //{
    //    Debug.Log("Video Load Error");
    //}
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
        display.texture = camTexture;
        camTexture.Play();

        Mat image = new Mat();
        while (true)
        {
            //Texture2D destTexture = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, false);
            //Color[] textureData = camTexture.GetPixels();
            //destTexture.SetPixels(textureData);

            //Mat mat = Unity.TextureToMat(this.texture);
            //Mat grayMat = new Mat();
            //Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

            //     image = new Mat(camTexture.height, camTexture.width, CvType.CV_8UC4); 
            if (image.Empty())
                break;

            //detect Rect
            //            OpenCvSharp.Rect[] faces = faceCascade.DetectMultiScale(image);
            //            foreach (var item in faces)
            //            {
            //                //Cv2.Rectangle(image, item, Scalar.Red); // add rectangle to the image
            //                int circle_x = item.Left + (item.Width / 2);
            //                int circle_y = item.Top + (item.Height / 2);
            //                Cv2.Circle(image, new Point(circle_x, circle_y), 140, Scalar.Green, 3, LineTypes.AntiAlias);

            ////                Console.WriteLine("faces : " + item);
            //            }

            //display
            //         window.ShowImage(image);

            //Cv2.WaitKey(sleepTime);
            //if (Cv2.WaitKey(33) == 'q')
            //{
            //    break;
            //}
        }
    }

    // Update is called once per frame
    void Update()
    {
        //        Utils.webCamTextureToMat(camTexture, rgbaMat, colors);   
    }
}
