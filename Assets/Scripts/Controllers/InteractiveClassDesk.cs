using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveClassDesk : InteractiveObject
{
    public override void Interaction()
    {
        if (PlayfabManager.Instance.playerJob == "ÇÐ»ý")
        {
            SittingChair();
        }
    }

    public void SittingChair()
    {
        StudentListUIManager.Instance.SetStudentListUI(GetDeskNum(gameObject.name), PlayfabManager.Instance.playerName);
    }

    public string GetDeskNum(string str)
    {
        string strTmp = Regex.Replace(str, @"\D", "");
        return strTmp;
    }
}
