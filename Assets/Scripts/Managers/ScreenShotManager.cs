using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotManager : MonoBehaviour
{
    private static ScreenShotManager instance;

    public static ScreenShotManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<ScreenShotManager>();
                if (obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<ScreenShotManager>();
        }
    }

    public Image uiImage;
    public byte[] pngData;

    IEnumerator charkack()
    {
        UIManager.Instance.hudUI.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);
        uiImage.sprite = Sprite.Create(screenTexture, area, Vector2.one * 5f);
        pngData = screenTexture.EncodeToPNG();

        UIManager.Instance.OpenWindow(Define.UI.SCREENSHOT);
        UIManager.Instance.hudUI.SetActive(true);
    }
    public void TakeScreenShot()
    {
        StartCoroutine(charkack());
    }

    public void RemoveScreenShot()
    {
        uiImage.sprite = null;
        UIManager.Instance.CloseWindow();
    }

    public void OpenDialogToSave()
    {
        FileManager.Instance.SaveFile();
    }
}
