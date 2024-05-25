using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] FadeScreen _fadeScreen;
    [SerializeField] SceneLoader _sceneLoader;
    public List<GameObject> _menuList;
    public GameObject _activeMenu;

    int _levelToLoad;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var menu in _menuList)
        {
            if (menu.name != "MainMenu")
            {
                menu.SetActive(false);
            }
        }

        _fadeScreen.FadeIn();
    }

    public void SetActiveMenu(string menuName)
    {
        GetActiveMenu().SetActive(false);

        //Finds and turns on the requested menu
        foreach (var menu in _menuList)
        {
            if (menu.name == menuName)
            {
                menu.SetActive(true);
                _activeMenu = menu;
                break;
            }
            menu.SetActive(false);
        }
    }

    public GameObject GetActiveMenu()
    {
        //Returns the active menu gameobject
        int index = -1;
        for (int i = 0; i < _menuList.Count; i++)
        {
            if (_menuList[i].activeSelf)
            {
                index = i;
            }
        }
        if (index != -1)
        {
            return _menuList[index];
        }
        return null;
    }

    public void LoadLevel(int buildIndex)
    {
        _levelToLoad = buildIndex;
        _fadeScreen.FadeOut();
        StartCoroutine(WaitToLoad());
    }

    IEnumerator WaitToLoad()
    {
        yield return new WaitForSeconds(1.5f);
        _sceneLoader.LoadSceneFromMenu(_levelToLoad);
    }
}
