using UnityEngine;

namespace FlameOfTheLost
{
    public partial class CandidateLoadoutSubPanelController : MonoBehaviour
    {
        // --- States ---
        //  1. Configs
        [Header("Configs")]
        [SerializeField] private int _slotsPerPage = 16;
        [SerializeField] private int _maxSupportedPages = 9;
        [SerializeField] private Vector2 _startPosition = new Vector2(-225f, 240f);
        [SerializeField] private float _stepX = 150f;
        [SerializeField] private float _stepY = 150f;
        private int SlotsPerPage => _slotsPerPage;
        private int MaxSupportedPages => _maxSupportedPages;
        private Vector2 StartPosition => _startPosition;
        private float StepX => _stepX;
        private float StepY => _stepY;

        //  3. Candidates
        [Header("Candidates")]
        [SerializeField] private Transform _candidatesRoot;
        [SerializeField] private CandidateLoadoutSlotUIController _candidateSlotPrefab;
        private Transform CandidatesRoot => _candidatesRoot;
        private CandidateLoadoutSlotUIController CandidateSlotPrefab => _candidateSlotPrefab;

        [Header("References (Player)")]
        [SerializeField] private CharacterCombatLoadoutController _playerCombatLoadoutController;
        [SerializeField] private PlayerInteractionLoadoutController _playerInteractionLoadoutController;
        [SerializeField] private InventoryController _playerInventoryController;
        private CharacterCombatLoadoutController PlayerCombatLoadoutController => _playerCombatLoadoutController;
        private PlayerInteractionLoadoutController PlayerInteractionLoadoutController => _playerInteractionLoadoutController;
        private InventoryController PlayerInventoryController => _playerInventoryController;

        [Header("References (Equipped Loadout Sub-Panel)")]
        [SerializeField] private EquippedLoadoutSubPanelController _equippedLoadoutSubPanelController;
        private EquippedLoadoutSubPanelController EquippedLoadoutSubPanelController => _equippedLoadoutSubPanelController;

        private EquippedLoadoutSlotUIController _targetSlot;
        private EquippedLoadoutSlotUIController TargetSlot
        {
            get => _targetSlot;
            set => _targetSlot = value;
        }

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.ButtonPrevious != null) _buttonPrevious.onClick.AddListener(this.GoToPreviousPage);
            if (this.ButtonNext != null) _buttonNext.onClick.AddListener(this.GoToNextPage);

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                if (this.PlayerCombatLoadoutController == null) _playerCombatLoadoutController = player.GetComponent<CharacterCombatLoadoutController>();
                if (this.PlayerInteractionLoadoutController == null) _playerInteractionLoadoutController = player.GetComponent<PlayerInteractionLoadoutController>();
                if (this.PlayerInventoryController == null) _playerInventoryController = player.GetComponent<InventoryController>();
            }

            if (this.EquippedLoadoutSubPanelController == null) _equippedLoadoutSubPanelController = this.transform.parent.GetComponentInChildren<EquippedLoadoutSubPanelController>();

        }
        private void OnDestroy()
        {
            if (this.ButtonPrevious != null) _buttonPrevious.onClick.RemoveListener(this.GoToPreviousPage);
            if (this.ButtonNext != null) _buttonNext.onClick.RemoveListener(this.GoToNextPage);
        }

        private CandidateLoadoutSlotUIController CreateCandidateSlot(int localIndex)
        {
            if (this.CandidateSlotPrefab == null || this.CandidatesRoot == null) return null;

            CandidateLoadoutSlotUIController instance = UnityEngine.Object.Instantiate(this.CandidateSlotPrefab, this.CandidatesRoot);

            RectTransform rect = instance.GetComponent<RectTransform>();
            if (rect != null)
            {
                int row = localIndex / 4;
                int column = localIndex % 4;

                float x = StartPosition.x + (column * StepX);
                float y = StartPosition.y - (row * StepY);
                rect.anchoredPosition = new Vector2(x, y);
            }

            return instance;
        }
    }
}