using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    
    public static CameraController Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<CameraController>();
                if(obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    public Transform playerTransform;
    public GameObject playerAvatar;
    public PlayerContoller playerContoller;

    [SerializeField]
    private Transform cameraArmTransform;
    [SerializeField]
    private Transform cameraPositionTransform;
    [SerializeField]
    private Transform mainCameraTransform;
    public Transform defaultObjectTransform;

    Vector3 cameraArmPositionOffset = new Vector3(0, 0.5f, 0);
    Vector3 cameraPositionOffset = new Vector3(0, 0.5f, -3f);
    float camera_dist;

    public Vector3 TPSCameraOffset;
    public Vector3 FPSCameraOffset;

    public bool isTPS;

    public bool isChangeCameraModeDown;

    private void Awake()
    {
        if (instance == null)
            instance = GetComponent<CameraController>();
        else
            Destroy(gameObject);
        playerTransform = defaultObjectTransform;
        cameraArmTransform = GetComponent<Transform>();
        UIManager.Instance.cameraController = GetComponent<CameraController>();
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
            DontBeyondWall();
        }
        MoveCamera();
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

    public void ChangeCameraMode()
    {
        if (isTPS)
        {
            cameraPositionTransform.localPosition = FPSCameraOffset;
            playerAvatar.layer = 8;   // Player
            isTPS = false;
        }
        else if (!isTPS)
        {
            cameraPositionTransform.localPosition = TPSCameraOffset;
            playerAvatar.layer = 2;   // Ignore Raycast
            isTPS = true;
        }
    }
    void DontBeyondWall()
    {
        if (isTPS)
        {
            Vector3 rayTarget = cameraPositionTransform.position - cameraArmTransform.position;

            RaycastHit rayPoint;
            if (Physics.Raycast(cameraArmTransform.position, rayTarget, out rayPoint, camera_dist))
            {
                mainCameraTransform.position = rayPoint.point - (rayTarget * 0.2f);
            }
            else
            {
                mainCameraTransform.localPosition = Vector3.zero;
            }
        }
    }
}
