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

    [SerializeField]
    private Vector3 playerNameTextOffset = new Vector3(0, 1, 0);

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

    void FixedUpdate()
    {
        playerNameTextTransform.position = Camera.main.WorldToScreenPoint(playerTransform.position + playerNameTextOffset);
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
