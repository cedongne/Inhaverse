using UnityEngine;
using WebSocketSharp;

public class SocketIoClient : MonoBehaviour
{
    private WebSocket m_WebSocket;

    void Start()
    {
        m_WebSocket = new WebSocket("ws://localhost:3000");
        m_WebSocket.Connect();

        m_WebSocket.OnMessage += (sender, e) =>
        {
            Debug.Log($"{((WebSocket)sender).Url}ø°º≠ + µ•¿Ã≈Õ : {e.Data}∞° ø».");
        };
    }

    void Update()
    {
        if (m_WebSocket == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_WebSocket.Send("æ»≥Á");
        }
    }
}