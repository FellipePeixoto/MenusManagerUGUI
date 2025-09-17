#if UNITY_EDITOR
using Codice.Client.BaseCommands;
using System;
using System.Collections.Generic;
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
            root = new VisualElement();

            #region FIRST MENU SELECTABLE
            try
            {
                SerializedProperty firstSelectedProp = serializedObject.FindProperty("firstSelected");
                root.Add(new PropertyField(firstSelectedProp));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            #endregion

            #region KEEP ON BACKGROUND
            SerializedProperty keepOnBackgroundProp = serializedObject.FindProperty("keepOnBackground");
            root.Add(new PropertyField(keepOnBackgroundProp));
            #endregion

            #region DISPLAY METHOD
            try
            {
                var displayMethod = (MenuDisplayMethod)serializedObject.FindProperty("menuDisplayMethod").enumValueFlag;
                var container = new DisplayMetohdContainer(serializedObject);
                root.Add(container);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            #endregion

            #region EVENTS
            Foldout eventsFoldout = new Foldout();
            eventsFoldout.text = "Events";
            SerializedProperty onInitProp = serializedObject.FindProperty("onInit");
            SerializedProperty onBeforeShowProp = serializedObject.FindProperty("onBeforeShow");
            SerializedProperty onShowProp = serializedObject.FindProperty("onShow");
            SerializedProperty onBeforeHideProp = serializedObject.FindProperty("onBeforeHide");
            SerializedProperty onHideProp = serializedObject.FindProperty("onHide");
            eventsFoldout.Add(new PropertyField(onInitProp));
            eventsFoldout.Add(new PropertyField(onBeforeShowProp));
            eventsFoldout.Add(new PropertyField(onShowProp));
            eventsFoldout.Add(new PropertyField(onBeforeHideProp));
            eventsFoldout.Add(new PropertyField(onHideProp));
            root.Add(eventsFoldout);
            #endregion

            #region NAVIGATION
            SerializedProperty uiFlowsProp = serializedObject.FindProperty("navButtons");
            var parent = serializedObject.FindProperty("owner").objectReferenceValue;
            var options = MenuOptionsSingleton.Instance.GetOptions(parent as MenusManager);
            var navigationList = new ListView()
            {
                headerTitle = "Nav Buttons In Menu",
                showBoundCollectionSize = false,
                showFoldoutHeader = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            };
            navigationList.BindProperty(uiFlowsProp);
            root.Add(navigationList);
            #endregion

            return root;
        }

        class DisplayMetohdContainer: VisualElement
        {
            public static readonly string k_displayMethodField = "DisplayMethod";
            DropdownField displayMethodDropdownField;

            public static readonly string k_containerGameObjectState = "GameObjectMethod";
            GameObjectStateMethodContainer gameObjectContainer;

            public static readonly string k_containerCanvasGroupMethod = "CanvasGroupMethod";
            CanvasGroupMethodContainer canvasGroupMethodContainer;

            public static readonly string k_containerAnimatorMethod = "AnimatorMethod";
            AnimatorMethodContainer animatorMethodContainer;

            MenuDisplayMethod currentDisplayMethod;
            VisualElement currentMethodContainer;
            SerializedObject serializedObject;

            public DisplayMetohdContainer(SerializedObject obj) 
            {
                this.serializedObject = obj;
                MenuDisplayMethod displayMethod = (MenuDisplayMethod) obj.FindProperty("menuDisplayMethod").enumValueFlag;
                currentDisplayMethod = displayMethod;

                displayMethodDropdownField = new DropdownField("Display Method") { name = k_displayMethodField };
                Add(displayMethodDropdownField);
                
                var values = Enum.GetValues(typeof(MenuDisplayMethod)).Cast<MenuDisplayMethod>();
                var labels = values.Select(v => GetDescription(v)).ToList();
                displayMethodDropdownField.choices = labels;

                displayMethodDropdownField.index = (int)displayMethod;

                var displayMethodProp = obj.FindProperty("menuDisplayMethod");
                displayMethodDropdownField.RegisterValueChangedCallback(e =>
                {
                    displayMethodProp.enumValueIndex = displayMethodDropdownField.index;
                    obj.ApplyModifiedProperties();

                    HandleDisplayMethodChange((MenuDisplayMethod)displayMethodDropdownField.index);
                });

                currentMethodContainer = HandleDisplayMethodContainer(displayMethod);
                Add(currentMethodContainer);
            }

            void HandleDisplayMethodChange(MenuDisplayMethod displayMethod)
            {
                if (currentDisplayMethod == displayMethod)
                    return;

                if (currentMethodContainer != null)
                    Remove(currentMethodContainer);

                if (currentDisplayMethod == MenuDisplayMethod.Animator)
                {
                    (serializedObject.targetObject as Menu).HandleChangeToAnimatorMode(false);
                }

                if (displayMethod == MenuDisplayMethod.Animator)
                {
                    (serializedObject.targetObject as Menu).HandleChangeToAnimatorMode(true);
                }

                currentDisplayMethod = displayMethod;
                currentMethodContainer = HandleDisplayMethodContainer(displayMethod);
                Add(currentMethodContainer);
            }

            VisualElement HandleDisplayMethodContainer(MenuDisplayMethod displayMethod)
            {
                switch (displayMethod)
                {
                    case MenuDisplayMethod.State:
                        if (gameObjectContainer == null)
                            gameObjectContainer = new GameObjectStateMethodContainer(serializedObject);
                        
                        return gameObjectContainer;

                    case MenuDisplayMethod.CanvasGroup:
                        if (canvasGroupMethodContainer == null)
                            canvasGroupMethodContainer = new CanvasGroupMethodContainer(serializedObject);
                        
                        return canvasGroupMethodContainer;

                    case MenuDisplayMethod.Animator:
                        if (animatorMethodContainer == null)
                            animatorMethodContainer = new AnimatorMethodContainer(serializedObject);
                        
                        return animatorMethodContainer;
                }

                return null;
            }

            class GameObjectStateMethodContainer: VisualElement
            {
                public GameObjectStateMethodContainer(SerializedObject obj) { }
            }

            class CanvasGroupMethodContainer : VisualElement
            {
                public Toggle ScaledTimeToggle { get; private set; }
                public PropertyField FadeInGroup { get; private set; }
                public PropertyField FadeOutGroup { get; private set; }

                public CanvasGroupMethodContainer(SerializedObject obj)
                {
                    Add(new Label("Fade Options"));

                    SerializedProperty prop = obj.FindProperty("fades");

                    SerializedProperty scaledTimeProp = prop.FindPropertyRelative("UseScaledTime");
                    ScaledTimeToggle = new Toggle("Use Scaled Time");
                    ScaledTimeToggle.BindProperty(scaledTimeProp);
                    Add(ScaledTimeToggle);

                    SerializedProperty fadeInProp = prop.FindPropertyRelative("FadeIn");
                    Add(FadeInGroup = new PropertyField(fadeInProp));
                    
                    SerializedProperty fadeOutProp = prop.FindPropertyRelative("FadeOut");
                    Add(FadeOutGroup = new PropertyField(fadeOutProp));
                }
            }

            class AnimatorMethodContainer: VisualElement
            {
                Toggle ScaledTimeToggle;
                Toggle executeOnInitToggle;
                Button generateAnimatorButton;

                public AnimatorMethodContainer(SerializedObject obj)
                {
                    Add(new Label("Animator Method"));

                    SerializedProperty prop = obj.FindProperty("animatorSettings");

                    SerializedProperty scaledTimeProp = prop.FindPropertyRelative("UseScaledTime");
                    ScaledTimeToggle = new Toggle("Use Scaled Time");
                    ScaledTimeToggle.BindProperty(scaledTimeProp);
                    Add(ScaledTimeToggle);

                    executeOnInitToggle = new Toggle("Execute on Init");
                    executeOnInitToggle.BindProperty(prop.FindPropertyRelative("ExecuteAnimationOnInit"));
                    Add(executeOnInitToggle);

                    generateAnimatorButton = new Button() { text = "Create Animator Controller" };
                    generateAnimatorButton.RegisterCallback<ClickEvent>(e =>
                    {
                        var targetObject = obj.targetObject as Menu;
                        var animatorSettings = obj.FindProperty("animatorSettings");
                        
                        var created = AnimatorControllerTemplate();
                        if (created != null)
                        {
                            targetObject.Animator.runtimeAnimatorController = created;
                        }
                    });
                    Add(generateAnimatorButton);
                }

                AnimatorController AnimatorControllerTemplate()
                {
                    string animatorPath = EditorUtility.SaveFilePanelInProject(
                        "New animation controller",
                        "MenuAnimatorController",
                        "controller",
                        "Create a new animator controller"
                    );

                    if (string.IsNullOrEmpty(animatorPath))
                        return null;

                    AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(animatorPath);

                    controller.AddParameter(AnimatorSettings.VisibleParam, AnimatorControllerParameterType.Bool);

                    AnimationClip defaultClip = new AnimationClip() { name = AnimatorSettings.HiddenState, wrapMode = WrapMode.Once, hideFlags = HideFlags.None };
                    AssetDatabase.AddObjectToAsset(defaultClip, controller);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(defaultClip));
                    AnimatorState defaultStateHidden = controller.AddMotion(defaultClip);
                    controller.layers[0].stateMachine.AddState(defaultStateHidden, Vector3.one);
                    controller.layers[0].stateMachine.defaultState = defaultStateHidden;

                    AnimationClip visibleClip = new AnimationClip() { name = AnimatorSettings.VisibleState, wrapMode = WrapMode.Once, hideFlags = HideFlags.None };
                    AssetDatabase.AddObjectToAsset(visibleClip, controller);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(visibleClip));
                    AnimatorState visibleState = controller.AddMotion(visibleClip);
                    controller.layers[0].stateMachine.AddState(visibleState, Vector3.one + Vector3.right * 20);

                    AnimatorStateTransition hiddenToVisible = defaultStateHidden.AddTransition(visibleState);
                    hiddenToVisible.hasExitTime = false;
                    hiddenToVisible.exitTime = 0;
                    hiddenToVisible.duration = 0.1f;
                    hiddenToVisible.AddCondition(AnimatorConditionMode.If, 1, AnimatorSettings.VisibleParam);

                    AnimatorStateTransition visibleToExit = visibleState.AddExitTransition();
                    visibleToExit.hasExitTime = false;
                    visibleToExit.exitTime = 0;
                    visibleToExit.duration = 0.1f;
                    visibleToExit.AddCondition(AnimatorConditionMode.IfNot, 1, AnimatorSettings.VisibleParam);

                    return controller;
                }
            }
        }

        static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attr.Length > 0 ? attr[0].Description : value.ToString();
        }
    }
}
#endif