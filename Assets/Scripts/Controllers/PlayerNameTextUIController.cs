using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerNameTextUIController : MonoBehaviourPunCallbacks
{
    public Transform playerTransform;
    public Text playerNameTextUI;
    public Transform playerNameTextTransform;
    public RectTransform playerNameTextBackgroundImage;

    [SerializeField]
    private Vector3 playerNameTextOffset = new Vector3(0, 1, 0);

    private void Awake()
    {
        transform.parent = GameObject.Find("Canvas").transform;
        Invoke("SetPlayerName", 1f);
    } 

    // Update is called once per frame
    void FixedUpdate()
    {
        playerNameTextTransform.position = Camera.main.WorldToScreenPoint(playerTransform.position + playerNameTextOffset);
    }

    void SetPlayerName()
    {
        if (photonView.IsMine)
            playerNameTextUI.text = PlayfabManager.Instance.playerName;
        playerNameTextBackgroundImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerNameTextUI.text.Length * 32);
        playerNameTextBackgroundImage.gameObject.SetActive(true);

    }
}
