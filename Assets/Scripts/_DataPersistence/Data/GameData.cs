[System.Serializable] //can be saved
public class GameData
{
    //Global game info
    public int shipsDestroyedEver;
    public int losses;
    public int wins;
    public bool completedGame;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        shipsDestroyedEver = 0;
        losses = 0;
        wins = 0;
        completedGame = false;
    }
}
