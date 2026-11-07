using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "NPCDlg_New", menuName = "Data/FlameOfTheLost/Characters/NPC/Dialogue/NPCDialogueProfile")]
    public class NPCDialogueProfile : CharacterData
    {
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
