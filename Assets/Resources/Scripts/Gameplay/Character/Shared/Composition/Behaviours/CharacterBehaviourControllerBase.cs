using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterBehaviourControllerBase : FOTLMonoBehaviour
    {
        [Header("References (Character Root)")]
        [SerializeField] private CharacterRootController _characterRootController;
        protected CharacterRootController CharacterRootController => _characterRootController;

        public string CharacterName => this.CharacterRootController.gameObject.name;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.CharacterRootController == null) _characterRootController = this.GetComponentInParent<CharacterRootController>();
        }
        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.CharacterRootController == null) Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] CharacterComponentController reference is not set in the inspector and could not be found via GetComponentInParent.");
        }
    }
}
