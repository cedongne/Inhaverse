using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RpcUIManager : MonoBehaviour
{
    private static RpcUIManager instance;

    public static RpcUIManager Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<RpcUIManager>();
                if(obj != null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }

    public List<Transform> playerList;
    public List<GameObject> playerUILIst;

    public List<GameObject> webCamImageList;

    private void Awake()
    {
        if(instance == null)
        {
            instance = GetComponent<RpcUIManager>();
        }
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
