#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static Codice.Utils.Buffers.SizeBufferPool;
using static UnityEditor.Experimental.GraphView.GraphView;
using static DevPeixoto.UI.MenuManager.UGUI.ButtonMenuNav;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [AddComponentMenu("DevPeixoto/UI/Menu Manager/Menu")]
    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : MonoBehaviour
    {
        [SerializeField, HideInInspector] internal MenusManager owner;

        public GameObject firstSelected;

        [SerializeField] MenuDisplayMethod menuDisplayMethod;

        [SerializeField] AnimatorSettings animatorSettings = new AnimatorSettings();

        [SerializeField] Fades fades = new Fades();

        [SerializeField] List<NavContainer> navButtons = new List<NavContainer>();
        
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
        internal Animator Animator
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
        internal Transform backgroundParent;
        Coroutine fadeCoroutine;

#if UNITY_EDITOR
        RuntimeAnimatorController lastController;
#endif

        internal void Init(bool visible)
        {
            sibblingIndex = transform.GetSiblingIndex();

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    gameObject.SetActive(visible);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                    CanvasGroup.alpha = visible ? 1 : 0;
                    HandleCanvasGroupInteraction(visible);
                    break;

                case MenuDisplayMethod.Animator:
                    if (Animator == null)
                    {
                        Debug.LogError("No Animator Component");
                        break;
                    }
                    Animator.SetBool(AnimatorSettings.VisibleParam, visible);
                    if (!animatorSettings.ExecuteAnimationOnInit)
                    {
                        Animator.Play(
                        visible ? AnimatorSettings.VisibleState : AnimatorSettings.DefaultState,
                        0,
                        animatorSettings.ExecuteAnimationOnInit ? 0 : 1);
                    }
                    HandleCanvasGroupInteraction(visible);
                    break;
            }

            SetupNavButtons();
            var buttonBack = GetComponent<ButtonMenuBack>();
            if (buttonBack != null)
                buttonBack.owner = owner;

            onInit?.Invoke();
        }

        internal void Show(bool notify = true)
        {
            if (notify)
                onBeforeShow?.Invoke();

            HandleReturnToDefaultParent();

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    gameObject.SetActive(true);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                    if (fades.FadeIn.Duration > 0)
                    {
                        if (fadeCoroutine != null)
                            StopCoroutine(fadeCoroutine);
                        fadeCoroutine = StartCoroutine(CanvasGroupFade());
                    }
                    else
                    {
                        CanvasGroup.alpha = 1;
                        HandleCanvasGroupInteraction(true);
                    }
                    break;

                case MenuDisplayMethod.Animator:
                    if (Animator == null)
                    {
                        Debug.LogError("No Animator Component");
                        break;
                    }
                    Animator.SetBool(AnimatorSettings.VisibleParam, true);
                    HandleCanvasGroupInteraction(true);
                    break;
            }

            EventSystem.current.SetSelectedGameObject(firstSelected);

            if (notify)
                onShow?.Invoke();
        }

        internal void Hide(bool notify = true)
        {
            if (notify)
                onBeforeHide?.Invoke();

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    gameObject.SetActive(false);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                    if (fades.FadeOut.Duration > 0)
                    {
                        if (fadeCoroutine != null)
                            StopCoroutine(fadeCoroutine);
                        fadeCoroutine = StartCoroutine(CanvasGroupFadeOut());
                    }
                    else
                    {
                        CanvasGroup.alpha = 0;
                        HandleCanvasGroupInteraction(false);
                    }
                    break;

                case MenuDisplayMethod.Animator:
                    if (Animator == null)
                    {
                        Debug.LogError("No Animator Component");
                        break;
                    }
                    Animator.SetBool(AnimatorSettings.VisibleParam, false);
                    HandleCanvasGroupInteraction(false);
                    break;
            }

            if (notify)
                onHide?.Invoke();
        }

        void HandleReturnToDefaultParent()
        {
            if (transform.parent == backgroundParent)
            {
                transform.SetParent(owner.transform);
                transform.SetSiblingIndex(sibblingIndex);
            }
        }

        IEnumerator CanvasGroupFade()
        {
            Fades.Fade fade = fades.FadeOut;

            HandleCanvasGroupInteraction(true);

            if (animatorSettings.UseScaledTime)
                yield return new WaitForSeconds(fade.Delay);
            else
                yield return new WaitForSecondsRealtime(fade.Delay);

            float timer = 0;

            if (fades.UseScaledTime)
            {
                while (timer < fade.Duration)
                {
                    timer += Time.deltaTime;
                    CanvasGroup.alpha = fade.Curve.Evaluate(Mathf.Abs(timer / fade.Duration));
                    yield return null;
                }
            }
            else
            {
                while (timer < fade.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    CanvasGroup.alpha = fade.Curve.Evaluate(Mathf.Abs(timer / fade.Duration));
                    yield return null;
                }
            }

            CanvasGroup.alpha = 1;

            fadeCoroutine = null;
        }

        IEnumerator CanvasGroupFadeOut()
        {
            Fades.Fade fadeInfo = fades.FadeOut;

            HandleCanvasGroupInteraction(false);

            if (animatorSettings.UseScaledTime)
                yield return new WaitForSeconds(fadeInfo.Delay);
            else
                yield return new WaitForSecondsRealtime(fadeInfo.Delay);

            float timer = fadeInfo.Duration;

            if (fades.UseScaledTime)
            {
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    CanvasGroup.alpha = fadeInfo.Curve.Evaluate(timer / fadeInfo.Duration);
                    yield return null;
                }
            }
            else
            {
                while (timer > 0)
                {
                    timer -= Time.unscaledDeltaTime;
                    CanvasGroup.alpha = fadeInfo.Curve.Evaluate(timer / fadeInfo.Duration);
                    yield return null;
                }
            }

            CanvasGroup.alpha = 0;

            fadeCoroutine = null;
        }

        internal void HandleCanvasGroupInteraction(bool interactive)
        {
            CanvasGroup.blocksRaycasts = interactive;
            CanvasGroup.interactable = interactive;
        }

        void SetupNavButtons()
        {
            if (this == null)
                return;

            navButtons = GetComponentsInChildren<ButtonMenuNav>(true).ToList().Select(x => x.navContainer).ToList();
            foreach (var button in navButtons)
                button.owner = owner;
        }

