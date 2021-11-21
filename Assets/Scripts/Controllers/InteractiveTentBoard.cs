using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTentBoard : InteractiveObject
{
    public override void Interaction()
    {
        UIManager.Instance.OpenWindow(Define.UI.OPENFILE);
        FileManager.Instance.board = this.gameObject;
    }
}
