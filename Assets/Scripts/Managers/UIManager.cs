using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private UIManager() { }

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<UIManager>();
                if (obj == null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    public GameObject loginUI;
    public GameObject hudUI;
    public GameObject classUI;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<UIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowUI(Define.UI showingUi)
    {
        loginUI.SetActive(false);
        hudUI.SetActive(false);
        classUI.SetActive(false);
        if (showingUi.Equals(Define.UI.LOGIN))
        {
            loginUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.HUD))
        {
            hudUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.CLASS))
        {
            classUI.SetActive(true);
        }
    }

    public void OpenWindow(Define.UI showingUi)
    {
        classUI.SetActive(false);

        if (showingUi.Equals(Define.UI.LOGIN))
        {
            loginUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.HUD))
        {
            hudUI.SetActive(true);
        }
        else if (showingUi.Equals(Define.UI.CLASS))
        {
            classUI.SetActive(true);
        }
    }

    public void CloseWindow()
    {
        loginUI.SetActive(false);
        classUI.SetActive(false);

        hudUI.SetActive(true);
    }
}
