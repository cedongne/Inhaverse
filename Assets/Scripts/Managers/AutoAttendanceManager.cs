using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class AutoAttendanceManager : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        if(/*PN.CurrentRoom.Name.Equals("OpenClass") ||*/ PN.CurrentRoom.Name.Equals("Lobby"))
        {
            PlayfabManager.Instance.getLeaderBoardValueEvent.AddListener(GetAttendanceDataEventCallback);
            PlayfabManager.Instance.GetLeaderBoardUserValue("Attendance " + PN.CurrentRoom.Name, PlayfabManager.Instance.playerName);
            PlayfabManager.Instance.UpdateLeaderBoard("Attendance " + PN.CurrentRoom.Name, 1);

        }
    }

    public void GetAttendanceDataEventCallback(int attendance)
    {
        Debug.Log(attendance);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
