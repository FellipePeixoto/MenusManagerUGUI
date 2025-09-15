#if UNITY_EDITOR
using DevPeixoto.UI.MenuManager.UGUI;
using System.Collections.Generic;

public class MenuOptionsSingleton
{
    static MenuOptionsSingleton instance;
    public static MenuOptionsSingleton Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new MenuOptionsSingleton();
            }
            return instance; 
        }
    }

    public List<string> GetOptions(MenusManager target)
    {
        return target.menusNames;
    }
}
#endif