using UnityEngine;
using UnityEngine.UI;

public class AmmoTrackerUI : MonoBehaviour
{
    [SerializeField] Image[] _uiBullets;
    [SerializeField] Image[] _uiMines;

    [SerializeField] Color _uiEnabledColor;
    [SerializeField] Color _uiDisabledColor;

    public void UpdateBullets(int bulletCount)
    {
        if (bulletCount < 0 || bulletCount > _uiBullets.Length)
        {
            Debug.Log("The number of bullets in pool does not match range of bullet ui objects.");
        }

        for (int i = 0; i < _uiBullets.Length; i++)
        {
            if (i + 1 <= bulletCount)
            {
                _uiBullets[i].color = _uiEnabledColor;
                continue;
            }

            _uiBullets[i].color = _uiDisabledColor;
        }
    }

    public void UpdateMines(int mineCount)
    {
        if (mineCount < 0 || mineCount > _uiMines.Length)
        {
            Debug.Log("The number of mines in pool does not match range of mine ui objects.");
        }

        for (int i = 0; i < _uiMines.Length; i++)
        {
            if (i + 1 <= mineCount)
            {
                _uiMines[i].color = _uiEnabledColor;
                continue;
            }
            
            _uiMines[i].color = _uiDisabledColor;
        }
    }
}
