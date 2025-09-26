using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [AddComponentMenu("DevPeixoto/UI/Menu Manager/MenusManager")]
    public class MenusManager : MonoBehaviour
    {
        [SerializeField] bool firstSiblingIsTheDefault;
        [SerializeField] Menu defaultMenu;
        [SerializeField] bool handleOperationsNextFrame;
        [SerializeField] public bool nonRepeatedNav;
        [SerializeField] bool getMenusInChildren;
        [SerializeField] List<Menu> menus = new();
        [SerializeField, HideInInspector] internal List<string> menusNames = new ();

        List<Menu> currentMenuList = new ();
        Transform backgroundMenusParent;

        [Header("Events")]
        public UnityEvent onReachedLastMenu;

        private void Awake()
        {
            var bgMenusParent = new GameObject("BackgroundParent");
            backgroundMenusParent = bgMenusParent.GetComponent<Transform>();
            backgroundMenusParent.SetParent(transform);
            backgroundMenusParent.SetSiblingIndex(0);

            if (getMenusInChildren)
            {
                menus = GetComponentsInChildren<Menu>(true).ToList();
            }

            if (defaultMenu != null)
            {
                defaultMenu.Show();
                currentMenuList.Add(defaultMenu);
            }
            else if (firstSiblingIsTheDefault && menus.Count > 0)
            { 
                defaultMenu = menus[0];
                currentMenuList.Add(defaultMenu);
                defaultMenu.Show();
            }

            foreach (var menu in menus)
            {
                if (menu == null)
                    continue;

                menu.owner = this;
                menu.backgroundParent = backgroundMenusParent;
                menu.gameObject.SetActive(true);
                if (menu == defaultMenu)
                {
                    menu.Init(true);
                }
                else
                {
                    menu.Init(false);
                }
            }
        }

        /// <summary>
        /// Switchs to other menu
        /// </summary>
        /// <param name="menu">The target menu</param>
        public void Open(Menu menu)
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

            if (handleOperationsNextFrame)
            {
                StartCoroutine(HandleActionNextFrame(() =>
                {
                    if (nonRepeatedNav && currentMenuList.Contains(menu))
                    {
                        ReInsert(menu);
                        return;
                    }

                    Peek().Hide();

                    menu.Show();

                    currentMenuList.Add(menu);
                }));
            }
            else
            {
                if (nonRepeatedNav && currentMenuList.Contains(menu))
                {
                    ReInsert(menu);
                    return;
                }

                Peek().Hide();

                menu.Show();

                currentMenuList.Add(menu);
            }
        }

        public void Open(string menuName)
        {
            Open(menus.Find(x => x.name == menuName));
        }

        public void OpenOverlay(Menu menu)
        {
            if (menu == null)
                return;

            if (menu == defaultMenu)
                Debug.LogError("Can't open default menu as Overlay");

            if (!menus.Contains(menu))
            {
                Debug.LogError($"The Menu {menu.name} isn't part of this manager ({name}) group");
                return;
            }

            var peek = Peek();
            if (peek == menu)
                return;

            if (handleOperationsNextFrame)
            {
                StartCoroutine(HandleActionNextFrame(() =>
                {
                    if (nonRepeatedNav && currentMenuList.Contains(menu))
                    {
                        ReInsert(menu);
                        return;
                    }

                    peek.transform.SetParent(backgroundMenusParent);
                    peek.HandleCanvasGroupInteraction(false);

                    menu.Show();

                    currentMenuList.Add(menu);
                }));
            }
            else
            {
                if (nonRepeatedNav && currentMenuList.Contains(menu))
                {
                    ReInsert(menu);
                    return;
                }

                peek.transform.SetParent(backgroundMenusParent);
                peek.HandleCanvasGroupInteraction(false);

                menu.Show();

                currentMenuList.Add(menu);
            }
        }

        public void OpenOverlay(string menuName)
        {
            OpenOverlay(menus.Find(x => x.name == menuName));
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

            if (handleOperationsNextFrame)
            {
                StartCoroutine(HandleActionNextFrame(() =>
                {
                    var pop = Pop();
                    pop.Hide();

                    var peek = Peek();
                    peek.Show();
                }));
            }
            else
            {
                var pop = Pop();
                pop.Hide();

                var peek = Peek();
                peek.Show();
            }                
        }

        IEnumerator HandleActionNextFrame(Action action)
        {
            yield return null;
            action?.Invoke();
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
            menu.Show();
        }

#if UNITY_EDITOR
        void InEditorSetup()
        {
            SetupChildMenus();
            SetupMenuNamesList();
        }

        void SetupChildMenus()
        {
            menus.Where(m => m != null).ToList().ForEach(m => m.owner = this);
        }

        void SetupMenuNamesList()
        {
            menusNames = menus.Where(m => m != null).ToList().Select(m => m.name).ToList();
            menusNames.Sort();
        }

        [InitializeOnLoadMethod]
        static void OnUnityReload()
        {
            var allMenus = FindObjectsByType<MenusManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var item in allMenus)
            {
                EditorApplication.delayCall += item.InEditorSetup;
                EditorApplication.hierarchyChanged += item.InEditorSetup;
            }
        }

        private void OnValidate()
        {
            InEditorSetup();
        }

        private void Reset()
        {
            InEditorSetup();
        }
#endif
    }
}