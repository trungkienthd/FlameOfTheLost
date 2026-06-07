using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class UICanvasHUDAppliedStatusEffectSlotContainerController : FOTLMonoBehaviour
    {
        // --- States ---
        //  1. References
        [Header("References")]
        [SerializeField] private StatusEffectController _playerStatusEffectController;
        [SerializeField] private UICanvasHUDAppliedStatusEffectSlotController _slotCtrl;
        private StatusEffectController PlayerStatusEffectController => _playerStatusEffectController;
        private UICanvasHUDAppliedStatusEffectSlotController SlotController => _slotCtrl;

        //  2. Layout Configuration
        [Header("Layout")]
        [SerializeField] private float _startX = 20f;
        [SerializeField] private float _stepX = 80f;
        [SerializeField] private int _maxSlots = 10;
        private float StartX => _startX;
        private float StepX => _stepX;
        private int MaxSlots => _maxSlots;

        //  3. Runtime
        private readonly List<UICanvasHUDAppliedStatusEffectSlotController> _spawnedSlots = new();
        private IReadOnlyList<UICanvasHUDAppliedStatusEffectSlotController> SpawnedSlots => _spawnedSlots;

        // --- Unity Callbacks ---
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
            if (this.SlotController == null) return;
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

                UICanvasHUDAppliedStatusEffectSlotController newSlot = UnityEngine.Object.Instantiate(this.SlotController, this.transform);
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
                UICanvasHUDAppliedStatusEffectSlotController slot = this.SpawnedSlots[i];

                if (slot == null)
                {
                    _spawnedSlots.RemoveAt(i);
                    continue;
                }

                slot.Refresh();
            }
        }

        private void SetSlotPosition(UICanvasHUDAppliedStatusEffectSlotController slot, int slotIndex)
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

        protected override void CacheReferences()
        {
            base.CacheReferences();

            if (this.PlayerStatusEffectController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _playerStatusEffectController = player.GetComponentInChildren<StatusEffectController>();
            }

            if (this.SlotController == null)
            {
                GameObject slot = Resources.Load<GameObject>("Prefabs/UI/HUD/AppliedStatusEffectSlot");
                if (slot != null) _slotCtrl = slot.GetComponent<UICanvasHUDAppliedStatusEffectSlotController>();
            }
        }

        protected override void ValidateReferences()
        {
            base.ValidateReferences();

            if (this.PlayerStatusEffectController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDAppliedStatusEffectSlotContainerController)}] PlayerStatusEffectController reference is not set and could not be found via fallback.");
            }

            if (this.SlotController == null)
            {
                Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDAppliedStatusEffectSlotContainerController)}] SlotController reference is not set and could not be found via fallback.");
            }
        }
    }
}