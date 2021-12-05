using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraController : MonoBehaviour
{
    public GameObject targetObject;
    public Vector3 CameraPos;
    // Start is called before the first frame update
    void Start()
    {

        transform.position = targetObject.transform.position + CameraPos;
        transform.LookAt(targetObject.transform);
    }
}
