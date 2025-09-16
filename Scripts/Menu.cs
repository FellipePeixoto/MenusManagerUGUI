using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [AddComponentMenu("DevPeixoto/UI/Menu Manager/Menu")]
    [RequireComponent(typeof(CanvasGroup), typeof(Animator))]
    public class Menu : MonoBehaviour
    {
        [Header("Event System")]
        public GameObject firstSelected;
        [SerializeField] bool keepOnBackground = false;
        [SerializeField] bool scaledTime = false;
        [SerializeField] AnimatorSettings animatorSettings = new AnimatorSettings();
        [SerializeField] MenuDisplayMethod menuDisplayMethod;
        [SerializeField] Fade fadeIn;
        [SerializeField] Fade fadeOut;
        [SerializeField] List<UiFlow> uiFlows = new List<UiFlow>();
        [SerializeReference, HideInInspector] internal MenusManager parentMenusManager;
        public UnityEvent onInit;
        public UnityEvent onBeforeShow;
        public UnityEvent onShow;
        public UnityEvent onBeforeHide;
        public UnityEvent onHide;

        CanvasGroup canvasGroup;
        CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null)
                {
                    canvasGroup = GetComponent<CanvasGroup>();
                }

                return canvasGroup;
            }
        }

        Animator animator;
        public Animator Animator
        {
            get
            {
                if (animator == null)
                {
                    animator = GetComponent<Animator>();
                }

                return animator;
            }
        }

        int sibblingIndex;
        Coroutine fadeCoroutine;

        public void Init(bool visible)
        {
            sibblingIndex = transform.GetSiblingIndex();

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    gameObject.SetActive(visible);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                    CanvasGroup.alpha = visible ? 1 : 0;
                    CanvasGroup.blocksRaycasts = visible;
                    CanvasGroup.interactable = visible;
                    break;

                case MenuDisplayMethod.Animator:
                    Animator.Play(
                        visible ? animatorSettings.VisibleState : animatorSettings.DefaultState,
                        0,
                        animatorSettings.ExecuteAnimationOnInit ? 0 : 1);
                    Animator.SetBool(animatorSettings.VisibleParam, visible);
                    HandleCanvasGroupOnAnimatorMode(visible);
                    break;
            }

            SetFlows();
            var buttonBack = GetComponent<ButtonMenuBack>();
            if (buttonBack != null)
                buttonBack.RegisterUniqueCallback(Back);

            onInit?.Invoke();
        }

        public void Show(bool notify = true, Transform parent = null)
        {
            if (notify)
                onBeforeShow?.Invoke();

            if (keepOnBackground)
            {
                transform.SetParent(parent);
                transform.SetSiblingIndex(sibblingIndex);
            }

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    gameObject.SetActive(true);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                    if (fadeIn.Duration > 0)
                    {
                        if (fadeCoroutine != null)
                            StopCoroutine(fadeCoroutine);
                        fadeCoroutine = StartCoroutine(CanvasGroupFade(fadeIn));
                    }
                    else
                    {
                        CanvasGroup.alpha = 1;
                        CanvasGroup.interactable = true;
                        CanvasGroup.blocksRaycasts = true;
                    }
                    break;

                case MenuDisplayMethod.Animator:
                    Animator.SetBool(animatorSettings.VisibleParam, true);
                    HandleCanvasGroupOnAnimatorMode(true);
                    break;
            }

            EventSystem.current.SetSelectedGameObject(firstSelected);

            if (notify)
                onShow?.Invoke();
        }

        public void Hide(bool notify = true, Transform parent = null)
        {
            if (notify)
                onBeforeHide?.Invoke();

            if (keepOnBackground)
                transform.SetParent(parent);

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    if (!keepOnBackground)
                        gameObject.SetActive(false);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                    if (!keepOnBackground)
                    {
                        if (fadeOut.Duration > 0)
                        {
                            if (fadeCoroutine != null)
                                StopCoroutine(fadeCoroutine);
                            fadeCoroutine = StartCoroutine(CanvasGroupFade(fadeOut, false));
                        }
                        else
                        {
                            CanvasGroup.alpha = 0;
                        }
                    }
                    CanvasGroup.interactable = false;
                    CanvasGroup.blocksRaycasts = false;
                    break;

                case MenuDisplayMethod.Animator:
                    Animator.SetBool(animatorSettings.VisibleParam, false);
                    HandleCanvasGroupOnAnimatorMode(false);
                    break;
            }

            if (notify)
                onHide?.Invoke();
        }

        public void Back()
        {
            if (parentMenusManager != null)
                parentMenusManager.Back();
        }

        IEnumerator CanvasGroupFade(Fade fade, bool fadeIn = true)
        {
            if (!fadeIn)
            {
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            }

            if (scaledTime)
                yield return new WaitForSeconds(fade.Delay);
            else
                yield return new WaitForSecondsRealtime(fade.Delay);

            float timer = 0;
            float evaluator = fadeIn ? 0 : 1;

            if (scaledTime)
            {
                while (timer < fade.Duration)
                {
                    timer += Time.deltaTime;
                    CanvasGroup.alpha = fade.Curve.Evaluate(Mathf.Abs((timer / fade.Duration) - evaluator));
                    yield return null;
                }
            }
            else
            {
                while (timer < fade.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    CanvasGroup.alpha = fade.Curve.Evaluate(Mathf.Abs((timer / fade.Duration) - evaluator));
                    yield return null;
                }
            }

            if (fadeIn)
            {
                CanvasGroup.alpha = 1;
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
            }

            fadeCoroutine = null;
        }

        void HandleCanvasGroupOnAnimatorMode(bool visible)
        {
            CanvasGroup.blocksRaycasts = visible;
            CanvasGroup.interactable = visible;
        }

        void SetFlows()
        {
            var allButtons = GetComponentsInChildren<ButtonMenuNav>(true);
            uiFlows.RemoveAll(x => !allButtons.Contains(x.Button));
            var nonSetButtons = allButtons.Where(x => !uiFlows.Select(y => y.Button).Contains(x)).ToList();
            foreach (var button in nonSetButtons)
            {
                var uiFlow = new UiFlow()
                {
                    Button = button,
                    targetMenuName = "None"
                };

                uiFlows.Add(uiFlow);

                uiFlow.Button.RegisterUniqueCallback(() =>
                {
                    if (parentMenusManager == null)
                    {
                        Debug.LogError("This Menu doens't have any Manager associated. Did you set the manager to manually set the the menus?");
                        return;
                    }

                    parentMenusManager.SwitchTo(uiFlow.targetMenuName);
                });
            }
        }

