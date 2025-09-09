using System;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [CustomEditor(typeof(Menu))]
    [CanEditMultipleObjects]
    public class MenuEditor : Editor
    {
        VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            root = Resources.Load<VisualTreeAsset>("XML/MenuEditor").CloneTree();

            VisualElement topOptions = root.Q<VisualElement>("TopOptions");
            SerializedProperty firstSelectedProp = serializedObject.FindProperty("firstSelected");
            topOptions.Add(new PropertyField(firstSelectedProp));
            SerializedProperty keepOnBackgroundProp = serializedObject.FindProperty("keepOnBackground");
            topOptions.Add(new PropertyField(keepOnBackgroundProp));

            new XmlComponents(root, (Menu)target, serializedObject);

            Foldout eventsFoldout = new Foldout();
            eventsFoldout.text = "Events";
            SerializedProperty onInitProp = serializedObject.FindProperty("onInit");
            SerializedProperty onShowProp = serializedObject.FindProperty("onShow");
            SerializedProperty onHideProp = serializedObject.FindProperty("onHide");
            eventsFoldout.Add(new PropertyField(onInitProp));
            eventsFoldout.Add(new PropertyField(onShowProp));
            eventsFoldout.Add(new PropertyField(onHideProp));
            root.Add(eventsFoldout);

            ((Menu)target).LoadUiFlow();
            SerializedProperty uiFlowsProp = serializedObject.FindProperty("uiFlows");
            for (int i = 0; i < uiFlowsProp.arraySize; i++)
            {
                SerializedProperty uiFlowProp = uiFlowsProp.GetArrayElementAtIndex(i);
                var visualElement = Resources.Load<VisualTreeAsset>("XML/UIFlow").CloneTree();

                var buttonNameLabel = visualElement.Q<Label>("ButtonNameLabel");
                SerializedProperty buttonProp = uiFlowProp.FindPropertyRelative("Button");
                buttonNameLabel.text = buttonProp.objectReferenceValue.name;

                var isBackButtonToggle = visualElement.Q<Toggle>("IsBackButton");
                SerializedProperty isBackButtonProp = uiFlowProp.FindPropertyRelative("IsBackButton");
                isBackButtonToggle.value = isBackButtonProp.boolValue;
                var groupGoTo = visualElement.Q<VisualElement>("GroupGoTo");
                groupGoTo.style.display = isBackButtonToggle.value ? DisplayStyle.None : DisplayStyle.Flex;
                isBackButtonToggle.RegisterValueChangedCallback(evnt =>
                {
                    isBackButtonProp.boolValue = evnt.newValue;
                    serializedObject.ApplyModifiedProperties();

                    if (evnt.newValue)
                    {
                        groupGoTo.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        groupGoTo.style.display = DisplayStyle.Flex;
                    }
                });

                var objectField = visualElement.Q<ObjectField>("TargetMenu");
                SerializedProperty objectFieldProp = uiFlowProp.FindPropertyRelative("GoTo");
                objectField.labelElement.style.display = DisplayStyle.None;
                objectField.objectType = typeof(Menu);
                objectField.value = objectFieldProp.objectReferenceValue;
                objectField.BindProperty(objectFieldProp);

                root.Add(visualElement);
            }

            serializedObject.ApplyModifiedProperties();
            return root;
        }
    }

    public class XmlComponents
    {
        public XmlComponent<DropdownField> DisplayMethodContainer;
        public Box FadeSection;
        public Box AnimatorSection;

        public XmlComponents(VisualElement root, Menu target, SerializedObject serializedObject)
        {
            FadeSection = new Box();
            FadeSection.Add(new Label("Fade Options"));
            SerializedProperty scaledTimeProp = serializedObject.FindProperty("scaledTime");
            SerializedProperty fadeInProp = serializedObject.FindProperty("fadeIn");
            SerializedProperty fadeOutProp = serializedObject.FindProperty("fadeOut");
            FadeSection.Add(new PropertyField(scaledTimeProp));
            FadeSection.Add(new PropertyField(fadeInProp));
            FadeSection.Add(new PropertyField(fadeOutProp));
            root.Add(FadeSection);

            AnimatorSection = new Box();
            if (target.Animator.runtimeAnimatorController == null)
            {
                root.Add(new Button() { text = "Generate Animator Controller" });
            }
            root.Add(AnimatorSection);

            var values = Enum.GetValues(typeof(MenuDisplayMethod)).Cast<MenuDisplayMethod>();
            var labels = values.Select(v => GetDescription(v)).ToList();
            DisplayMethodContainer = new XmlComponent<DropdownField>(root, "DisplayMethodDropdown");
            DisplayMethodContainer.Component.choices = labels;
            SerializedProperty menuDisplayMethodProp = serializedObject.FindProperty("menuDisplayMethod");
            DisplayMethodContainer.Component.index = menuDisplayMethodProp.intValue;
            HandleDisplayMethodSection((MenuDisplayMethod)DisplayMethodContainer.Component.index);
            DisplayMethodContainer.Component.RegisterValueChangedCallback(evnt =>
            {
                menuDisplayMethodProp.enumValueIndex = DisplayMethodContainer.Component.index;
                serializedObject.ApplyModifiedProperties();

                HandleDisplayMethodSection((MenuDisplayMethod)DisplayMethodContainer.Component.index);
            });
        }

        void HandleDisplayMethodSection(MenuDisplayMethod displayMethod)
        {
            FadeSection.style.display = DisplayStyle.None;
            AnimatorSection.style.display = DisplayStyle.None;

            switch (displayMethod)
            {
                case MenuDisplayMethod.Fade:
                    FadeSection.style.display = DisplayStyle.Flex;
                    break;

                case MenuDisplayMethod.Animator:
                    AnimatorSection.style.display = DisplayStyle.Flex;
                    break;
            }
        }

        AnimatorController CreateAnimatorController(AnimatorSettings animatorSettings)
        {
            string animatorPath = EditorUtility.SaveFilePanelInProject(
                "New animation controller",
                "Menu Animator Controller",
                "controller",
                "Create a new animator controller"
            );

            if (string.IsNullOrEmpty(animatorPath))
                return null;

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(animatorPath);

            controller.AddParameter(animatorSettings.VisibleParam, AnimatorControllerParameterType.Bool);

            AnimatorState defaultState = new AnimatorState() { name = animatorSettings.HiddenState };
            controller.layers[0].stateMachine.AddState(defaultState, Vector3.zero);
            controller.layers[0].stateMachine.defaultState = defaultState;

            AnimatorState visibleState = new AnimatorState() { name = animatorSettings.VisibleState };
            controller.layers[0].stateMachine.AddState(animatorSettings.VisibleState, Vector3.right * 5);

            return null;
        }

        string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attr.Length > 0 ? attr[0].Description : value.ToString();
        }
    }

    public class XmlComponent<T> where T : VisualElement
    {
        string id;
        public T Component;

        public XmlComponent(VisualElement root, string id)
        {
            this.id = id;
            Component = root.Q<T>(id);
        }
    }
}