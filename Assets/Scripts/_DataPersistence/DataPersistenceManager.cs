using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string savefileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects; //All scripts that derive from IDataPersistense - scripts that have data to be saved/loaded
    private FileDataHandler dataHandler;

    //Singleton pattern
    public static DataPersistenceManager instance { get; private set; }
    private void Awake() 
    {
        if (instance != null) 
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        instance = this;

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, savefileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void LoadNewGame() //Combo of NewGame() and LoadGame()
    {
        this.gameData = new GameData();

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void NewGame() 
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();
        
        // if no data can be loaded, initialize to a new game
        if (this.gameData == null) 
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.SaveData(gameData);
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit() 
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
