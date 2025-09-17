#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [AddComponentMenu("DevPeixoto/UI/Menu Manager/Nav Button")]
    public class ButtonMenuBack : ButtonMenuBase 
    {
        [SerializeField] internal MenusManager owner;

        protected override void NavigateToDestiny()
        {
            if (owner != null)
                owner.Back();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/DevPeixoto/Menus Manager/Back Button - TextMeshPro")]
        public static void CreateFromHierachyButtonNav(MenuCommand menuCommand)
        {
            var component = Resources.Load<GameObject>("Prefabs/ButtonMenuBack");

            if (component == null)
                return;

            var parent = Selection.activeGameObject;

            if (menuCommand.context as GameObject == null)
                parent = menuCommand.context as GameObject;

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(component);
            PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            if (parent != null)
                GameObjectUtility.SetParentAndAlign(instance, parent);

            Undo.RegisterCreatedObjectUndo(instance, "Create Back Button");

            Selection.activeGameObject = instance;
        }
#endif
    }
}