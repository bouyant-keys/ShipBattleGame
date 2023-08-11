using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Text : MonoBehaviour
{
    GameManager _gameManager;
    TMP_Text _text;

    public enum TextInfo
    {
        Lives,
        Mission,
        EnemyTanks,
        TotalTanks
    }
    public TextInfo _textType = TextInfo.Lives;
    public string _prefixes = ""; //Any words or symbols that go before the variable (e.g. the "Mission : " part) 

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _text = this.GetComponent<TMP_Text>();
    }

    public void UpdateText()
    {
        switch (_textType)
        {
            case TextInfo.Lives:
                _text.text = $"{_prefixes}{GameManager._lives}";
                break;
            case TextInfo.Mission:
                _text.text = $"{_prefixes}{SceneLoader._currentLevel}";
                break;
            case TextInfo.EnemyTanks:
                _text.text = $"{_prefixes}{_gameManager._enemies.Count}";
                break;
            case TextInfo.TotalTanks:
                _text.text = $"{_prefixes}{GameManager._totalTanksDestroyed}";
                break;
        }
    }
}
