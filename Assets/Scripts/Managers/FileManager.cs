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
        openFileDialog.Title = "���� ����";
        openFileDialog.Filter
            = "�̹��� ���� |*.jpg; *.png" +
            "|����� ���� |*.mp3; *.wav" +
            "|���� ���� |*.mp4; *.avi" +
            "|��� ����|*.*";
        openFileDialog.FilterIndex = 1;
        openFileDialog.Multiselect = true;
    }

    public void SaveFile()
    {
        VistaSaveFileDialog saveFileDialog = new VistaSaveFileDialog()
        {
            FileName = "Inhaverse" + DateTime.Now.ToString("yyyyMMdd") + ".png",
            Filter = "png|",
            AddExtension = true,
            OverwritePrompt = true,
            DefaultExt = ".png"
        };

        string fileName;
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            fileName = saveFileDialog.FileName;
            File.WriteAllBytes(fileName, ScreenShotManager.Instance.pngData);
        }
        

    }

    public void UpdateImage()
    {
        photonView.RPC("UpdateImageRPC", RpcTarget.All);
    }

    public void DeleteImage()
    {
        for (int idx = 0; idx < boardList.Count; idx++)
        {
            if (boardList[idx].Equals(board))
            {
                photonView.RPC("DeleteImageRPC", RpcTarget.AllBuffered, idx);
                break;
            }
        }

    }
    
    [PunRPC]
    public void DeleteImageRPC(int idx)
    {
        boardList[idx].GetComponent<MeshRenderer>().material.mainTexture = null;
        boardList[idx].GetComponent<InteractiveTentBoard>().DisconnectHost();

        boardList[idx].transform.parent.parent.GetComponentInChildren<InteractiveTent>().SetTriggerOnOff();
    }

    public IEnumerator UrlUpload(string url, string hostName)
    {
        WWW www = new WWW(url);
        yield return www;
        if (www.texture != null)
        {
            board.GetComponent<InteractiveTentBoard>().SetNewHost(hostName);
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
                photonView.RPC("GetBoardRPC", RpcTarget.AllBuffered, idx, input.text, PlayfabManager.Instance.playerName);
                break;
            }
        }
    }
    [PunRPC]
    private void GetBoardRPC(int idx, string url, string hostName)
    {
        
        Debug.Log(boardList[idx]);
        Debug.Log(url);
        board = boardList[idx];
        StartCoroutine(UrlUpload(url, hostName));
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
