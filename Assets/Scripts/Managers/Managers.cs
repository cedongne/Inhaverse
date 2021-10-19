using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private Managers() { }

    private static Managers instance;
    public static Managers Instance
    {
        get
        {
            if(instance == null)
            {
                var obj = FindObjectOfType<Managers>();
                if(obj == null)
                {
                    instance = obj;
                }
            }

            return instance;
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = gameObject.GetComponent<Managers>();
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
