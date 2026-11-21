using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCDlgNode_New", menuName = "Data/FlameOfTheLost/Character/NPC/Dialogue/NPCDialogueNodeData")]
    public class NPCDialogueNodeData : CharacterData
    {
        // --- Data / Configs ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(3, 6)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        //  2. Content
        [Header("Content")]
        [SerializeField] private string _speakerName;
        [SerializeField, TextArea(3, 10)] private string _dialogueText;
        public string SpeakerName => _speakerName;
        public string DialogueText => _dialogueText;

        //  3. Choices
        [Header("Choices")]
        [SerializeField] private List<NPCDialogueChoiceEntry> _choices = new();
        public IReadOnlyList<NPCDialogueChoiceEntry> Choices => _choices;
    }
}
