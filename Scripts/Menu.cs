using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [System.Serializable]
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
        public MenusManager parentMenusManager;
        public UnityEvent onInit;
        public UnityEvent onShow;
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

        public void Init(MenusManager respectiveMenusManager)
        {
            sibblingIndex = transform.GetSiblingIndex();

            switch (menuDisplayMethod)
            {
                case MenuDisplayMethod.State:
                    gameObject.SetActive(false);
                    break;

                case MenuDisplayMethod.CanvasGroup:
                case MenuDisplayMethod.Fade:
                    CanvasGroup.alpha = 0;
                    CanvasGroup.blocksRaycasts = false;
                    CanvasGroup.interactable = false;
                    break;

                case MenuDisplayMethod.Animator:
                    Animator.Play(animatorSettings.DefaultState);
                    Animator.SetBool(animatorSettings.VisibleParam, false);
                    HandleCanvasGroupOnAnimatorMode(false);
                    break;
            }

            parentMenusManager = respectiveMenusManager;
            SetFlows();
            onInit?.Invoke();
        }

        public void Show(bool notify = true, Transform parent = null)
        {
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
                    CanvasGroup.alpha = 1;
                    CanvasGroup.interactable = true;
                    CanvasGroup.blocksRaycasts = true;
                    break;

                case MenuDisplayMethod.Fade:
                    StartCoroutine(CanvasGroupFadeIn());
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
                        CanvasGroup.alpha = 0;
                    }
                    CanvasGroup.interactable = false;
                    CanvasGroup.blocksRaycasts = false;
                    break;

                case MenuDisplayMethod.Fade:
                    if (!keepOnBackground)
                        StartCoroutine(CanvasGroupFadeOut());
                    break;

                case MenuDisplayMethod.Animator:
                    Animator.SetBool(animatorSettings.VisibleParam, false);
                    HandleCanvasGroupOnAnimatorMode(false);
                    break;
            }

            if (notify)
                onHide?.Invoke();
        }

        IEnumerator CanvasGroupFadeIn()
        {
            if (scaledTime)
                yield return new WaitForSeconds(fadeIn.Delay);
            else
                yield return new WaitForSecondsRealtime(fadeIn.Delay);

            float timer = 0;
            CanvasGroup.alpha = 0;

            if (scaledTime)
            {
                while (timer < fadeIn.Duration)
                {
                    timer += Time.deltaTime;
                    CanvasGroup.alpha = fadeIn.Curve.Evaluate(timer / fadeIn.Duration);
                    yield return null;
                }
            }
            else
            {
                while (timer < fadeIn.Duration)
                {
                    timer += Time.unscaledDeltaTime;
                    CanvasGroup.alpha = fadeIn.Curve.Evaluate(timer / fadeIn.Duration);
                    yield return null;
                }
            }

            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        IEnumerator CanvasGroupFadeOut()
        {
            if (scaledTime)
                yield return new WaitForSeconds(fadeOut.Delay);
            else
                yield return new WaitForSecondsRealtime(fadeOut.Delay);

            float timer = fadeOut.Duration;
            CanvasGroup.alpha = 1;

            if (scaledTime)
            {
                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    CanvasGroup.alpha = fadeOut.Curve.Evaluate(timer / fadeOut.Duration);
                    yield return null;
                }
            }
            else
            {
                while (timer > 0)
                {
                    timer -= Time.unscaledDeltaTime;
                    CanvasGroup.alpha = fadeOut.Curve.Evaluate(timer / fadeOut.Duration);
                    yield return null;
                }
            }

            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void LoadUiFlow()
        {
            var allButtons = GetComponentsInChildren<Button>();

            if (uiFlows == null)
            {
                uiFlows = new();
                foreach (var button in allButtons)
                {
                    uiFlows.Add(new UiFlow() { Button = button, GoTo = null });
                }
            }
            else
            {
                var nonSetButtons = allButtons.Where(x => !uiFlows.Select(y => y.Button).Contains(x)).ToList();
                foreach (var button in nonSetButtons)
                {
                    uiFlows.Add(new UiFlow()
                    {
                        Button = button,
                        GoTo = null
                    });
                }
            }
        }

        void HandleCanvasGroupOnAnimatorMode(bool visible)
        {
            CanvasGroup.blocksRaycasts = visible;
            CanvasGroup.interactable = visible;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetFlows();
        }

        void SetFlows()
        {
            foreach (var uiflow in uiFlows)
            {

                uiflow.Button.onClick.RemoveAllListeners();
                uiflow.Button.onClick.AddListener(() =>
                {
                    if (parentMenusManager == null)
                    {
                        Debug.LogError("This Menu doens't have any Manager associated. Did you set the manager to manually set the the menus?");
                        return;
                    }

                    if (uiflow.IsBackButton)
                        parentMenusManager.Back();
                    else if (uiflow.GoTo != null)
                        parentMenusManager.SwitchTo(uiflow.GoTo);
                });
            }
        }
#endif
    }

    [System.Serializable]
    public class Fade
    {
        public float Delay;
        public float Duration = 0.35f;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    }

    [System.Serializable]
    public class UiFlow
    {
        public bool IsBackButton;
        public Button Button;
        public Menu GoTo;
    }

    [System.Serializable]
    public class AnimatorSettings
    {
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
        [Description("Fade In and Out")]
        Fade = 2,
        [Description("Contro with Animator")]
        Animator = 3,
    }
}