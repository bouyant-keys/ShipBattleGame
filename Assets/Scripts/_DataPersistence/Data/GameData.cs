[System.Serializable] //can be saved
public class GameData
{
    //Global game info
    public int shipsDestroyedEver;
    public bool completedGame;

    //In-game/Round info
    public int shipsDestroyedRound;
    public int livesRemaining;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        shipsDestroyedEver = 0;
        completedGame = false;

        shipsDestroyedRound = 0;
        livesRemaining = 5;
    }
}
