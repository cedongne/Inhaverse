using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTabMove: MonoBehaviour
{
    public List<InputField> inputList;
    private int curPos;

    // Start is called before the first frame update
    void Awake()
    {
        curPos = 0;
        inputList.Add(PlayfabManager.Instance.emailInput);
        inputList.Add(PlayfabManager.Instance.passwordInput);
        inputList.Add(PlayfabManager.Instance.usernameInput);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SkipField();
        }
    }

    public void SkipField()
    {
        curPos = GetCurrentPosition();
        if(curPos + 1 < inputList.Count)
        {
            curPos++;
        }
        else
        {
            curPos = (curPos + 1) % inputList.Count;
        }
        inputList[curPos].Select();
    }

    public int GetCurrentPosition()
    {
        for(int i = 0; i < inputList.Count; i++)
        {
            if(inputList[i].isFocused == true)
            {
                return i;
            }
        }

        return 0;
    }
}
