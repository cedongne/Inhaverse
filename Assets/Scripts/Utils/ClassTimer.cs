//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using Photon.Pun;
//using Photon.Realtime;

//using PlayFab.ClientModels;

//public class ClassTimer : MonoBehaviourPun
//{

//    // Start is called before the first frame update
//    void Start()
//    {
//        PlayfabManager.Instance.SetUserData("ClassTimer", "0");
        
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }


//    public void Counting()
//    {
//        PlayfabManager.Instance.IncreaseLeaderBoardValue("ClassTimer", PlayfabManager.Instance.playerName);
//        Invoke("Counting", 300f);
//    }
//}
