using System;
using DG.Tweening;
using UnityEngine;

namespace Modules.UIScreen
{
    //[RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        // Properties -----------------------------------

        [field: SerializeField] public bool IsShow { get; private set; } = false;
        [field: SerializeField] public string Name { get; private set; } = "Screen";

        // Fields ---------------------------------------

        [SerializeField] private CanvasGroup m_CanvasGroup;

        // Public Methods ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Show the screen
        /// </summary>
        /// <param name="freeze">Should the screen freeze the game?</param>
        public virtual Tween Show(bool freeze = false)
        {
            if (IsShow) return default;
            IsShow = true;

            // Freeze the game
            if (freeze) Time.timeScale = 0;

            // Set game object active
            gameObject.SetActive(true);

            // Fade in
            return m_CanvasGroup.DOFade(1, 0.5f).OnComplete(() => SetCanvasInteractable(true));
        }

        /// <summary>
        /// Hide the screen
        /// </summary>
        /// <param name="unfreeze">Should unfreeze the game?</param>
        public virtual Tween Hide(bool unfreeze = false)
        {
            if (!IsShow) return default;
            IsShow = false;

            // Unfreeze the game
            if (unfreeze) Time.timeScale = 1;

            // Set interactable to false
            SetCanvasInteractable(false);

            // Fade out
            return m_CanvasGroup.DOFade(0, 0.5f).OnComplete(() => gameObject.SetActive(false));
        }

        // Unity Methods ----------------------------------------------------------------------------------------------

        protected virtual void OnValidate()
        {
            m_CanvasGroup ??= GetComponentInChildren<CanvasGroup>();

            // Assert if the canvas group is null
            Debug.Assert(m_CanvasGroup != null, "No CanvasGroup found in children", gameObject);
        }

        // Private Methods ---------------------------------------------------------------------------------------------

        private void SetCanvasInteractable(bool interactable)
        {
            m_CanvasGroup.blocksRaycasts = interactable;
            m_CanvasGroup.interactable = interactable;
        }
    }
}