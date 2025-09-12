#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [CustomEditor(typeof(MenusManager))]
    [CanEditMultipleObjects]
    public class MenusManagerEditor : Editor
    {
        VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            root = Resources.Load<VisualTreeAsset>("XML/MenusManagerEditor").CloneTree();

            SerializedProperty defaultMenuProp = serializedObject.FindProperty("defaultMenu");
            var defaultMenuField = new PropertyField(defaultMenuProp);

            SerializedProperty firstSibProp = serializedObject.FindProperty("firstSiblingIsTheDefault");
            var firstSibField = new PropertyField(firstSibProp, "Use First Sibling as Default");
            firstSibField.RegisterValueChangeCallback(evnt =>
            {
                if (firstSibProp.boolValue)
                {
                    defaultMenuField.style.display = DisplayStyle.None;
                }
                else
                {
                    defaultMenuField.style.display = DisplayStyle.Flex;
                }
            });

            root.Add(firstSibField);
            root.Add(defaultMenuField);

            var nonRepeatedNavProp = serializedObject.FindProperty("nonRepeatedNav");
            root.Add(new PropertyField(nonRepeatedNavProp, "Do not repeat menus in stack"));

            var menusProp = serializedObject.FindProperty("menus");
            var menusField = new PropertyField(menusProp);

            var getMenusInChildrenProp = serializedObject.FindProperty("getMenusInChildren");
            var getMenusInChildrenField = new PropertyField(getMenusInChildrenProp, "Automatically get the menus in children");
            getMenusInChildrenField.RegisterValueChangeCallback(evnt =>
            {
                if (getMenusInChildrenProp.boolValue)
                {
                    menusField.style.display = DisplayStyle.None;
                }
                else
                {
                    menusField.style.display = DisplayStyle.Flex;
                }
            });
            root.Add(getMenusInChildrenField);

            root.Add(menusField);

            return root;
        }
    }
}
#endif