using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Windows.Forms;
using Ookii.Dialogs;
using Photon.Pun;
using Photon.Realtime;

public class FileManager : MonoBehaviourPun
{
    private static FileManager instance;
    public GameObject board;

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


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<FileManager>();
        }
    }

    VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();

    [SerializeField]
    private string[] filePaths;

    public void OnButtonOpenFile()
    {
        SetOpenFileDialog();
        filePaths = FileOpen(openFileDialog);
        UploadImage();
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
            = "오디오 파일 |*.mp3; *.wav" +
            "|비디오 파일 |*.mp4; *.avi" +
            "|이미지 파일 |*.jpg; *.png" +
            "|모든 파일|*.*";
        openFileDialog.FilterIndex = 1;
        openFileDialog.Multiselect = true;
    }

    void UploadImage()
    {
        photonView.RPC("UploadImageRPC", RpcTarget.All);
        UIManager.Instance.CloseWindow();
    }

    [PunRPC]
    public void UploadImageRPC()
    {
        Texture2D texture = null;
        byte[] fileData;
        fileData = File.ReadAllBytes(filePaths[0]);
        texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        board.GetComponent<MeshRenderer>().material.mainTexture = texture;
    }
}
