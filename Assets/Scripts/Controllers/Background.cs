using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public Material dayBox;
    public Material nightBox;
    private bool isNight = false;

    void Update()
    {
        //StartCoroutine(ChangeDays());
    }

    IEnumerator ChangeDays()
    {
        yield return new WaitForSeconds(5.0f);
        if (isNight)
        {
            RenderSettings.skybox = dayBox;
            isNight = false;
        }
        else
        {
            isNight = true;
            RenderSettings.skybox = nightBox;
        }

    }
}
