using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    List<GameObject> _menuList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var menu in GameObject.FindGameObjectsWithTag("Menu"))
        {
            _menuList.Add(menu);
            if (menu.name != "MainMenu")
            {
                menu.SetActive(false);
            }
        }
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
}
