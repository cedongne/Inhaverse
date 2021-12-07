using System.Collections;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    private static MineManager instance;

    public static MineManager Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<MineManager>();
                if(obj != null)
                {
                    instance = obj;
                }
            }
            return instance;
        }
    }


    private void Awake()
    {
        if(instance == null)
        {
            instance = GetComponent<MineManager>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public PlayerContoller playerController;
    public GameObject player;

    public GameObject playerUI;

}