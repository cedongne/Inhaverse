using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;

public class DistanceController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    private void Awake()
    {
        playerTransform = transform;
    }
}
