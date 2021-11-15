using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class PlayerContoller : MonoBehaviourPun
{
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private GameObject cameraArm;
    [SerializeField]
    private Transform cameraArmTransform;
    [SerializeField]
    private Rigidbody playerRigid;
    [SerializeField]
    private GameObject interactionUI;

    private Vector3 screenCenter;
    private InputField inputField;

    Animator animator;

    public float walkMoveSpeed;
    public float runMoveSpeed;
    float moveSpeed;

    public float prevYPosition;
    public float jumpPower;

    private ChatController chatController;
    private VoiceController voiceController;

    bool isRun;
    bool isJump;
    bool isDown;

    public bool isJumpDown;
    public bool isRunDown;
    public bool isCamDown;

    public Outline currentTouch;

    Vector3 InteractionUIOffsetForMouse = new Vector3(120, -50);
    Vector3 InteractionUIOffsetForLook = new Vector3(170, -75.5f);

    private void Awake()
    {
        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        animator = GetComponent<Animator>();
        cameraArm = GameObject.Find("Camera Arm");
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (photonView.IsMine)
        {
            cameraArmTransform = cameraArm.transform;

            cameraArm.GetComponent<CameraController>().enabled = true;
            cameraArm.GetComponent<LobbyCameraRatate>().enabled = false;

            cameraArmTransform.rotation = new Quaternion(0, 0, 0, 0);
            cameraArm.GetComponent<CameraController>().playerTransform = transform;
            cameraArm.GetComponent<CameraController>().playerAvatar = GameObject.Find("Avatar");
            cameraArm.GetComponent<CameraController>().playerContoller = GetComponent<PlayerContoller>();
            UIManager.Instance.playerController = GetComponent<PlayerContoller>();
        }
        moveSpeed = walkMoveSpeed;

        interactionUI = GameObject.Find("Canvas").transform.Find("Interaction UI").gameObject;
        currentTouch = GameObject.Find("Initializing Object").GetComponent<Outline>();
        chatController = GameObject.Find("ChatController").GetComponent<ChatController>();
        voiceController = GameObject.Find("VoiceController").GetComponent<VoiceController>();

        DontDestroyOnLoad(cameraArm);
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("ChatController"));
        DontDestroyOnLoad(GameObject.Find("Initializing Object"));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!UIManager.Instance.isOpenWindow)
        {
            if (!chatController.onChat)
            {
                Move();
                JumpDown();
            }
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!UIManager.Instance.isOpenWindow)
        {
            if (!chatController.onChat)
            {
                GetInput();
                Jump();
                WalkToRun();

                DetectInteractiveObject();
                OnCursorVisible();
            }
        }
    }

    void GetInput()
    {
        isRunDown = Input.GetKeyDown(KeyCode.R);
        isJumpDown = Input.GetKeyDown(KeyCode.Space);
        isCamDown = Input.GetKeyDown(KeyCode.C);
    }

    public void WalkToRun()
    {
        if (isRunDown)
        {
            if (isRun)
            {
                isRun = false;
                moveSpeed = walkMoveSpeed;
            }
            else
            {
                isRun = true;
                moveSpeed = runMoveSpeed;
            }
            animator.SetBool("isRun", isRun);
        }
    }

    void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isMove", isMove);

        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArmTransform.forward.x, 0f, cameraArmTransform.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArmTransform.right.x, 0f, cameraArmTransform.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            playerTransform.position += moveDir * Time.deltaTime * moveSpeed;
            playerTransform.forward = moveDir;
        }
    }

    void Jump()
    {
        if (isJumpDown && !isJump)
        {
            animator.SetTrigger("DoJump");
            playerRigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", isJump);
            Invoke("StartJumpDown", 0.1f);
        }
    }

    void StartJumpDown()
    {
        isDown = true;
    }

    void JumpDown()
    {
        if (isDown)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerTransform.position, Vector3.down, out hit, 0.1f))
            {
                isJump = false;
                isDown = false;
                animator.SetBool("isJump", isJump);
                animator.SetBool("isDown", isDown);
            }
        }
    }

    void DetectInteractiveObject()
    {
        RaycastHit hit;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(screenCenter), out hit, 5);
            interactionUI.transform.localPosition = InteractionUIOffsetForLook;
        }
        else
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 5);
            interactionUI.transform.position = Input.mousePosition + InteractionUIOffsetForMouse;
        }

        if (hit.collider != null && hit.transform.gameObject.CompareTag("Interactive Object"))
        {
            currentTouch = hit.transform.gameObject.GetComponent<Outline>();
            currentTouch.enabled = true;
            interactionUI.SetActive(true);

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.gameObject.GetComponent<InteractiveObject>().Interaction();
                currentTouch.enabled = false;
                interactionUI.SetActive(false);
            }
        }
        else
        {
            currentTouch.enabled = false;
            interactionUI.SetActive(false);
        }
    }

    void OnCursorVisible()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.None;
            CancelInvoke("OnCursorUnvisible");
            if(!UIManager.Instance.isOpenWindow)
                Invoke("OnCursorUnvisible", 3);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            OnCursorUnvisible();
        }
    }

    void OnCursorUnvisible()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
