using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class SetPlayerName : MonoBehaviourPunCallbacks
{
    // Update is called once per frame
    void Update()
    {
        if (!photonView.Owner.NickName.Equals(""))
        {
            Debug.Log("Name is Loaded. " + photonView.Owner.NickName);
            name = photonView.Owner.NickName;
            enabled = false;
        }
    }
}
