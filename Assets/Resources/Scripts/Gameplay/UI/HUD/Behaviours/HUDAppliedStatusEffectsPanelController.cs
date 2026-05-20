using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class HUDAppliedStatusEffectsPanelController : MonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private StatusEffectController _playerStatusEffectController;
        [SerializeField] private AppliedStatusEffectSlotUIController _slotPrefabCtrl;
        private StatusEffectController PlayerStatusEffectController => _playerStatusEffectController;
        private AppliedStatusEffectSlotUIController SlotPrefabController => _slotPrefabCtrl;

        //  2. Layout Configuration
        [Header("Layout")]
        [SerializeField] private float _startX = 20f;
        [SerializeField] private float _stepX = 80f;
        [SerializeField] private int _maxSlots = 10;
        private float StartX => _startX;
        private float StepX => _stepX;
        private int MaxSlots => _maxSlots;

        //  3. Runtime
        private readonly List<AppliedStatusEffectSlotUIController> _spawnedSlots = new();
        private IReadOnlyList<AppliedStatusEffectSlotUIController> SpawnedSlots => _spawnedSlots;

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.PlayerStatusEffectController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _playerStatusEffectController = player.GetComponentInChildren<StatusEffectController>();
            }
        }
        private void OnEnable()
        {
            if (this.PlayerStatusEffectController != null)
            {
                this.PlayerStatusEffectController.OnRuntimeListChanged += this.RebuildSlots;
                this.PlayerStatusEffectController.OnRuntimeValuesChanged += this.RefreshSlotValues;
            }

            this.RebuildSlots();
        }

        private void OnDisable()
        {
            if (this.PlayerStatusEffectController != null)
            {
                this.PlayerStatusEffectController.OnRuntimeListChanged -= this.RebuildSlots;
                this.PlayerStatusEffectController.OnRuntimeValuesChanged -= this.RefreshSlotValues;
            }
        }


        private void RebuildSlots()
        {
            this.ClearSlots();

            if (this.PlayerStatusEffectController == null) return;
            if (this.SlotPrefabController == null) return;
            if (this.MaxSlots <= 0) return;

            IReadOnlyList<RuntimeStatusEffect> runtimeStatusEffects = this.PlayerStatusEffectController.RuntimeStatusEffects;
            if (runtimeStatusEffects == null || runtimeStatusEffects.Count == 0) return;

            // RuntimeStatusEffects order is oldest -> newest.
            // Therefore, we loop backwards so the newest one appears first / farthest left.
            int createdSlotCount = 0;
            for (int i = runtimeStatusEffects.Count - 1; i >= 0; i--)
            {
                if (createdSlotCount >= this.MaxSlots) break; 

                RuntimeStatusEffect runtimeStatusEffect = runtimeStatusEffects[i];
                if (runtimeStatusEffect == null || runtimeStatusEffect.Profile == null) continue;

                AppliedStatusEffectSlotUIController newSlot = UnityEngine.Object.Instantiate(this.SlotPrefabController, this.transform);
                newSlot.name = $"AppliedStatusEffectSlot_{createdSlotCount}_{runtimeStatusEffect.Profile.name}";
                newSlot.Bind(runtimeStatusEffect);

                this.SetSlotPosition(newSlot, createdSlotCount);

                _spawnedSlots.Add(newSlot);

                createdSlotCount++;
            }
        }

        private void RefreshSlotValues()
        {
            for (int i = this.SpawnedSlots.Count - 1; i >= 0; i--)
            {
                AppliedStatusEffectSlotUIController slot = this.SpawnedSlots[i];

                if (slot == null)
                {
                    _spawnedSlots.RemoveAt(i);
                    continue;
                }

                slot.Refresh();
            }
        }

        private void SetSlotPosition(AppliedStatusEffectSlotUIController slot, int slotIndex)
        {
            if (slot == null) return;

            RectTransform rectTransform = slot.transform as RectTransform;

            float x = this.StartX + slotIndex * this.StepX;

            if (rectTransform != null)
            {
                Vector2 anchoredPosition = rectTransform.anchoredPosition;
                anchoredPosition.x = x;
                rectTransform.anchoredPosition = anchoredPosition;
            }
            else
            {
                Vector3 localPosition = slot.transform.localPosition;
                localPosition.x = x;
                slot.transform.localPosition = localPosition;
            }
        }


        private void ClearSlots()
        {
            for (int i = this.SpawnedSlots.Count - 1; i >= 0; i--)
            {
                if (this.SpawnedSlots[i] != null)
                {
                    UnityEngine.Object.Destroy(this.SpawnedSlots[i].gameObject);
                }
            }

            _spawnedSlots.Clear();
        }
    }
}