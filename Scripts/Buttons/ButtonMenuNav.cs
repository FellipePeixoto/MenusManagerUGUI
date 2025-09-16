#if UNITY_EDITOR
using System;
using System.Linq.Expressions;
using UnityEditor;
#endif
using UnityEngine;


namespace DevPeixoto.UI.MenuManager.UGUI
{
    [Serializable]
    public class ButtonMenuNav : ButtonMenuBase 
    {
        [SerializeField] internal MenusManager owner;
        [SerializeField] internal string targetMenu = "None";

        protected override void NavigateToDestiny()
        {
            if (owner != null)
                owner.SwitchTo(targetMenu);
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