using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Windows.Forms;
using Ookii.Dialogs;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class FileManager : MonoBehaviourPunCallbacks
{
    public InputField inputField;
    public GameObject entrance;
    private FileManager() { }
    private static FileManager instance;


    public static FileManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<FileManager>();
                if (obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    public GameObject board;
    public List<GameObject> boardList;
    public InputField input;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<FileManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();

    [SerializeField]
    private string[] filePaths;

    public void OnButtonDisconnectHost()
    {
        board.GetComponent<InteractiveTentBoard>().DisconnectHost();
        DeleteImage();

    }

    public void InputUrl()
    {
        GetBoard();
        Invoke("CloseWindowInvoke", 0.1f);
    }

    void CloseWindowInvoke()
    {
        inputField.text = "";
        UIManager.Instance.CloseWindow();
    }

    string[] FileOpen(VistaOpenFileDialog openFileDialog)
    {
        var result = openFileDialog.ShowDialog();
        var filenames = result == DialogResult.OK ?
            openFileDialog.FileNames :
            new string[0];
        openFileDialog.Dispose();
        return filenames;
    }

    void SetOpenFileDialog()
    {
        openFileDialog.Title = "파일 열기";
        openFileDialog.Filter
            = "이미지 파일 |*.jpg; *.png" +
            "|오디오 파일 |*.mp3; *.wav" +
            "|비디오 파일 |*.mp4; *.avi" +
            "|모든 파일|*.*";
        openFileDialog.FilterIndex = 1;
        openFileDialog.Multiselect = true;
    }

    public void UpdateImage()
    {
        photonView.RPC("UpdateImageRPC", RpcTarget.All);
    }

    public void DeleteImage()
    {
        board.GetComponent<MeshRenderer>().material.mainTexture = null;
    }

    public IEnumerator UrlUpload(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.texture != null)
        {
            board.GetComponent<InteractiveTentBoard>().SetNewHost();
            board.GetComponent<MeshRenderer>().material.mainTexture = RotateImage(www.texture, -90);
            board.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(3, 4);
            board.GetComponent<InteractiveTentBoard>().imageExisted = true;
        }
        board.transform.parent.parent.GetComponentInChildren<InteractiveTent>().SetTriggerOnOff();
    }
    public void UploadFileOnPlayFab()
    {

    }

    [PunRPC]
    private void UpdateImageRPC()
    {
    }
    public void GetBoard()
    {
        for(int idx = 0; idx < boardList.Count; idx++)
        {
            if(boardList[idx].Equals(board))
            {
                photonView.RPC("GetBoardRPC", RpcTarget.AllBuffered, idx, input.text);
                break;
            }
        }
    }
    [PunRPC]
    private void GetBoardRPC(int idx, string url)
    {
        
        Debug.Log(boardList[idx]);
        Debug.Log(url);
        board = boardList[idx];
        StartCoroutine("UrlUpload", url);
    }

    public Texture2D RotateImage(Texture2D originTexture, int angle)
    {
        Texture2D result;
        result = new Texture2D(originTexture.width, originTexture.height);
        Color32[] pix1 = result.GetPixels32();
        Color32[] pix2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int x = 0;
        int y = 0;
        Color32[] pix3 = rotateSquare(pix2, (Math.PI / 180 * (double)angle), originTexture);
        for (int j = 0; j < H; j++)
        {
            for (var i = 0; i < W; i++)
            {
                //pix1[result.width/2 - originTexture.width/2 + x + i + result.width*(result.height/2-originTexture.height/2+j+y)] = pix2[i + j*originTexture.width];
                pix1[result.width / 2 - W / 2 + x + i + result.width * (result.height / 2 - H / 2 + j + y)] = pix3[i + j * W];
            }
        }
        result.SetPixels32(pix1);
        result.Apply();
        return result;
    }
    static Color32[] rotateSquare(Color32[] arr, double phi, Texture2D originTexture)
    {
        int x;
        int y;
        int i;
        int j;
        double sn = Math.Sin(phi);
        double cs = Math.Cos(phi);
        Color32[] arr2 = originTexture.GetPixels32();
        int W = originTexture.width;
        int H = originTexture.height;
        int xc = W / 2;
        int yc = H / 2;
        for (j = 0; j < H; j++)
        {
            for (i = 0; i < W; i++)
            {
                arr2[j * W + i] = new Color32(0, 0, 0, 0);
                x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                if ((x > -1) && (x < W) && (y > -1) && (y < H))
                {
                    arr2[j * W + i] = arr[y * W + x];
                }
            }
        }
        return arr2;
    }
}
