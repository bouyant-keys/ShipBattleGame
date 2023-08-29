using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistence
{
    List<GameObject> _enemies = new List<GameObject>();
    SceneLoader _sceneLoader;
    UIManager _uiManager;
    MusicManager _musicManager;
    FadeScreen _fadeScreen;
    DataPersistenceManager _dataManager;

    public static int _enemyTotal;
    public static bool _globalInputEnabled = false;
    public static int _lives = 5;
    public static int _totalTanksDestroyed;
    bool _playerDestroyed = false; //If the player is destroyed do not check for remaining enemies
    bool _tanksDestroyed = false; //If the tanks are destroyed do not check for player destroyed

    void Awake() //Just found out that its safe to initialize variables like this in awake, fucking hell
    {
        _sceneLoader = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneLoader>();
        _uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        _musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        _fadeScreen = GameObject.FindGameObjectWithTag("FadeScreen").GetComponent<FadeScreen>();
        _dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataPersistenceManager>();
    }

    private void Start() 
    {
        _dataManager.LoadGame();

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            _enemies.Add(enemy);
        }
        _enemyTotal = _enemies.Count;

        _uiManager.UpdateUI();
        _fadeScreen.FadeIn();
        StartCoroutine(LevelIntro());
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
        _musicManager.StartMusic(HighestLevel(), _enemies.Count);
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
            int level = _enemies[i].GetComponent<IEnemyTank>().GetTankLevel();
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

    public void SaveData(GameData data)
    {
        data.shipsDestroyedEver += _totalTanksDestroyed;
        //data.completedGame

        data.shipsDestroyedRound = _totalTanksDestroyed;
        data.livesRemaining = _lives;
    }

    public void LoadData(GameData data)
    {
        //data.shipsDestroyedEver
        //data.completedGame

        _totalTanksDestroyed = data.shipsDestroyedRound;
        _lives = data.livesRemaining;
    }
}
