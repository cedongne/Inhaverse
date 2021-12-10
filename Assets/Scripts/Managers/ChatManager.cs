using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Chat;
using PN = Photon.Pun.PhotonNetwork;

public class ChatManager : MonoBehaviour, IChatClientListener
{
	public ChatClient chatClient;
	private string userName;
	public  string currentChannelName;
	private ChannelCreationOptions conferenceOption;

	public InputField inputField;
	public Text outputText;
	public GameObject manager;
	public Scrollbar chatScrollBar;
	public GameObject ifObject;
	public GameObject sbObject;

	public bool onChat;

	private ChatManager() { }
	private static ChatManager instance;

	public static ChatManager Instance
    {
        get
        {
			if(instance == null)
            {
				var obj = FindObjectOfType<ChatManager>();
				if(obj != null)
                {
					instance = obj;
                }
            }

			return instance;
        }
    }

	void Awake()
    {
		if (instance == null)
		{
			instance = GetComponent<ChatManager>();
			gameObject.SetActive(true);
			
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start()
	{
		Application.runInBackground = true;
		onChat = false;
		conferenceOption = new ChannelCreationOptions();
		conferenceOption.MaxSubscribers = 4;
		conferenceOption.PublishSubscribers = true;

		//		userName = manager.GetComponent<PlayfabManager>().playerName;
		userName = PlayfabManager.Instance.playerName;

//		ChatStart();
	}

	void Update()
	{
		if(chatClient != null)
			chatClient.Service();
		if (!UIManager.Instance.isOpenWindow)
		{
			Chat();
		}
	}

	public void ChatStart()
	{
		userName = PlayfabManager.Instance.playerName;
		Debug.Log(userName);

		StartCoroutine(ChatStartCoroutine());
	}

	IEnumerator ChatStartCoroutine()
    {
		chatClient = new ChatClient(this);
		chatClient.Connect(ChatSettings.Instance.AppId, "1.0", new AuthenticationValues(userName));
		this.enabled = true;

		yield return new WaitForEndOfFrame();
	}

	public void AddLine(string lineString)
	{
		outputText.text += lineString + "\r\n";
	}

	public void OnApplicationQuit()
	{
		if (chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	public GameObject chatBack;
	public void SetHUDChatUI()
    {
		inputField = UIManager.Instance.hudChatInputField;
		outputText = UIManager.Instance.hudChatText;
		chatScrollBar = UIManager.Instance.hudChatScrollbar;
		ifObject = UIManager.Instance.hudInputField;
		sbObject = UIManager.Instance.hudSendButton;
		chatBack = UIManager.Instance.hudChatBack;
	}

	public void SetConferenceChatUI()
    {
		inputField = UIManager.Instance.conferenceChatInputField;
		outputText = UIManager.Instance.conferenceChatText;
		chatScrollBar = UIManager.Instance.conferenceChatScrollbar;
		ifObject = UIManager.Instance.conferenceInputField;
		sbObject = UIManager.Instance.conferenceSendButton;
		chatBack = UIManager.Instance.conferenceChatBack;
	}

	public void OnConnected()
	{
		AddLine("서버에 연결되었습니다.");

		currentChannelName = PN.CurrentRoom.Name;
		chatClient.Subscribe(new string[] { currentChannelName }, 0);
	}

	public void OnDisconnected()
	{
		AddLine("서버에 연결이 끊어졌습니다.");
	}

	public void OnChatStateChange(ChatState state)
	{
		Debug.Log("OnChatStateChange = " + state);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		AddLine(string.Format("{0}에 입장하셨습니다.", string.Join(",", channels)));
		if(channels[0].Contains("Conference"))
			ConferenceManager.Instance.UpdateConferenceState();
	}

	public void EnterConferenceChat(string channelName)
    {
		chatClient.Subscribe(channelName, 0, 10, conferenceOption);
		currentChannelName = channelName;
		ConferenceManager.Instance.channelName = currentChannelName;
	}

	public void LeaveConferenceChat(string channelName)
    {
		chatClient.Unsubscribe(new string[] { channelName });
	}

	public void EnterLobbyChat()
	{
		chatClient.Subscribe(new string[] { "Campus" }, 10);
		currentChannelName = "Campus";
	}

	public void LeaveChat()
	{
		EraseText();
		chatClient.Unsubscribe(new string[] { currentChannelName });
	}

	public void EraseText()
    {
		Debug.Log("Erase");
		outputText.text = "";
		Debug.Log("Erased");
	}

	public void OnUnsubscribed(string[] channels)
	{
		AddLine(string.Format("{0}에서 퇴장하셨습니다.", string.Join(",", channels)));
		if (channels[0].Contains("Conference"))
			ConferenceManager.Instance.UpdateConferenceState();
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		for (int i = 0; i < messages.Length; i++)
		{
			AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		Debug.Log(sender + "의 귓속말: " + message.ToString());
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
	}

	void Chat()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
            if(!onChat)
            {
				onChat = true;
				ifObject.SetActive(true);
				inputField.ActivateInputField();
				sbObject.SetActive(true);
				chatBack.SetActive(true);
            }
            else
            {
				if (inputField.text != "")
				{
					chatClient.PublishMessage(currentChannelName, inputField.text);
					inputField.text = "";
				}
				onChat = false;
				ifObject.SetActive(false);
				sbObject.SetActive(false);
				chatBack.SetActive(false);
			}
		}
	}

	public void ExitConference()
    {
		LeaveChat();
		EnterLobbyChat();
    }

#region Useless
    public void OnUserSubscribed(string channel, string name) { }

	public void OnUserUnsubscribed(string channel, string name) { }

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message) { }

	public void Input_OnEndEdit(string text)
	{
		if (chatClient.State == ChatState.ConnectedToFrontEnd)
		{
			//chatClient.PublishMessage(currentChannelName, text);
			if (inputField.text != "")
			{
				chatClient.PublishMessage(currentChannelName, inputField.text);
				inputField.text = "";
			}
		}
	}
	#endregion
}
