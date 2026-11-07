using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCDlgNode_New", menuName = "Data/FlameOfTheLost/Characters/NPC/Dialogue/NPCDialogueNodeData")]
    public class NPCDialogueNodeData : CharacterData
    {
        // --- Data / Configs ---
        //  1. Content
        [Header("Content")]
        [SerializeField] private string _speakerName;
        [SerializeField, TextArea(3, 10)] private string _dialogueText;
        public string SpeakerName => _speakerName;
        public string DialogueText => _dialogueText;

        //  2. Choices
        [Header("Choices")]
        [SerializeField] private List<NPCDialogueChoiceEntry> _choices = new();
        public IReadOnlyList<NPCDialogueChoiceEntry> Choices => _choices;
    }
}
