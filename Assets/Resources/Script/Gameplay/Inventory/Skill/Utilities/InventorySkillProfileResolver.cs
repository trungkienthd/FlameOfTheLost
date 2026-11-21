using UnityEngine;

namespace FlameOfTheLost
{
    public static class InventorySkillProfileResolver
    {
        // --- Public APIs ---
        public static SkillProfile GetById(int id)
        {
            if (id < 0) return null;

            SkillProfile[] profiles = Resources.LoadAll<SkillProfile>("Data/Inventory/Skills");
            for (int i = 0; i < profiles.Length; i++)
            {
                SkillProfile profile = profiles[i];
                if (profile == null) continue;

                if (profile.Id == id)
                {
                    return profile;
                }
            }

            return null;
        }
    }
}