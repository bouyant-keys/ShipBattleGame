using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public static int _currentLevelIndex => SceneManager.GetActiveScene().buildIndex;

    public void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void LoadSceneFromMenu(int index)
    {
        SceneManager.LoadScene(_currentLevelIndex + index);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(_currentLevelIndex + 1);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(_currentLevelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
