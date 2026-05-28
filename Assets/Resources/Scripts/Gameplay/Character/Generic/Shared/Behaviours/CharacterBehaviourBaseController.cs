using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterBehaviourBaseController : FOTLMonoBehaviour
    {
        [Header("References (All Character Components)")]
        [SerializeField] private CharacterComponentController _characterComponentController;
        protected CharacterComponentController CharacterComponentController => _characterComponentController;

        public string CharacterName => this.CharacterComponentController.gameObject.name;

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.CharacterComponentController == null) _characterComponentController = this.GetComponentInParent<CharacterComponentController>();
        }
        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.CharacterComponentController == null) Debug.LogError($"[{this.CharacterName}.{this.GetType().Name}] CharacterComponentController reference is not set in the inspector and could not be found via GetComponentInParent.");
        }
    }
}
