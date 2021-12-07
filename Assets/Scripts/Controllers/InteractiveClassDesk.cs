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

        deskNumInt = GetDeskNumInt(gameObject.name) - 1;

        chairPos.x = chairAddress.position.x - (deskNumInt / 10) * (25 / 20) - 0.45f;
        int xTmp = deskNumInt % 10;
        chairPos.z = chairAddress.position.z + (xTmp / 2) * (50.5f / 20) + (xTmp % 2) * (15 / 20);
        chairPos.y = chairAddress.position.y;

        MineManager.Instance.player.transform.position = chairPos;
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
