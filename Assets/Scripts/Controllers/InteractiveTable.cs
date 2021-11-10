using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTable : InteractiveObject
{
    // Start is called before the first frame update
    public ChatController chatController;

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interaction()
    {
        chatController.LeaveChat();
        chatController.EnterConferenceChat(this.transform.parent.name);
    }
}
