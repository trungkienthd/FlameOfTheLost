using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CandidateLoadoutSubPanelCandidateListHandler : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private Transform _candidateListRoot;
        [SerializeField] private CandidateLoadoutSlotUIController _candidateSlotPrefab;
        private Transform CandidateListRoot => _candidateListRoot;
        private CandidateLoadoutSlotUIController CandidateSlotPrefab => _candidateSlotPrefab;

        //  2. Layout Configs
        [Header("Layout")]
        [SerializeField] private int _columnCount = 3;
        [SerializeField] private Vector2 _startPosition = new Vector2(-225f, 240f);
        [SerializeField] private float _stepX = 150f;
        [SerializeField] private float _stepY = 150f;
        private int ColumnCount => Mathf.Max(1, _columnCount);
        private Vector2 StartPosition => _startPosition;
        private float StepX => _stepX;
        private float StepY => _stepY;

        //  3. Runtime States
        //      3.1. Spawned Slots (Controllers)
        private readonly List<CandidateLoadoutSlotUIController> _spawnedSlots = new();
        private IReadOnlyList<CandidateLoadoutSlotUIController> SpawnedSlots => _spawnedSlots;

        //  4. Events
        public event Action<CandidateLoadoutSlotUIController> OnCandidateSlotClicked;

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.CandidateListRoot == null)
            {
                _candidateListRoot = this.transform;
            }
        }

        private void OnDestroy()
        {
            this.ClearSpawnedSlots();
        }
        public void RefreshCurrentPage(CandidateLoadoutSelectionModeType mode, IReadOnlyList<object> candidates, int currentPageIndex, int slotsPerPage)
        {
            this.ClearSpawnedSlots();

            if (candidates == null) return;
            if (slotsPerPage <= 0) return;

            int safePageIndex = Mathf.Max(0, currentPageIndex);
            int startIndex = safePageIndex * slotsPerPage;
            int endIndexExclusive = Mathf.Min(startIndex + slotsPerPage, candidates.Count);

            for (int i = startIndex; i < endIndexExclusive; i++)
            {
                int localIndex = i - startIndex;

                CandidateLoadoutSlotUIController slot = this.CreateCandidateSlot(localIndex);
                if (slot == null) continue;

                object candidate = candidates[i];
                this.BindCandidate(slot, mode, candidate);

                slot.OnClicked -= this.HandleCandidateSlotClicked;
                slot.OnClicked += this.HandleCandidateSlotClicked;

                _spawnedSlots.Add(slot);
            }
        }

        private CandidateLoadoutSlotUIController CreateCandidateSlot(int localIndex)
        {
            if (this.CandidateSlotPrefab == null || this.CandidateListRoot == null)
            {
                return null;
            }

            CandidateLoadoutSlotUIController instance = Instantiate(this.CandidateSlotPrefab, this.CandidateListRoot);

            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                int row = localIndex / this.ColumnCount;
                int column = localIndex % this.ColumnCount;

                float x = this.StartPosition.x + column * this.StepX;
                float y = this.StartPosition.y - row * this.StepY;

                rectTransform.anchoredPosition = new Vector2(x, y);
            }

            instance.gameObject.SetActive(true);
            return instance;
        }

        public void ClearSpawnedSlots()
        {
            for (int i = 0; i < this.SpawnedSlots.Count; i++)
            {
                CandidateLoadoutSlotUIController slot = this.SpawnedSlots[i];

                if (slot == null) continue;

                slot.OnClicked -= this.HandleCandidateSlotClicked;
                UnityEngine.Object.Destroy(slot.gameObject);
            }

            _spawnedSlots.Clear();
        }

        private void BindCandidate(CandidateLoadoutSlotUIController slot, CandidateLoadoutSelectionModeType mode, object candidate)
        {
            if (slot == null) return;

            switch (mode)
            {
                case CandidateLoadoutSelectionModeType.ReplaceWeapon:
                    slot.BindWeapon(candidate as InventoryItemEntry);
                    break;

                case CandidateLoadoutSelectionModeType.ReplaceConsumableItem:
                    slot.BindConsumableItem(candidate as InventoryItemEntry);
                    break;

                case CandidateLoadoutSelectionModeType.ReplaceCarriableItem:
                    slot.BindCarriableItem(candidate as InventoryItemEntry);
                    break;

                case CandidateLoadoutSelectionModeType.ChangeWeaponSpecialSkill:
                    slot.BindSpecialSkill(candidate as SkillProfile);
                    break;

                default:
                    slot.Clear();
                    break;
            }
        }

        private void HandleCandidateSlotClicked(CandidateLoadoutSlotUIController slot)
        {
            this.OnCandidateSlotClicked?.Invoke(slot);
        }
    }
}