using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public List<Menu> menuList;
    public void OpenMenu(string menuName)
    {
        foreach (Menu menu in menuList)
        {
            if (menu.menuName == menuName)
            {
                menu.Open();
                continue;
            }
            menu.Close();
        }
    }
    public void CloseAllMenus()
    {
        foreach (Menu menu in menuList)
        {
            menu.Close();
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}