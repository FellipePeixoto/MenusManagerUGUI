using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonMenuBase : MonoBehaviour
    {
        Button button;

        protected Button Button
        {
            get
            {
                if (button == null)
                {
                    button = GetComponent<Button>();
                }

                return button;
            }
        }

        protected virtual void OnEnable()
        {
            Button.onClick.AddListener(NavigateToDestiny);
        }

        protected virtual void OnDisable()
        {
            Button.onClick.RemoveListener(NavigateToDestiny);
        }

        protected abstract void NavigateToDestiny();
    }
}