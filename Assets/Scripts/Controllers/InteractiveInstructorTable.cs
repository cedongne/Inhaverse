using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class InteractiveInstructorTable : InteractiveObject
{
    string class_name;
    public Transform professorViewObject;

    private void Start()
    {
        class_name = NetworkManager.Instance.room_name;
    }

    public override void Interaction()
    {
        if (PlayfabManager.Instance.playerJob == "교수")
        {
            if (ClassProcessManager.Instance.classState == Define.CLASSSTATE.END)
            {
                ClassProcessManager.Instance.ReadyClass();
                ClassProcessManager.Instance.SetProfessorView(professorViewObject);
            }
            else
            {
                ClassProcessManager.Instance.EndClass();
            }
        }
        else
        {
            UIManager.Instance.StopCoroutine(UIManager.Instance.FadeOutwarningMessageUI(""));
            UIManager.Instance.StopCoroutine(UIManager.Instance.FadeOutCoroutine());
            UIManager.Instance.StartCoroutine(UIManager.Instance.FadeOutwarningMessageUI("접근 권한이 없습니다!"));
            
        }
    }
}
