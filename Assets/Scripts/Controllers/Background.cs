using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class Background : MonoBehaviour
{
    private Background() { }
    private static Background instance;


    public static Background Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<Background>();
                if (obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject.GetComponent<Background>();
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public Material dayBox;
    public Material nightBox;

    void Update()
    {
        ChangeDays();
        if (!UIManager.Instance.isOpenWindow && !ChatManager.Instance.onChat)
        {
            ChageDaysWithButton();
        }
    }

    public void ChangeDays()
    {
        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 18 && RenderSettings.skybox != dayBox)
        {
            RenderSettings.skybox = dayBox;
            RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f);
        }
        else if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour < 6 && RenderSettings.skybox != nightBox)
        {
            RenderSettings.skybox = nightBox;
            RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);
        }

    }

    public void ChageDaysWithButton()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (RenderSettings.skybox != dayBox)
            {
                RenderSettings.skybox = dayBox;
                RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                RenderSettings.skybox = nightBox;
                RenderSettings.ambientLight = new Color(0.15f, 0.15f, 0.15f);
            }
        }
    }
}
