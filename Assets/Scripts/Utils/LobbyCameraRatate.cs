using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCameraRatate : MonoBehaviour
{
    Transform cameraArmTransform;

    [SerializeField]
    private Vector3 rotateSpeed = new Vector3(0, 0.2f, 0);

    private void Awake()
    {
        cameraArmTransform = transform;
    }

    private void Update()
    {
        cameraArmTransform.Rotate(rotateSpeed);
    }
}