using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    MusicManager _musicManager;
    AudioManager _audioManager;

    [SerializeField] List<Menu> _menuList;
    [SerializeField] List<Text> _textList;

    Menu _activeMenu;

    private void Awake() 
    {
        _musicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<MusicManager>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < _textList.Count; i++)
        {
            _textList[i].UpdateText();
        }
    }

    public void SetActiveMenu(Menu.MenuType menuType)
    {
        //When called on an already active menu, it is turned off and the default menu is activated 
        if (_activeMenu)
        {
            if (_activeMenu._menuType == menuType && menuType != Menu.MenuType.Default)
            {
                SetActiveMenu(Menu.MenuType.Default);
                return;
            }
        }

        //Turns off all menus but the requested menu via name
        bool activate;
        foreach (var menu in _menuList)
        {
            activate = false;
            if (menu._menuType == menuType)
            {
                activate = true;
                _activeMenu = menu;
            }
            menu.gameObject.SetActive(activate);
        }

        UpdateUI();
    }

    public void SetActiveMenu(Menu setMenu)
    {
        //When called on an already active menu, it is turned off and the default menu is activated 
        if (_activeMenu == setMenu && setMenu._menuType != Menu.MenuType.Default)
        {
                SetActiveMenu(Menu.MenuType.Default);
            return;
        }

        //Turns off all menus but the requested menu via menu
        bool activate;
        foreach (var menu in _menuList)
        {
            activate = false;
            if (menu._menuType == setMenu._menuType)
            {
                activate = true;
                _activeMenu = menu;
            }
            menu.gameObject.SetActive(activate);
        }

        UpdateUI();
    }

    public Menu GetActiveMenu()
    {
        //Returns the index of the active menu
        int index = -1;
        for (int i = 0; i < _menuList.Count; i++)
        {
            if (_menuList[i].gameObject.activeSelf)
            {
                index = i;
            }
        }
        if (index != -1)
        {
            _activeMenu = _menuList[index];
            return _menuList[index];
        }
        return null;
    }

    public void Pause()
    {
        SetActiveMenu(Menu.MenuType.Pause);

        if (_activeMenu._menuType == Menu.MenuType.Pause)
        {
            Time.timeScale = 0;
            GameManager._globalInputEnabled = false;
        }
        else
        {
            Time.timeScale = 1;
            GameManager._globalInputEnabled = true;
        }
    }

    //Pause menu functions:
    public void QuitToMenu()
    {
        GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneLoader>().LoadStartScene();
    }

    public void ChangeMusicVolume(float value)
    {
        _musicManager._audioSource.volume = value;
    }

    public void ChangeSFXVolume(float value)
    {
        _audioManager._audioSource.volume = value;
    }
}
