using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Button", 30)]
    public class Button : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected Button() { }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
            // After pressing, make sure it goes to the pressed state.
            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(ResetStateAfterClick());
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(ResetStateAfterClick());
        }

        private IEnumerator ResetStateAfterClick()
        {
            // Short delay to wait for the pressed state to complete.
            yield return new WaitForSeconds(0.1f);

            // After the delay, transition to either highlighted or normal state based on whether the button is being hovered over.
            if (IsPointerInside())
            {
                DoStateTransition(SelectionState.Highlighted, false); // Highlighted if the button is still being hovered over.
            }
            else
            {
                DoStateTransition(SelectionState.Normal, false); // Otherwise, return to normal.
            }
        }

        private bool IsPointerInside()
        {
            return EventSystem.current != null &&
                   EventSystem.current.IsPointerOverGameObject() &&
                   EventSystem.current.currentSelectedGameObject == gameObject;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            // When the pointer enters the button, ensure it transitions to highlighted state.
            DoStateTransition(SelectionState.Highlighted, false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            // When the pointer exits the button, ensure it transitions to normal state.
            DoStateTransition(SelectionState.Normal, false);
        }
    }
}
