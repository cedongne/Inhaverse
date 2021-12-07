using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveClassDesk : InteractiveObject
{

    public Transform chairAddress;

    private Vector3 chairPos;
    private int deskNumInt;

    public override void Interaction()
    {
        SittingChair();
    }

    public void SittingChair()
    {
        UIManager.Instance.ShowSubUI(Define.UI.CLASSCHAIR);
        //StudentListUIManager.Instance.SetStudentListUI(GetDeskNum(gameObject.name), PlayfabManager.Instance.playerName);
        MineManager.Instance.player.transform.GetComponent<Rigidbody>().isKinematic = true;

        Debug.Log(GetDeskNumInt(gameObject.name));
        deskNumInt = (GetDeskNumInt(gameObject.name) - 1);

        int z = deskNumInt / 10;
        chairPos.x = chairAddress.position.x - (float)z * 1.25f - 0.45f;
        int xf = (deskNumInt % 10) / 2;
        int xs = (deskNumInt % 10) % 2;
        chairPos.z = chairAddress.position.z + (float)xf * 2.525f + (float)xs * 0.75f;
        chairPos.y = chairAddress.position.y;

        Debug.Log(chairPos);
        MineManager.Instance.player.transform.position = chairPos;
        Debug.Log(MineManager.Instance.player.transform.position);
        MineManager.Instance.player.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public string GetDeskNum(string str)
    {
        string strTmp = Regex.Replace(str, @"\D", "");
        return strTmp;
    }

    public int GetDeskNumInt(string str)
    {
        string strTmp = Regex.Replace(str, @"\D", "");
        return int.Parse(strTmp);
    }
}
