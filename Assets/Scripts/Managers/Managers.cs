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

    public GameObject gameManager;
    public GameObject communicationManager;
    public GameObject fileManager;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = gameObject.GetComponent<Managers>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Instantiate(gameManager);
        Instantiate(communicationManager);
        Instantiate(fileManager);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
