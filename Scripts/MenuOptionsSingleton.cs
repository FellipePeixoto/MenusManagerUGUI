#if UNITY_EDITOR
using DevPeixoto.UI.MenuManager.UGUI;
using System.Collections.Generic;

namespace DevPeixoto.UI.MenuManager.UGUI
{
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
            if (target == null)
                return null;

            return target.menusNames;
        }
    }
}
#endif