#if UNITY_EDITOR
        private void DelayCall()
        {
            if (this == null)
                return;

            SetupNavButtons();
        }

        void HierarchyChanged()
        {
            if (this == null)
                return;

            SetupNavButtons();
        }

        internal void HandleChangeToAnimatorMode(bool animatorMode)
        {
            if (animatorMode)
            {
                if (Animator == null)
                {
                    gameObject.AddComponent<Animator>();
                    Animator.runtimeAnimatorController = lastController;
                }
            }
            else
            {
                if (Animator != null)
                {
                    lastController = Animator.runtimeAnimatorController;
                    DestroyImmediate(Animator);
                }
            }
        }

        [InitializeOnLoadMethod]
        static void OnUnityReload()
        {
            var allMenus = FindObjectsByType<Menu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var item in allMenus)
            {
                EditorApplication.delayCall += item.DelayCall;
                EditorApplication.hierarchyChanged += item.HierarchyChanged;
            }
        }

        private void OnValidate()
        {
            SetupNavButtons();
        }

        private void Reset()
        {
            SetupNavButtons();
        }
#endif
    }

    [System.Serializable]
    public class Fades
    {
        public bool UseScaledTime;
        public Fade FadeIn;
        public  Fade FadeOut;

        [System.Serializable]
        public class Fade
        {
            public float Delay;
            public float Duration = 0f;
            public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
        }
    }    

    [System.Serializable]
    public class AnimatorSettings
    {
        public bool ExecuteAnimationOnInit;
        public bool UseScaledTime;
        public static readonly string DefaultState = "MenuHidden";
        public static readonly string HiddenState = "MenuHidden";
        public static readonly string VisibleState = "MenuVisible";
        public static readonly string VisibleParam = "visible";
    }

    public enum MenuDisplayMethod
    {
        [Description("Game Object State")]
        State = 0,
        [Description("Control with Canvas Group")]
        CanvasGroup = 1,
        [Description("Contro with Animator")]
        Animator = 2,
    }
}