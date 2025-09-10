using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [AddComponentMenu("DevPeixoto/UI/Menu Manager/MenusManager")]
    public class MenusManager : MonoBehaviour
    {
        [SerializeField] bool firstSiblingIsTheDefault;
        [SerializeField] Menu defaultMenu;
        [SerializeField] bool nonRepeatedNav;
        [SerializeField] bool getMenusInChildren;
        [SerializeField] List<Menu> menus = new();

        List<Menu> currentMenuList = new ();
        Transform backgroundMenusParent;

        [Header("Events")]
        public UnityEvent onReachedLastMenu;

        private void Awake()
        {
            var bgMenusParent = new GameObject("BackgroundMenusParent");
            backgroundMenusParent = bgMenusParent.GetComponent<Transform>();
            backgroundMenusParent.SetParent(transform);
            backgroundMenusParent.SetSiblingIndex(0);

            if (getMenusInChildren)
            {
                menus = GetComponentsInChildren<Menu>(true).ToList();
            }

            if (defaultMenu != null)
            {
                defaultMenu.Show(true, transform);
                currentMenuList.Add(defaultMenu);
            }
            else if (firstSiblingIsTheDefault && menus.Count > 0)
            { 
                defaultMenu = menus[0];
                currentMenuList.Add(defaultMenu);
                defaultMenu.Show(true, transform);
            }

            foreach (var menu in menus)
            {
                menu.gameObject.SetActive(true);
                if (menu == defaultMenu)
                {
                    menu.Init(this, true);
                }
                else
                {
                    menu.Init(this, false);
                }
            }
        }

        /// <summary>
        /// Switchs to other menu
        /// </summary>
        /// <param name="menu">The target menu</param>
        public void SwitchTo(Menu menu)
        {
            if (menu == null)
                return;

            if (!menus.Contains(menu))
            {
                Debug.LogError($"The Menu {menu.name} isn't part of this manager ({name}) group");
                return;
            }

            if (Peek() == menu)
                return;

            if (nonRepeatedNav && currentMenuList.Contains(menu))
            {
                ReInsert(menu);
                return;
            }

            Peek().Hide(true, backgroundMenusParent);

            menu.Show(true, transform);
            currentMenuList.Add(menu);
        }

        /// <summary>
        /// Triggs a back action, returning to the last active menu.
        /// </summary>
        public void Back()
        {
            if (currentMenuList.Count <= 1)
            {
                onReachedLastMenu?.Invoke();
                return;
            }

            Pop().Hide(true, backgroundMenusParent);
            Peek().Show(true, transform);
        }

        Menu Peek() 
        {
            return currentMenuList[currentMenuList.Count - 1];
        }

        Menu Pop()
        {
            var popped = currentMenuList[currentMenuList.Count - 1];
            currentMenuList.RemoveAt(currentMenuList.Count - 1);
            return popped;
        }

        void ReInsert(Menu menu)
        {
            if (currentMenuList[currentMenuList.Count - 1] == menu)
                return;

            Peek().Hide();
            currentMenuList.Remove(menu);
            currentMenuList.Add(menu);
            menu.Show(true, transform);
        }
    }
}