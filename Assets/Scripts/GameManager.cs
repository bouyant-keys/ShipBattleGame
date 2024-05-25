using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject[] _enemies;
    SceneLoader _sceneLoader;
    UIManager _uiManager;
    MusicManager _musicManager;
    FadeScreen _fadeScreen;

    public static int _enemyTotal = 0;
    public static bool _globalInputEnabled = false;
    public static int _lives = 5;
    public static int _totalTanksDestroyed = 0;
    bool[] _enemyStatus;
    bool _levelRestart = false;
    bool _playerDestroyed = false; //If the player is destroyed do not check for remaining enemies
    bool _tanksDestroyed = false; //If the tanks are destroyed do not check for player destroyed

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");
        if (objs.Length > 1) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeVars()
    {
        _sceneLoader = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneLoader>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        _fadeScreen = GameObject.FindGameObjectWithTag("FadeScreen").GetComponent<FadeScreen>();
        _playerDestroyed = false;
        _tanksDestroyed = false;
    }

    public void NewRound()
    {
        InitializeVars();

        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (_levelRestart) //On restart, deactivates previously destroyed tanks
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].SetActive(_enemyStatus[i]);
            }
            _levelRestart = false;
        }
        else 
        {
            _enemyStatus = new bool[_enemies.Length];
            Array.Fill(_enemyStatus, true);
        }
        UpdateEnemyCount();

        _uiManager.UpdateUI();
        _fadeScreen.FadeIn();
        StartCoroutine(LevelIntro());
    }

    public void EndGame()
    {
        Destroy(this);
    }

    private IEnumerator LevelIntro()
    {
        _musicManager.StartLevelMusic(0);
        _uiManager.SetActiveMenu(Menu.MenuType.Intro);
        yield return new WaitForSeconds(_musicManager._levelMusic[0].length);

        _musicManager.StartLevelMusic(1);
        _uiManager.SetActiveMenu(Menu.MenuType.Default);
        yield return new WaitForSeconds(_musicManager._levelMusic[1].length);

        _globalInputEnabled = true;
        _musicManager.PlayStartMusic();
    }

    private IEnumerator LevelOutro(bool win)
    {
        _globalInputEnabled = false;

        if (win)
        {
            _musicManager.EndLevelMusic(true);
            _uiManager.SetActiveMenu(Menu.MenuType.Cleared);

            yield return new WaitForSeconds(_musicManager._levelMusic[2].length);
            _fadeScreen.FadeOut();

            yield return new WaitForSeconds(0.5f);
            _sceneLoader.LoadNextLevel();
        }
        else
        {
            _musicManager.EndLevelMusic(false);
            yield return new WaitForSeconds(_musicManager._levelMusic[3].length);

            _fadeScreen.FadeOut();
            yield return new WaitForSeconds(0.5f);
            if (_lives < 0)
            {
                _sceneLoader.LoadScene("LoseScene");
            }
            else
            {
                _levelRestart = true;
                _sceneLoader.ReloadScene();
            }
        }
    }

    private void UpdateEnemyCount()
    {
        int tempCount = 0;
        for (int i = 0; i < _enemies.Length; i++) //Updates enemyStatus, so on restart the active ships on death will remain
        {
            if (!_enemies[i].activeSelf)
            {
                _enemyStatus[i] = false;
                continue;
            }
            _enemyStatus[i] = true;
            tempCount++;
        }
        _enemyTotal = tempCount;
        _uiManager.UpdateUI();
    }

    public void EnemyDestroyed() //Updates enemy count when an enemy is destroyed
    {
        if (_playerDestroyed) return;

        print("Enemy ship destroyed");
        _totalTanksDestroyed++;
        UpdateEnemyCount();

        if (_enemyTotal == 0)
        {
            _tanksDestroyed = true;
            StartCoroutine(LevelOutro(true)); //Starts outro into next level
            return;
        }
    }

    public void PlayerDestroyed() //Updates life count and determines whether to reload level or restart game
    {
        if (_tanksDestroyed) return;

        _lives--;
        _playerDestroyed = true;
        StartCoroutine(LevelOutro(false));
    }
}
