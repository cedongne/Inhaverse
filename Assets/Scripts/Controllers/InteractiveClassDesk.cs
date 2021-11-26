using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }
}
