using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> _enemies; //DO NOT add any tanks into this list through the inspector!
    GameObject _player;
    SceneLoader _sceneLoader;
    UIManager _uiManager;
    MusicManager _musicManager;
    FadeScreen _fadeScreen;

    public static bool _inputEnabled = false;
    public static int _lives = 5;
    public static int _totalTanksDestroyed;
    bool _playerDestroyed = false; //If the player is destroyed do not check for remaining enemies
    bool _tanksDestroyed = false; //If the tanks are destroyed do not check for player destroyed

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");
        if (objs.Length > 1) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public void LevelStart()
    {
        _sceneLoader = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneLoader>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        _fadeScreen = GameObject.FindGameObjectWithTag("FadeScreen").GetComponent<FadeScreen>();
        _player = GameObject.FindGameObjectWithTag("Player");

        _playerDestroyed = false;
        _tanksDestroyed = false;

        _enemies.Clear();
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            _enemies.Add(enemy);
        }
        
        _fadeScreen.FadeIn();
        StartCoroutine(LevelIntro());
    }

    private IEnumerator LevelIntro()
    {
        _musicManager.StartLevelMusic(0);
        _uiManager.SetActiveMenu("IntroMenu");
        yield return new WaitForSeconds(_musicManager._levelMusic[0].length);

        _musicManager.StartLevelMusic(1);
        _uiManager.SetActiveMenu("DefaultMenu");
        yield return new WaitForSeconds(_musicManager._levelMusic[1].length);

        _inputEnabled = true;
        _musicManager.StartMusic(HighestLevel(), _enemies.Count);
    }

    private IEnumerator LevelOutro(bool win)
    {
        _inputEnabled = false;

        if (win)
        {
            _musicManager.EndLevelMusic(true);
            _uiManager.SetActiveMenu("ClearedMenu");

            yield return new WaitForSeconds(_musicManager._levelMusic[2].length);
            _fadeScreen.FadeOut();
            _sceneLoader.LoadNextLevel();
        }
        else
        {
            _musicManager.EndLevelMusic(false);

            yield return new WaitForSeconds(_musicManager._levelMusic[3].length);
            _fadeScreen.FadeOut();
            if (_lives < 0)
            {
                _sceneLoader.LoadScene("LoseScene");
            }
            else
            {
                _sceneLoader.ReloadScene();
            }
        }
    }

    private int HighestLevel() //Finds the active tank with the highest level in the scene
    {
        int tempNum = 0;
        for (int i = 0; i < _enemies.Count; i++)
        {
            int level = _enemies[i].GetComponent<EnemyController>()._tank._level;
            if (level > tempNum)
            {
                tempNum = level;
            }
        }
        return tempNum;
    }

    public void CheckRemainingEnemies() //Updates enemy count, total tanks destroyed, and determines whether to move to the next level
    {
        if (_playerDestroyed) return;

        int tempCount = _enemies.Count;
        for (int i = 0; i < _enemies.Count; i++) //loops through enemy list removing inactive/destroyed enemies
        {
            if (!_enemies[i].activeSelf)
            {
                _enemies.RemoveAt(i);
            }
        }
        _totalTanksDestroyed += (tempCount - _enemies.Count);
        _uiManager.UpdateUI();

        if (_enemies.Count == 0)
        {
            _tanksDestroyed = true;
            StartCoroutine(LevelOutro(true)); //Starts outro into next level
            return;
        }
        _musicManager.UpdateMusic(HighestLevel(), _enemies.Count);
    }

    public void PlayerDestroyed() //Updates life count and determines whether to reload level or restart game
    {
        if (_tanksDestroyed) return;

        _lives--;
        _playerDestroyed = true;
        StartCoroutine(LevelOutro(false));
    }
}
