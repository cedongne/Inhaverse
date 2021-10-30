using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;

    [SerializeField]
    private Transform cameraArmTransform;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform defaultObjectTransform;

    Vector3 cameraArmPositionOffset = new Vector3(0, 1, 0);

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
        cameraTransform.localPosition = TPSCameraOffset;


        isTPS = true;
    }

    // Update is called once per frame
    void Update()
    {
        FPSLookAround();
        MoveCamera();
        ChangeCameraMode();
        Penetrate();
    }

    void FPSLookAround()
    {
        if (UIManager.Instance.isOpenWindow)
            return;
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * 2, Input.GetAxis("Mouse Y") * 2);
        Vector3 camAngle = cameraArmTransform.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 80f);
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
                cameraTransform.localPosition = FPSCameraOffset;
                isTPS = false;
            }
            else if (!isTPS)
            {
                cameraTransform.localPosition = TPSCameraOffset;
                isTPS = true;
            }
        }
    }

    void Penetrate()
    {
        Renderer ObstacleRenderer;

        float Distance = Vector3.Distance(cameraTransform.position, playerTransform.position);
        Vector3 Direction = (playerTransform.position - cameraTransform.position).normalized;

        RaycastHit hit;
        Debug.DrawRay(cameraTransform.position, Direction * Distance, Color.red);
        if (Physics.Raycast(cameraTransform.position, Direction, out hit, Distance))
        {
            // 2.맞았으면 Renderer를 얻어온다.
            ObstacleRenderer = hit.transform.GetComponentInChildren<MeshRenderer>();
            if (ObstacleRenderer != null)
            {
                Debug.Log(ObstacleRenderer.material.color);
                // 3. Metrial의 Aplha를 바꾼다.
                Material Mat = ObstacleRenderer.material;
                Color matColor = Mat.color;
                matColor.a = 0.2f;
                Mat.color = matColor;
            }
        }
    }
}
