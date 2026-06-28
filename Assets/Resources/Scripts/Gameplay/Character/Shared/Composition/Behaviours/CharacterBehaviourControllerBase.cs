using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterBehaviourControllerBase : FOTLMonoBehaviour
    {
        [Header("References (Character Root)")]
        [SerializeField] private CharacterRootController _characterRootController;
        protected CharacterRootController CharacterRootController => _characterRootController;

        public string CharacterName => this.CharacterRootController.gameObject.name;

        public CharacterRootController GetCharacter()
        {
            return this.CharacterRootController;
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.CharacterRootController == null) _characterRootController = this.GetComponentInParent<CharacterRootController>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.CharacterRootController == null) Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] {nameof(CharacterRootController)} reference is not set in the inspector and could not be found via GetComponentInParent.", this);
        }
    }
}
