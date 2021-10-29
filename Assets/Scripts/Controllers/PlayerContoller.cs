using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private RectTransform interactionUITransform;

    Animator animator;

    public float walkMoveSpeed;
    public float runMoveSpeed;
    float moveSpeed;

    public float prevYPosition;
    public float jumpPower;

    bool isRun;
    bool isJump;
    bool isDown;

    public Outline currentTouch;

    Vector3 InteractionUIOffset = new Vector3(120, -50);

    private void Awake()
    {
        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        animator = GetComponent<Animator>();
        cameraArm = GameObject.Find("Camera Arm");
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        if (photonView.IsMine)
        {
            cameraArm.GetComponent<CameraController>().playerTransform = transform;
        }
        cameraArmTransform = cameraArm.transform;
        moveSpeed = walkMoveSpeed;

        interactionUI = GameObject.Find("Canvas").transform.Find("Interaction UI").gameObject;
        interactionUITransform = interactionUI.GetComponent<RectTransform>();
 //       interactionUITransform.SetParent(GameObject.Find("Canvas").transform);

        currentTouch = GameObject.Find("Initializing Object").GetComponent<Outline>();
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
            Move();
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
            Jump();
            walkToRun();
            DetectInteractiveObject();
        }
    }

    void walkToRun()
    {
        if (Input.GetKeyDown(KeyCode.R))
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
        if (isDown)
        {
            RaycastHit hit;
            Debug.DrawRay(playerTransform.position, Vector3.down * 0.1f, Color.red);
            if (Physics.Raycast(playerTransform.position, Vector3.down, out hit, 0.1f))
            {
                isJump = false;
                isDown = false;
                animator.SetBool("isJump", isJump);
                animator.SetBool("isDown", isDown);
            }
        }
        else if (isJump)
        {
            if (playerRigid.velocity.y < 0)
            {
                isDown = true;
            }
            animator.SetBool("isDown", isDown);
        }
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            animator.SetTrigger("DoJump");
            playerRigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            animator.SetBool("isJump", isJump);
        }
    }
    void DetectInteractiveObject()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(screenCenter), out hit, 5);
        if (hit.collider != null && hit.transform.gameObject.CompareTag("Interactive Object"))
        {
            currentTouch = hit.transform.gameObject.GetComponent<Outline>();
            currentTouch.enabled = true;
            interactionUI.SetActive(true);

            if (Input.GetMouseButtonDown(0))
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
    /*
    void DetectInteractiveObject()
    {
        RaycastHit hit;
        Physics.Raycast(playerTransform.position, playerTransform.forward, out hit, 2);
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        Physics.Raycast(ray, out hit, 5);
        if (hit.collider != null && hit.transform.gameObject.CompareTag("Interactive Object"))
        {
            currentTouch = hit.transform.gameObject.GetComponent<Outline>();
            currentTouch.enabled = true;
            interactionUI.SetActive(true);

            interactionUITransform.position = Input.mousePosition + InteractionUIOffset;

            if (Input.GetMouseButtonDown(0))
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
    */
}
