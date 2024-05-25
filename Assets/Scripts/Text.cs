using UnityEngine;
using TMPro;

public class Text : MonoBehaviour
{
    TMP_Text _text;

    public enum TextInfo
    {
        Lives,
        Mission,
        EnemyTanks,
        TotalTanks
    }
    public TextInfo _textType = TextInfo.Lives;
    [SerializeField] string _prefixes; //Any words or symbols that go before the variable (e.g. the "Mission : " part) 

    private void Awake() 
    {
        _text = GetComponent<TMP_Text>();
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(_prefixes)) _prefixes = "";

        switch (_textType)
        {
            case TextInfo.Lives:
                _text.text = $"{_prefixes}{GameManager._lives}";
                break;
            case TextInfo.Mission:
                _text.text = $"{_prefixes}{SceneLoader._currentLevelIndex}";
                break;
            case TextInfo.EnemyTanks:
                _text.text = $"{_prefixes}{GameManager._enemyTotal}";
                break;
            case TextInfo.TotalTanks:
                _text.text = $"{_prefixes}{GameManager._totalTanksDestroyed}";
                break;
        }
    }
}
