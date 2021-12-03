using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class InteractiveInstructorTable : InteractiveObject
{
    string class_name;

    private void Start()
    {
        class_name = NetworkManager.Instance.room_name;
    }

    public override void Interaction()
    {
        if (PlayfabManager.Instance.playerJob == "±³¼ö")
        {
            if (ClassProcessManager.Instance.classState == Define.CLASSSTATE.END)
            {
                ClassProcessManager.Instance.StartClass();
                UIManager.Instance.OpenWindow(Define.UI.CLASSSTUDENTLIST);
            }
            else
            {
                ClassProcessManager.Instance.EndClass();
            }
        }
        else
        {
            UIManager.Instance.StartCoroutine(UIManager.Instance.FadeOutDontHaveAuthority());
        }
    }
}
