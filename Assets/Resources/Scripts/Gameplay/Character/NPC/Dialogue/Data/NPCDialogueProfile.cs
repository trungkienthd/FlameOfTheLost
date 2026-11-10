using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCDlg_New", menuName = "Data/FlameOfTheLost/Character/NPC/Dialogue/NPCDialogueProfile")]
    public class NPCDialogueProfile : CharacterData
    {
        [Header("Identity")]
        [SerializeField] private int _id;
        [SerializeField] private string _key;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(3, 6)] private string _description;
        public int Id => _id;
        public string Key => _key;
        public string DisplayName => _displayName;
        public string Description => _description;

        [Header("Dialogue Entries")]
        [SerializeField] private List<NPCDialogueEntry> _entries = new();

        public NPCDialogueNodeData SelectStartingNode(GameConditionController conditions)
        {
            NPCDialogueEntry selected = null;

            foreach (NPCDialogueEntry entry in _entries)
            {
                if (entry == null) continue;
                if (!entry.IsAvailable(conditions)) continue;

                if (selected == null || entry.Priority > selected.Priority)
                {
                    selected = entry;
                }
            }

            return selected?.StartingNode;
        }
    }
}
