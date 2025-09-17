#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using UnityEngine;


namespace DevPeixoto.UI.MenuManager.UGUI
{
    [AddComponentMenu("DevPeixoto/UI/Menu Manager/Nav Button")]
    public class ButtonMenuNav : ButtonMenuBase 
    {
        [SerializeField] internal NavContainer navContainer = new NavContainer();

        protected override void NavigateToDestiny()
        {
            if (navContainer.owner == null)
                return;

            if (navContainer.Overlay)
                navContainer.owner.OpenOverlay(navContainer.targetMenu);
            else
                navContainer.owner.Open(navContainer.targetMenu);
        }

        private void OnValidate()
        {
            navContainer.selfRef = this;
        }

        private void Reset()
        {
            navContainer.selfRef = this;
        }

        [Serializable]
        internal class NavContainer
        {
            public ButtonMenuNav selfRef;
            public MenusManager owner;
            public string targetMenu = "None";
            public bool Overlay;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/DevPeixoto/Menus Manager/Nav Button - TextMeshPro")]
        public static void CreateFromHierachyButtonNav(MenuCommand menuCommand)
        {
            var component = Resources.Load<GameObject>("Prefabs/ButtonMenuNav");

            if (component == null)
                return;

            var parent = Selection.activeGameObject;

            if (menuCommand.context as GameObject == null)
                parent = menuCommand.context as GameObject;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(component);
            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (parent != null)
                GameObjectUtility.SetParentAndAlign(instance, parent);

            Undo.RegisterCreatedObjectUndo(instance, "Create Navigation Button");

            Selection.activeGameObject = instance;
        }
#endif
    }
}