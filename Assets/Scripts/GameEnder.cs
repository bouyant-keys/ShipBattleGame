using UnityEngine;

public class GameEnder : MonoBehaviour, IDataPersistence
{
    [SerializeField] bool _gameCompleted = false;
    int _shipsDestroyedThisGame;

    // Start is called before the first frame update
    void Start()
    {
        _shipsDestroyedThisGame = GameManager._enemyTotal;

        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().EndGame();
        GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataPersistenceManager>().SaveGame();
    }

    public void LoadData(GameData data)
    {
        //No data to load, but needed for interface
    }

    public void SaveData(GameData data)
    {
        if (_gameCompleted) data.wins++;
        else data.losses++;

        if (data.wins > 0) data.completedGame = true;
        data.shipsDestroyedEver += _shipsDestroyedThisGame;
    }
}
