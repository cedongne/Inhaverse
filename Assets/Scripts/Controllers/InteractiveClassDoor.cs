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

    public override void Interaction()
    {
        if (NetworkManager.Instance.room_name.Equals("Lobby"))
        {
            if (PlayfabManager.Instance.playerJob == "학생")
            {
                UIManager.Instance.EnterClassBtn();
            }
            else if (PlayfabManager.Instance.playerJob == "교수")
            {
                UIManager.Instance.OpenWindow(Define.UI.CLASS);
            }
        }
        else
        {
            NetworkManager.Instance.JoinToCampus();
        }
    }

    
}
