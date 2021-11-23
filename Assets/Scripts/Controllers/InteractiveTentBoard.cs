using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveTentBoard : InteractiveObject
{
    public RawImage rawImage;
    public bool imageExisted = false;
    public string hostName = "";
    public GameObject entrance;

    public override void Interaction()
    {
        UIManager.Instance.OpenWindow(Define.UI.OPENFILE);

        ShowHost();

        FileManager.Instance.board = this.gameObject;
        rawImage = UIManager.Instance.openFileWindow.transform.Find("RawImage").GetComponent<RawImage>();

        if (imageExisted)
        {
            Texture2D texture = TextureToTexture2D(GetComponent<MeshRenderer>().material.mainTexture);
            rawImage.texture = FileManager.Instance.RotateImage(texture, 90);
        }
    }

    public void ShowHost()
    {
        Debug.Log(hostName);
        if (hostName == "")
        {
            UIManager.Instance.openFileWindow.transform.Find("HostUI").gameObject.SetActive(true);
        }
        else if (hostName == PlayfabManager.Instance.playerName)
        {
            UIManager.Instance.openFileWindow.transform.Find("HostUI").gameObject.SetActive(true);
        }
        else
        {
            UIManager.Instance.openFileWindow.transform.Find("HostUI").gameObject.SetActive(false);
        }
    }

    public void SetNewHost()
    {
        hostName = PlayfabManager.Instance.playerName;
        Invoke("InitializeHost", 3600f);
    }

    public void DisconnectHost()
    {
        CancelInvoke();
        rawImage.texture = null;
        imageExisted = false;
        hostName = "";
        entrance.GetComponent<InteractiveTent>().SetTriggerOnOff();
    }

    private void InitializeHost()
    {
        rawImage.texture = null;
        imageExisted = false;
        hostName = "";
    }

    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }
}
