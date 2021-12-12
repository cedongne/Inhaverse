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
        Debug.Log(NetworkManager.Instance.room_name + " " + PlayfabManager.Instance.playerJob);
        if (NetworkManager.Instance.room_name.Equals("Campus"))
        {
            if (PlayfabManager.Instance.playerJob == "�л�")
            {
                UIManager.Instance.EnterClassBtn();
            }
            else if (PlayfabManager.Instance.playerJob == "����")
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
