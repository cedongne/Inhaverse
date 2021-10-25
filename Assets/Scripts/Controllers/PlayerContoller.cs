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
    private RectTransform interactionUITransform;

    Animator animator;

    public float walkMoveSpeed;
    public float runMoveSpeed;
    float moveSpeed;

    public float jumpPower;
    public float jumpSpeed;

    bool isRun;
    bool isJump;

    public Outline currentTouch;

    Vector3 InteractionUIOffset = new Vector3(120, -50);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cameraArm = GameObject.Find("Camera Arm");
    }

    // Start is called before the first frame update
    void Start()
    {
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
        if (isJump)
        {
            jumpSpeed -= Time.deltaTime * 0.1f;
            playerTransform.position += new Vector3(0, jumpSpeed, 0);
            animator.SetFloat("jumpSpeed", jumpSpeed);

            if (playerTransform.position.y <= 0)
            {
                jumpSpeed = 0;
                isJump = false;
                animator.SetBool("isJump", isJump);
            }
        }
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            animator.SetTrigger("DoJump");
            jumpSpeed = jumpPower * 0.1f;
            isJump = true;
            animator.SetBool("isJump", isJump);
        }
    }

    void DetectInteractiveObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
        if (hit.collider != null && (playerTransform.localPosition - hit.transform.localPosition).magnitude < 3 && hit.transform.gameObject.CompareTag("Interactive Object"))
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
}