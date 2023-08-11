using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    Canvas _canvas;
    List<GameObject> _menuList = new List<GameObject>();
    List<Text> _textList = new List<Text>();
    Vector3 _activeMenuPos;
    Vector3 _inactiveMenuPos;

    void Start()
    {
        _canvas = GameObject.FindObjectOfType<Canvas>();
        _activeMenuPos = _canvas.transform.position;
        _inactiveMenuPos = new Vector3(_canvas.renderingDisplaySize.x * 2, _canvas.renderingDisplaySize.y / 2, 0f);

        //Finds and adds all menus to the list, while setting the default menu active
        foreach (var menu in GameObject.FindGameObjectsWithTag("Menu"))
        {
            _menuList.Add(menu);
        }

        foreach (var text in GameObject.FindGameObjectsWithTag("Text"))
        {
            _textList.Add(text.GetComponent<Text>());
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _textList.Count; i++)
        {
            _textList[i].UpdateText();
        }
    }

    public void SetActiveMenu(string menuName)
    {
        //Moves active menu aside and updates the menu data
        MoveMenu(GetActiveMenu(), false);
        UpdateUI();

        //Finds and turns on the requested menu
        foreach (var menu in _menuList)
        {
            if (menu.name == menuName)
            {
                MoveMenu(menu, true);

                //Worry about pausing later
                //if (menuName == "PauseMenu")
                //{
                    //Pause(menu.activeSelf);
                //}
            }
            else
            {
                MoveMenu(menu, false);
            }
        }
    }

    public GameObject GetActiveMenu()
    {
        //Returns the index of the active menu
        int index = -1;
        for (int i = 0; i < _menuList.Count; i++)
        {
            if (_menuList[i].transform.position == _canvas.transform.position)
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

    //Make this work
    private void Pause(bool isActive)
    {
        if (isActive)
        {
            Time.timeScale = 0;
            GameManager._inputEnabled = false;
        }
        else
        {
            Time.timeScale = 1;
            GameManager._inputEnabled = true;
        }
    }

    private void MoveMenu(GameObject menu, bool isActiveMenu)
    {
        if (isActiveMenu)
        {
            menu.transform.position = _activeMenuPos;
        }
        else
        {
            menu.transform.position = _inactiveMenuPos;
        }
    }
}
