using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterIdentityController : CharacterBehaviourBaseController, IIdentityProvider
    {
        // --- States ---
        //  1. Identity
        [Header("Identity")]
        [SerializeField] private CharacterIdentityProfile _identityProfile;
        public CharacterIdentityProfile IdentityProfile => _identityProfile;

        public int Id => this.IdentityProfile != null ? this.IdentityProfile.Id : 0;
        public string Key => this.IdentityProfile != null ? this.IdentityProfile.Key : string.Empty;
        public string DisplayName => this.IdentityProfile != null ? this.IdentityProfile.DisplayName : this.CharacterName;
        public string Description => this.IdentityProfile != null ? this.IdentityProfile.Description : string.Empty;

        public CharacterType CharacterType => this.IdentityProfile != null
            ? this.IdentityProfile.CharacterType
            : CharacterType.Default;

        public CharacterFaction Faction => this.IdentityProfile != null
            ? this.IdentityProfile.Faction
            : CharacterFaction.Neutral;

        // --- Public APIs ---
        public bool IsPlayer => this.Faction == CharacterFaction.Player;
        public bool IsEnemy => this.Faction == CharacterFaction.Enemy;
        public bool IsAlly => this.Faction == CharacterFaction.Ally;

        // --- Helpers ---
        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.IdentityProfile == null)
            {
                Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] IdentityProfile is not assigned.");
            }
        }
    }
}
