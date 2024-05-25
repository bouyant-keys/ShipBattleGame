using UnityEngine;

public class RoundStarter : MonoBehaviour
{
    private void Start() 
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().NewRound();
    }
}
