using UnityEngine;

namespace FlameOfTheLost
{
    public abstract class CharacterBehaviourBaseController : MonoBehaviour
    {
        [Header("References (All Character Components)")]
        [SerializeField] private CharacterComponentController _characterComponentController;
        protected CharacterComponentController CharacterComponentController => _characterComponentController;

        public string CharacterName => this.CharacterComponentController.gameObject.name;

        protected virtual void Awake()
        {
            this.CacheReferences();
        }

        protected virtual void CacheReferences()
        {
            if (this.CharacterComponentController == null) _characterComponentController = this.GetComponentInParent<CharacterComponentController>();
        }
    }
}
