using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_NPC_New", menuName = "Data/FlameOfTheLost/Character/NPC/NPCProfile")]
    public class NPCProfile : CharacterProfile
    {
        // --- Data / Configs ---
        //  1. Hostility
        [Header("Hostility")]
        [SerializeField]
        private NPCHostilityResponseType _hostilityResponseType = NPCHostilityResponseType.None;
        public NPCHostilityResponseType HostilityResponseType => _hostilityResponseType;
    }
}