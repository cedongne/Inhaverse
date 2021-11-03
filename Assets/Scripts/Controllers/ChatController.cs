using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;

public class ChatController : MonoBehaviour, IChatClientListener
{

	private ChatClient chatClient;
	private string userName;
	private string currentChannelName;

	public InputField inputField;
	public Text outputText;
	public GameObject manager;
	public Scrollbar chatScrollBar;

	public bool onChat;

	// Use this for initialization
	void Start()
	{

		Application.runInBackground = true;
		onChat = false;

		userName = manager.GetComponent<PlayfabManager>().playerName;
		currentChannelName = "��üä��";

		chatClient = new ChatClient(this);
		chatClient.Connect(ChatSettings.Instance.AppId, "1.0", new AuthenticationValues(userName));

		AddLine(string.Format("����õ�", userName));
	}

	public void AddLine(string lineString)
	{
		outputText.text += lineString + "\r\n";
		chatScrollBar.value = 0;
		
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
		AddLine(string.Format("ä�� ���� ({0})", string.Join(",", channels)));
	}

	public void OnUnsubscribed(string[] channels)
	{
		AddLine(string.Format("ä�� ���� ({0})", string.Join(",", channels)));
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
		Debug.Log("OnPrivateMessage : " + message);
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
			inputField.Select();
			if (onChat)
				onChat = false;
			else
				onChat = true;
		}
	}

	public void Input_OnEndEdit(string text)
	{
		if (chatClient.State == ChatState.ConnectedToFrontEnd)
		{
			//chatClient.PublishMessage(currentChannelName, text);
			if(inputField.text != "")
				chatClient.PublishMessage(currentChannelName, inputField.text);

			inputField.text = "";
		}
	}
}
