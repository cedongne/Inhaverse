using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Windows.Forms;
using Ookii.Dialogs;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class FileManager : MonoBehaviourPun
{
    private static FileManager instance;
    public GameObject board;
    public InputField inputfield;

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

    public void InputUrl()
    {
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
        openFileDialog.Title = "���� ����";
        openFileDialog.Filter
            = "�̹��� ���� |*.jpg; *.png" +
            "|����� ���� |*.mp3; *.wav" +
            "|���� ���� |*.mp4; *.avi" +
            "|��� ����|*.*";
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
        StartCoroutine("UrlUpload");
    }

    public IEnumerator UrlUpload()
    {
        WWW www = new WWW(inputfield.text);
        www.texture.Resize(1, 1);
        yield return www;
        board.GetComponent<MeshRenderer>().material.mainTexture = www.texture;
    }
}
