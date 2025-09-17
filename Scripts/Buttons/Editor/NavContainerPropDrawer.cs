using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static DevPeixoto.UI.MenuManager.UGUI.ButtonMenuNav;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [CustomPropertyDrawer(typeof(NavContainer))]
    public class NavContainerPropDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty prop)
        {
            var root = new VisualElement();

            ObjectField selfField = new ObjectField();
            selfField.value = prop.FindPropertyRelative("selfRef").objectReferenceValue;
            selfField.SetEnabled(false);
            root.Add(selfField);

            ObjectField ownerField = new ObjectField("Respective Menu Owner");
            var ownerProp = prop.FindPropertyRelative("owner");
            ownerField.value = ownerProp.objectReferenceValue;
            ownerField.SetEnabled(false);
            ownerField.style.marginTop = 5;
            ownerField.style.marginBottom = 5;
            root.Add(ownerField);

            DropdownField targetMenuDropDownField = new DropdownField("Target Menu");
            var targetMenuProp = prop.FindPropertyRelative("targetMenu");
            targetMenuDropDownField.BindProperty(targetMenuProp);
            var options = MenuOptionsSingleton.Instance.GetOptions(ownerProp.objectReferenceValue as MenusManager);
            targetMenuDropDownField.choices = options != null ? options : new List<string>();
            targetMenuDropDownField.choices.Insert(0, "None");
            targetMenuDropDownField.style.marginTop = 5;
            targetMenuDropDownField.style.marginBottom = 5;
            root.Add(targetMenuDropDownField);

            Toggle openOverlayToggle = new Toggle("Open as Overlay");
            var overlayProp = prop.FindPropertyRelative("Overlay");
            openOverlayToggle.BindProperty(overlayProp);
            root.Add(openOverlayToggle);

            return root;
        }
    }
}