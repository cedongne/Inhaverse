using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveClassDesk : InteractiveObject
{
    public override void Interaction()
    {
        if (PlayfabManager.Instance.playerJob == "�л�")
        {
            SittingChair();
        }
    }

    public void SittingChair()
    {

    }
}
