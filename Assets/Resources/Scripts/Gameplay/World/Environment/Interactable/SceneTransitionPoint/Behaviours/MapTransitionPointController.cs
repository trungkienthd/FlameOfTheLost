using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(MapTransitionPointInteractionHandler))]
    public class MapTransitionPointController : FOTLWorldObjectProfileBehaviour<MapTransitionPointProfile>,
          IPlayerInteractable,
          IPlayerInteractionPromptSource
    {
        // --- References ---
        //  1. Arrival Point
        [Header("References - Arrival")]
        [SerializeField] private Transform _arrivalPoint;
        private Transform ArrivalPoint => _arrivalPoint;

        //  2. Sub-Behaviours
        [Header("References - Sub-Behaviours")]
        [SerializeField]
        private MapTransitionPointInteractionHandler _interactionHandler;
        private MapTransitionPointInteractionHandler InteractionHandler => _interactionHandler;

        // --- Data / Configs ---
        //  1. Arrival
        [Header("Configs - Arrival")]
        [SerializeField] private string _arrivalKey;
        [SerializeField] private CharacterFacingStatus _arrivalFacing = CharacterFacingStatus.Side;
        public string ArrivalKey => _arrivalKey;
        public CharacterFacingStatus ArrivalFacing => _arrivalFacing;

        public Vector3 ArrivalPosition => this.ArrivalPoint != null
            ? this.ArrivalPoint.position
            : this.transform.position;

        //  2. Transition Destination
        public string DestinationSceneName => this.Profile != null
            ? this.Profile.DestinationSceneName
            : string.Empty;

        public string DestinationArrivalKey => this.Profile != null
            ? this.Profile.DestinationArrivalKey
            : string.Empty;

        //  3. Interaction Prompt
        public int InteractionPromptPriority => 0;

        // --- Public APIs ---
        //  1. IPlayerInteractable Implementation
        public bool CanInteract(GameObject interactor)
        {
            if (!this.isActiveAndEnabled) return false;

            if (interactor == null) return false;
            if (interactor.GetComponent<PlayerRootController>() == null) return false;

            if (this.Profile == null) return false;

            if (string.IsNullOrWhiteSpace(this.DestinationSceneName)) return false;

            if (string.IsNullOrWhiteSpace(this.DestinationArrivalKey)) return false;

            if (SceneTransitionPointRequest.HasPendingRequest)  return false;
            if (WorldMapTransitionRequest.HasPendingRequest)    return false;
            if (MainMenuSessionRequest.HasPendingRequest)       return false;

            if (this.InteractionHandler == null) return false;

            return true;
        }
        public bool TryInteract(GameObject interactor)
        {
            if (!this.CanInteract(interactor))
                return false;

            return this.InteractionHandler.TryTransition(interactor);
        }

        //  2. IPlayerInteractionPromptSource Implementation
        public bool CanShowInteractionPrompt(GameObject interactor)
        {
            return this.CanInteract(interactor);
        }
        public string GetInteractionPromptText(GameObject interactor)
        {
            if (!this.CanShowInteractionPrompt(interactor)) return string.Empty;

            string promptText = this.Profile.InteractionPromptText;

            return string.IsNullOrWhiteSpace(promptText)
                ? "Enter"
                : promptText;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (this.ArrivalPoint == null) _arrivalPoint = this.transform.Find("ArrivalPoint");

            if (this.InteractionHandler == null) _interactionHandler = this.GetComponent<MapTransitionPointInteractionHandler>();
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.ArrivalPoint == null)
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(MapTransitionPointController)}] ArrivalPoint is not assigned. Root position will be used.", this);
            }

            if (this.InteractionHandler == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(MapTransitionPointController)}] {nameof(MapTransitionPointInteractionHandler)} is missing.", this);
            }

            if (string.IsNullOrWhiteSpace(this.ArrivalKey))
            {
                Debug.LogWarning($"[{this.gameObject.name}.{nameof(MapTransitionPointController)}] ArrivalKey is empty.", this);
            }
        }
        protected override void BindInternalReferences()
        {
            base.BindInternalReferences();

            this.InteractionHandler?.Bind(this);
        }
        protected override void UnbindInternalReferences()
        {
            this.InteractionHandler?.Unbind(this);

            base.UnbindInternalReferences();
        }
    }
}
