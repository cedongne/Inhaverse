using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class InteractiveObject : MonoBehaviour
{
    public virtual void Interaction()
    {
        Debug.Log("This is interactive object. Overrides interaction function.");
    }

    
}
