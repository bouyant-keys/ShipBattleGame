using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreenText : MonoBehaviour
{
    TMP_Text _text;

    public enum TextInfo
    {
        Lives,
        TotalTanks
    }
    public TextInfo _textType = TextInfo.Lives;

    // Start is called before the first frame update
    void Start()
    {
        _text = this.GetComponent<TMP_Text>();
    }

    public void UpdateText()
    {
        switch (_textType)
        {
            case TextInfo.Lives:
                _text.text = $"{GameManager._lives}";
                break;
            case TextInfo.TotalTanks:
                _text.text = $"{GameManager._totalTanksDestroyed}";
                break;
        }
    }
}
