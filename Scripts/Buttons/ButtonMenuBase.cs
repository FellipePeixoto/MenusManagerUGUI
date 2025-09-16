using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DevPeixoto.UI.MenuManager.UGUI
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonMenuBase : MonoBehaviour
    {
        Button button;
        UnityAction action;

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

        public virtual void RegisterUniqueCallback(UnityAction action)
        {
            if (action == null)
                return;

            if (this.action == null)
            {
                this.action = action;
                Button.onClick.AddListener(this.action);
            }

            if (this.action != action)
            {
                Button.onClick.RemoveListener(this.action);
                this.action = action;
                Button.onClick.AddListener(this.action);
            }
        }

        private void OnEnable()
        {
            if (!IsActionRegistered(action))
             Button.onClick.AddListener(action);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(action);
        }

        bool IsActionRegistered(UnityAction action)
        {
            for (int i = 0; i < Button.onClick.GetPersistentEventCount(); i++)
            {
                if ((UnityEngine.Object)Button.onClick.GetPersistentTarget(i) == (UnityEngine.Object)action.Target &&
                    Button.onClick.GetPersistentMethodName(i) == action.Method.Name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}