#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [CustomEditor(typeof(ButtonMenuNav))]
    public class ButtonMenuNavEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            ObjectField ownerField = new ObjectField("Respective Menu Owner");
            var ownerProp = serializedObject.FindProperty("owner");
            ownerField.value = ownerProp.objectReferenceValue;
            ownerField.SetEnabled(false);
            ownerField.style.marginTop = 5;
            ownerField.style.marginBottom = 5;
            root.Add(ownerField);

            DropdownField targetMenuDropDownField = new DropdownField("Target Menu");
            var targetMenuProp = serializedObject.FindProperty("targetMenu");
            targetMenuDropDownField.BindProperty(targetMenuProp);
            targetMenuDropDownField.choices = MenuOptionsSingleton.Instance.GetOptions(ownerProp.objectReferenceValue as MenusManager);
            targetMenuDropDownField.choices.Insert(0, "None");
            targetMenuDropDownField.style.marginTop = 5;
            targetMenuDropDownField.style.marginBottom = 5;
            root.Add(targetMenuDropDownField);

            return root;
        }
    }
}
#endif