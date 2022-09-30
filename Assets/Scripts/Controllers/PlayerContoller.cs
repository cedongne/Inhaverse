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
    public GameObject cameraArm;
    [SerializeField]
    private Transform cameraArmTransform;
    [SerializeField]
    private CameraController cameraController;
    [SerializeField]
    private Rigidbody playerRigid;
    [SerializeField]
    private GameObject interactionUI;
    [SerializeField]
    private GameObject playerUIObjects;
    [SerializeField]
    private GameObject webCamImage;
    [SerializeField]
    private PlayerUIController playerUIController;
    [SerializeField]
    private OpenCvSharp.WebCamController webCamController;

    private Vector3 screenCenter;
    private InputField inputField;

    Animator animator;

    public float walkMoveSpeed;
    public float runMoveSpeed;
    float moveSpeed;

    public float prevYPosition;
    public float jumpPower;

    bool isRun;
    bool isJump;
    bool isDown;
    public bool isSitted;

    public bool canMove;
    public bool canDetectInteractive;
    public bool canGetInput;

    public bool isJumpDown;
    public bool isRunDown;
    public bool isDanceDown;
    public bool isInfoWindowDown;
    public bool isChangeCameraModeDown;
    public Outline currentTouch;

    public List<string> playerList;

    Vector3 InteractionUIOffsetForMouse = new Vector3(120, -50);
    Vector3 InteractionUIOffsetForLook = new Vector3(170, -75.5f);

    private void Awake()
    {
        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        animator = GetComponent<Animator>();
        cameraArm = GameObject.Find("Camera Arm");

        canMove = true;
        canDetectInteractive = true;
        canGetInput = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        name = photonView.Owner.NickName;
        if (!photonView.IsMine)
        {
            RpcUIManager.Instance.playerList.Add(gameObject.transform);
            RpcUIManager.Instance.playerUILIst.Add(playerUIObjects);
            RpcUIManager.Instance.webCamImageList.Add(webCamImage);
        }

        Cursor.lockState = CursorLockMode.Locked;

        if (photonView.IsMine)
        {
            MineManager.Instance.player = gameObject;
            MineManager.Instance.playerController = GetComponent<PlayerContoller>();
            MineManager.Instance.playerUI = playerUIObjects;

            cameraController = cameraArm.GetComponent<CameraController>();
            cameraArmTransform = cameraArm.transform;
            cameraController.enabled = true;
            cameraArm.GetComponent<LobbyCameraRatate>().enabled = false;

            cameraArmTransform.rotation = Quaternion.identity;
            cameraController.playerTransform = transform;
            cameraController.playerAvatar = GameObject.Find("Avatar");
            cameraController.playerContoller = GetComponent<PlayerContoller>();
            if (!cameraController.isTPS)
            {
                cameraController.isTPS = true;
                isChangeCameraModeDown = true;
                ChangeCameraMode();
            }

            VoiceManager.Instance.playerUIObject = playerUIObjects;
        }
        moveSpeed = walkMoveSpeed;

        interactionUI = GameObject.Find("Canvas").transform.Find("Interaction UI").gameObject;

        DontDestroyOnLoad(cameraArm);
        DontDestroyOnLoad(GameObject.Find("Canvas"));
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
            if (!ChatManager.Instance.onChat)
            {
                Move();
            }
        }
        JumpDown();
        animator.SetFloat("yVelocity", playerRigid.velocity.y);
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (!UIManager.Instance.isOpenWindow)
        {
            if (!ChatManager.Instance.onChat)
            {
                GetInput();
                CheckClickedInputField();
                TeleportWayPoint();
                DetectInteractiveObject();
            }
        }
        Jump();
        WalkToRun();
        ChangeCameraMode();
        OpenInfoWindow();

        OnCursorVisible();
        ShowPlayerUIAsDistance();

        VoiceOnOff();
        OpenOptionWindow();
        SittingChair();

        Dance();
    }

    void GetInput()
    {
        if (!canGetInput)
            return;
        isRunDown = Input.GetKeyDown(KeyCode.R);
        isJumpDown = Input.GetKeyDown(KeyCode.Space);
        isChangeCameraModeDown = Input.GetKeyDown(KeyCode.Tab);
        isDanceDown = Input.GetKeyDown(KeyCode.F1);
    }
    
    void CheckClickedInputField()
    {
        if (EventSystem.current.currentSelectedGameObject != null &&
            EventSystem.current.currentSelectedGameObject.GetComponent<InputField>())
        {
            Debug.Log("Input");
            canGetInput = false;
        }
        else
        {
            canGetInput = true;
        }
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

    public void ChangeCameraMode()
    {
        if (isChangeCameraModeDown)
        {
            if (cameraController.isTPS)
                playerUIObjects.SetActive(false);
            else
                playerUIObjects.SetActive(true);
            cameraController.ChangeCameraMode();
        }
    }

    void Move()
    {
        if (!canMove)
            return;

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
        if (!canDetectInteractive)
            return;

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
            if (!currentTouch.Equals(hit.transform.gameObject.GetComponent<Outline>()))
            {
                currentTouch.enabled = false;
                interactionUI.SetActive(false);
            }
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

    void OpenInfoWindow()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!UIManager.Instance.playerInfoWindow.activeSelf)
                UIManager.Instance.InfoBtn();
            else
                UIManager.Instance.CloseWindow();
            foreach (var name in ChatManager.Instance.chatClient.PublicChannels.Keys)
            {
                Debug.Log(name);
            }
        }
    }

    void SittingChair()
    {
        animator.SetBool("isSitted", isSitted);
    }

    void Dance()
    {
        if (isDanceDown)
        {
            animator.SetTrigger("DoDance");
        }
    }

    void VoiceOnOff()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            VoiceManager.Instance.VoiceOnOff();
        }
    }

    void OnCursorVisible()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.None;
            CancelInvoke("OnCursorUnvisible");
            Invoke("OnCursorUnvisible", 3);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            OnCursorUnvisible();
        }
    }

    void OnCursorUnvisible()
    {
        if(!UIManager.Instance.isOpenWindow)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void ShowPlayerUIAsDistance()
    {
        for (int count = 0; count < RpcUIManager.Instance.playerList.Count; count++)
        {
            if (Vector3.Distance(RpcUIManager.Instance.playerList[count].transform.position, playerTransform.position) < 10)
            {
                RpcUIManager.Instance.playerUILIst[count].SetActive(true);
            }
            else
            {
                RpcUIManager.Instance.playerUILIst[count].SetActive(false);
            }
        }
    }

    void TeleportWayPoint()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerTransform.position = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerTransform.position = new Vector3(-23, 0.2f, 37);
        }
    }

    public void OpenOptionWindow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currentTouch.enabled = false;
            interactionUI.SetActive(false);
            if (UIManager.Instance.isOpenWindow)
                UIManager.Instance.CloseWindow();
            else if (UIManager.Instance.conferenceUI.activeSelf)
                ConferenceManager.Instance.ExitConference();
            else if (ClassProcessManager.Instance.isSittedChair)
                ClassProcessManager.Instance.GetUpFromChair();
            else
                UIManager.Instance.OptionBtn();
        }
    }

    public void OnKinematic(bool set)
    {
        photonView.RPC("OnKinematicRPC", RpcTarget.AllBuffered, set);
    }

    [PunRPC]
    public void OnKinematicRPC(bool set)
    {
        GetComponent<Rigidbody>().isKinematic = set;
    }

    private void OnDestroy()
    {
        RpcUIManager.Instance.playerList.Remove(gameObject.transform);
        RpcUIManager.Instance.playerUILIst.Remove(playerUIObjects);
        if (photonView.IsMine)
        {
            cameraController.enabled = false;
        }
    }
}
