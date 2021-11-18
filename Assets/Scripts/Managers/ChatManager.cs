using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using PN = Photon.Pun.PhotonNetwork;

public class ChatManager : MonoBehaviour, IChatClientListener
{
	private ChatClient chatClient;
	private string userName;
	private string currentChannelName;

	public InputField inputField;
	public Text outputText;
	public GameObject manager;
	public Scrollbar chatScrollBar;
	public GameObject ifObject;
	public GameObject sbObject;

	public bool onChat;

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
		if(instance == null)
			instance = GetComponent<ChatManager>();
    }
	// Use this for initialization
	void Start()
	{
		Application.runInBackground = true;
		onChat = false;

		//		userName = manager.GetComponent<PlayfabManager>().playerName;
		Debug.Log(PlayfabManager.Instance.playerName);
		userName = PlayfabManager.Instance.playerName;

		ChatStart();
	}

	public void ChatStart()
    {
		chatClient = new ChatClient(this);
		chatClient.Connect(ChatSettings.Instance.AppId, "1.0", new AuthenticationValues(userName));
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

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{
		if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
		{
			Debug.LogError(message);
		}
		else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
		}
		else
		{
			Debug.Log(message);
		}
	}

	public void OnConnected()
	{
		AddLine("������ ����Ǿ����ϴ�.");

		currentChannelName = PN.CurrentRoom.Name;
		chatClient.Subscribe(new string[] { currentChannelName }, 10);
	}

	public void OnDisconnected()
	{
		AddLine("������ ������ ���������ϴ�.");
	}

	public void OnChatStateChange(ChatState state)
	{
		Debug.Log("OnChatStateChange = " + state);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		AddLine(string.Format("{0}�� �����ϼ̽��ϴ�.", string.Join(",", channels)));
	}

	public void EnterConferenceChat(string channelName)
    {
		chatClient.Subscribe(new string[] { channelName }, 10);
		currentChannelName = channelName;
    }

	public void LeaveConferenceChat(string channelName)
    {
		chatClient.Unsubscribe(new string[] { channelName });
	}

	public void EnterLobbyChat()
	{
		chatClient.Subscribe(new string[] { "Lobby" }, 10);
		currentChannelName = "Lobby";
	}

	public void LeaveChat()
    {
		chatClient.Unsubscribe(new string[] { currentChannelName });
		EraseText();
	}

	public void EraseText()
    {
		outputText.text = "";
    }

	public void OnUnsubscribed(string[] channels)
	{
		AddLine(string.Format("{0}���� �����ϼ̽��ϴ�.", string.Join(",", channels)));
	}
	public void OnUserSubscribed(string channel, string name) { }
	public void OnUserUnsubscribed(string channel, string name) { }

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		for (int i = 0; i < messages.Length; i++)
		{
			AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		Debug.Log(sender + "�� �ӼӸ�: " + message.ToString());
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
	}

	void Update()
	{
		chatClient.Service();
		Chat();
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
			}
		}
	}

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

	public void ExitConference()
    {
		LeaveChat();
		EnterLobbyChat();
		UIManager.Instance.CloseWindow();
    }

	void RenewalChannel()
	{
		UIManager.Instance.ConferenceMemberText.text = "[" + currentChannelName + "] " + PN.CurrentRoom.PlayerCount + " / " + PN.CurrentRoom.MaxPlayers;
	}
}