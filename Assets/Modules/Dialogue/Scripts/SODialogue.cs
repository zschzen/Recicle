using UnityEngine;

namespace Modules.Dialogue
{
    [CreateAssetMenu(fileName = "SO Dialogue", menuName = "Screen/Dialogue", order = 0)]
    public class SODialogue : SharedEvent.SharedEvent
    {
        [System.Serializable]
        public struct DialogueData
        {
            [field: SerializeField]
            [field: Tooltip("The name of the dialogue.\n Leave empty to avoid updating the name.")]
            public string Title { get; private set; }

            [field: SerializeField]
            [field: TextArea(3, 10)]
            public string Content { get; private set; }

            [field: SerializeField]
            [field: Tooltip("Color of the background of the dialogue box.\nLeave empty (aka Color.Clear) for default component color.")]
            public Color BackgroundColor { get; private set; }
        }

        [field: SerializeField] public bool FreezeTime { get; private set; }
        [field: SerializeField] public DialogueData[] Dialogues { get; private set; }
    }
}