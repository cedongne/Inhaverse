using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StudentListUIManager : MonoBehaviourPunCallbacks
{
    public GameObject studentListWindow;

    private StudentListUIManager() { }

    private static StudentListUIManager instance;
    public static StudentListUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<StudentListUIManager>();
                if (obj != null)
                    instance = obj;
            }

            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<StudentListUIManager>();
            gameObject.SetActive(true);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializingText();
    }

    void InitializingText()
    {
        for (int idx = 1; idx < 60; idx++)
        {
            studentListWindow.transform.Find($"Student{idx}").Find("Text").GetComponent<Text>().text = $"Student{idx}";
        }
    }

    public void SetStudentListUI(string deskNum, string studentName)
    {
        photonView.RPC("SetStudentListUIRPC", RpcTarget.AllBuffered, deskNum, studentName);
    }

    [PunRPC]
    void SetStudentListUIRPC(string deskNum, string studentName)
    {
        studentListWindow.transform.Find($"Student{deskNum}").Find("Text").GetComponent<Text>().text = studentName;
    }

    public void GetUpFromChair()
    {
        photonView.RPC("RemoveStudent", RpcTarget.AllBuffered, PlayfabManager.Instance.playerName);
    }

    [PunRPC]
    public void RemoveStudent(string studentName)
    {
        for(int studentNum = 1; studentNum < 60; studentNum++)
        {
            if (studentListWindow.transform.Find($"Student{studentNum}").Find("Text").GetComponent<Text>().text.Equals(studentName))
            {
                studentListWindow.transform.Find($"Student{studentNum}").Find("Text").GetComponent<Text>().text = $"Student{studentNum}";
                break;
            }
        }
    }
}