#if UNITY_EDITOR
        private void DelayCall()
        {
            if (this == null)
                return;

            SetFlows();
        }

        [InitializeOnLoadMethod]
        static void OnUnityReload()
        {
            var allMenus = FindObjectsByType<Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var item in allMenus)
                EditorApplication.delayCall += item.DelayCall;
        }

        private void OnValidate()
        {
            SetFlows();
        }

        private void Reset()
        {
            SetFlows();
        }
#endif
    }

    [System.Serializable]
    public class Fade
    {
        public float Delay;
        public float Duration = 0f;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    }

    [System.Serializable]
    public class UiFlow
    {
        public ButtonMenuNav Button;
        public string targetMenuName;
    }

    [System.Serializable]
    public class AnimatorSettings
    {
        public bool ExecuteAnimationOnInit;
        [HideInInspector] public string DefaultState = "MenuHidden";
        [HideInInspector] public string HiddenState = "MenuHidden";
        [HideInInspector] public string VisibleState = "MenuVisible";
        [HideInInspector] public string VisibleParam = "visible";
    }

    public enum MenuDisplayMethod
    {
        [Description("Game Object State")]
        State = 0,
        [Description("Control with Canvas Group")]
        CanvasGroup = 1,
        [Description("Contro with Animator")]
        Animator = 3,
    }
}