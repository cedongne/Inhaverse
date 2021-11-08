using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;

    [SerializeField]
    private Transform cameraArmTransform;
    [SerializeField]
    private Transform cameraPositionTransform;
    [SerializeField]
    private Transform mainCameraTransform;
    [SerializeField]
    private Transform defaultObjectTransform;

    Vector3 cameraArmPositionOffset = new Vector3(0, 1, 0);
    Vector3 cameraPositionOffset = new Vector3(0, 0.5f, -3f);
    float camera_dist;

    public Vector3 TPSCameraOffset;
    public Vector3 FPSCameraOffset;

    bool isTPS;

    private void Awake()
    {
        cameraArmTransform = GetComponent<Transform>();
        playerTransform = defaultObjectTransform;
    }

    private void Start()
    {
        cameraPositionTransform.localPosition = TPSCameraOffset;
        mainCameraTransform.localPosition = Vector3.zero;

        camera_dist = Mathf.Sqrt(cameraPositionOffset.y * cameraPositionOffset.y + cameraPositionOffset.z * cameraPositionOffset.z);

        isTPS = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIManager.Instance.isOpenWindow)
        {
            FPSLookAround();
            MoveCamera();
            ChangeCameraMode();
            DontBeyondWall();
        }
    }

    private void FixedUpdate()
    {
    }

    void FPSLookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * 2, Input.GetAxis("Mouse Y") * 2);
        Vector3 camAngle = cameraArmTransform.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        { 
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArmTransform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    void MoveCamera()
    {
        cameraArmTransform.position = playerTransform.position + cameraArmPositionOffset;
    }

    void ChangeCameraMode()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isTPS)
            {
                cameraPositionTransform.localPosition = FPSCameraOffset;
                isTPS = false;
            }
            else if (!isTPS)
            {
                cameraPositionTransform.localPosition = TPSCameraOffset;
                isTPS = true;
            }
        }
    }

    void DontBeyondWall()
    {
        Vector3 rayTarget = (cameraArmTransform.position - cameraPositionTransform.position).normalized;

        RaycastHit[] rayPoint;
        Debug.DrawRay(cameraPositionTransform.position, rayTarget, Color.red);
        rayPoint = Physics.RaycastAll(cameraPositionTransform.position, rayTarget, camera_dist);

        Debug.Log(rayPoint.Length);
        if (rayPoint.Length != 0)
        {
            Debug.Log("Point");
            //           cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraArmTransform.position, Time.deltaTime * 10);
            mainCameraTransform.position = rayPoint[rayPoint.Length - 1].point;
        }
        else
        {
            Debug.Log("Not");
            //           cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, TPSCameraOffset, Time.deltaTime * 5f);
            mainCameraTransform.localPosition = Vector3.zero;
        }
        

    }
}
