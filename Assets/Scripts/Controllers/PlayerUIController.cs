using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerUIController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform playerTransform;
    public Text playerNameTextUI;
    public Transform playerNameTextTransform;
    public RectTransform playerNameTextBackgroundImage;
    public UnityEngine.UI.Outline playerNameTextOutline;

    public GameObject webCamImage;
    public PlayerContoller playerController;

    [SerializeField]
    private Vector3 playerNameTextOffset = new Vector3(0, 1, 0);

    bool isWebCamDown;

    private void Awake()
    {
        playerNameTextTransform.parent = GameObject.Find("Canvas").transform;
        transform.SetAsFirstSibling();
        Invoke("SetPlayerName", 1f);
    }

    void FixedUpdate()
    {
        playerNameTextTransform.position = Camera.main.WorldToScreenPoint(playerTransform.position + playerNameTextOffset);
        if(photonView.IsMine)
            CheckVoiceTransmitting();
        CheckMicColor();
    }

    public void CheckMicColor()
    {
        if (isOnVoice)
        {
            playerNameTextOutline.effectColor = Color.green;
        }
        else
        {
            playerNameTextOutline.effectColor = Color.white;
        }
    }

    public void ShowWebCamImage(bool onOff)
    {
        if (webCamImage.activeSelf)
            UIManager.Instance.curCamIcon.color = Color.gray;
        else
            UIManager.Instance.curCamIcon.color = Color.white;
        photonView.RPC("ShowWebCamImageRPC", RpcTarget.AllBuffered, onOff);
    }

    [PunRPC]
    public void ShowWebCamImageRPC(bool onOff)
    {
        webCamImage.SetActive(onOff);
    }

    void SetPlayerName()
    {
        if (photonView.IsMine)
            photonView.Owner.NickName = PlayfabManager.Instance.playerName;
        playerNameTextUI.text = photonView.Owner.NickName;

        playerNameTextBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerNameTextUI.text.Length * 32);
        playerNameTextBackgroundImage.gameObject.SetActive(true);
    }

    private bool isOnVoice;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        playerNameTextBackgroundImage.gameObject.SetActive(true);

        if (stream.IsReading)
        {
            isOnVoice = (bool)stream.ReceiveNext();
        }
        else if (stream.IsWriting)
        {
            stream.SendNext(isOnVoice);
        }
    }

    private void CheckVoiceTransmitting()
    {
        if (VoiceManager.Instance.voiceRecorder.IsCurrentlyTransmitting)
        {
            isOnVoice = true;
        }
        else
        {
            isOnVoice = false;
        }
    }
}
