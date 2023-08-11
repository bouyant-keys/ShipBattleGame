using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var text in GameObject.FindGameObjectsWithTag("Text"))
        {
            text.GetComponent<WinScreenText>().UpdateText();
        }
    }
}
