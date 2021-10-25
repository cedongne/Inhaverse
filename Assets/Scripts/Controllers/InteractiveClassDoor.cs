using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class InteractiveClassDoor : InteractiveObject
{
    void Update()
    {
        //       Debug.Log(DateTime.Now.Hour + " " + DateTime.Now.Minute);
        //    Debug.Log(DateTime.Now.DayOfWeek.);
    }

    public override void Interaction()
    {
        if (PlayfabManager.Instance.playerJob == "학생")
        {

        }
        else if (PlayfabManager.Instance.playerJob == "교수")
        {
            UIManager.Instance.OpenWindow(Define.UI.CLASS);
        }
    }

    
}
