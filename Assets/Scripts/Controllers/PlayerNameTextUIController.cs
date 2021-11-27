using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerNameTextUIController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform playerTransform;
    public Text playerNameTextUI;
    public Transform playerNameTextTransform;
    public RectTransform playerNameTextBackgroundImage;

    public GameObject webCamImage;
    public PlayerContoller playerController;

    [SerializeField]
    private Vector3 playerNameTextOffset = new Vector3(0, 1, 0);

    bool isWebCamDown;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        playerNameTextBackgroundImage.gameObject.SetActive(true);
    }

    private void Awake()
    {
        playerNameTextTransform.parent = GameObject.Find("Canvas").transform;
        transform.SetAsFirstSibling();
        Invoke("SetPlayerName", 1f);
    }

    private void Update()
    {
        GetInput();
        TurnWebCam();
    }

    void FixedUpdate()
    {
        playerNameTextTransform.position = Camera.main.WorldToScreenPoint(playerTransform.position + playerNameTextOffset);
    }

    void GetInput()
    {
        isWebCamDown = Input.GetKeyDown(KeyCode.C);
    }

    public void TurnWebCam()
    {
        if (isWebCamDown)
        {
            photonView.RPC("TurnWebCamRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void TurnWebCamRPC()
    {
        Debug.Log(photonView.Owner.NickName + "Turn");
        if (webCamImage.activeSelf)
        {
            playerController.OnOffWebCamController(false);
            webCamImage.SetActive(false);
        }
        else
        {
            playerController.OnOffWebCamController(true);
            webCamImage.SetActive(true);
        }
    }



    void SetPlayerName()
    {
        if (photonView.IsMine)
            photonView.Owner.NickName = PlayfabManager.Instance.playerName;
        playerNameTextUI.text = photonView.Owner.NickName;

        playerNameTextBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerNameTextUI.text.Length * 32);
        playerNameTextBackgroundImage.gameObject.SetActive(true);

    }
}
