using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CustomExtensions.TextMeshProExtensions;

namespace Modules.Dialogue
{
    public class Dialogue : Modules.UIScreen.UIScreen
    {
        // Properties -----------------------------------
        [field: Space(20F), SerializeField] public SODialogue DialogueData { get; private set; }

        // Fields -------------------------------
        [Space] [SerializeField] private bool m_showOnStart = false;

        [Space] [SerializeField] private Graphic m_background;
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_content;
        [SerializeField] private Button m_nextButton;

        private int m_currentDialogueIndex = 0;
        private SODialogue.DialogueData m_currentDialogue;

        private bool bHasNextDialogue => m_currentDialogueIndex < DialogueData.Dialogues.Length;

        // Public Methods ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets the dialogue data.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(SODialogue data)
        {
            DialogueData = data;

            // Update general data
            SetCurrentDialogueData();
        }

        // Unity Methods ----------------------------------------------------------------------------------------------

        private void OnEnable() => m_nextButton.onClick.AddListener(OnNextButtonPressed);

        private void OnDisable() => m_nextButton.onClick.RemoveListener(OnNextButtonPressed);

        private void Start()
        {
            SetData(DialogueData);

            // First immediately hide, then show, if needed
            _ = Hide(true, 0F)
                // Check if we should show on start
                .OnComplete(() =>
                {
                    if (m_showOnStart) Show();
                });
        }

        // Private Methods ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets the current dialogue data.
        /// </summary>
        private void SetCurrentDialogueData()
        {
            if (DialogueData == null) return;
            if (!bHasNextDialogue) return;

            var current = DialogueData.Dialogues[m_currentDialogueIndex];

            m_currentDialogue = DialogueData.Dialogues[m_currentDialogueIndex];

            // Update title
            if (m_currentDialogueIndex == 0 // Always update if its the first dialogue
                || !string.IsNullOrEmpty(m_currentDialogue.Title) // Avoid updating the title if its the same or empty
                && !m_currentDialogue.Title.Equals(m_title.text))
            {
                _ = m_title.DOText(m_currentDialogue.Title, .25F);
            }

            // Update content
            _ = m_content.DOText(m_currentDialogue.Content, .25F);

            // Update background color
            // Check if the new background color is different from the current one and if it's not clear
            if (current.BackgroundColor.Equals(m_currentDialogue.BackgroundColor)) return;
            if (current.BackgroundColor.Equals(default)) return;

            m_background.material.DOColor(current.BackgroundColor, "_Color", 0.5F);
        }

        /// <summary>
        /// Called when the next button is pressed.
        /// </summary>
        private void OnNextButtonPressed()
        {
            // Increment the dialogue index and update the current dialogue data.
            m_currentDialogueIndex++;
            SetCurrentDialogueData();

            // Check if we have reached the end of the dialogue.
            if (bHasNextDialogue) return;

            // Hides the dialogue screen.
            Hide(DialogueData.FreezeTime);
        }
    }
}