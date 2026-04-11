using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class HUDAppliedStatusEffectsPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StatusEffectController _playerStatusEffectController;
        [SerializeField] private AppliedStatusEffectSlotUIController _slotPrefabCtrl;
        private StatusEffectController PlayerStatusEffectController => _playerStatusEffectController;
        private AppliedStatusEffectSlotUIController SlotPrefabController => _slotPrefabCtrl;

        [Header("Layout")]
        [SerializeField] private float _startX = 20f;
        [SerializeField] private float _stepX = 80f;
        [SerializeField] private int _maxSlots = 10;
        private float StartX => _startX;
        private float StepX => _stepX;
        private int MaxSlots => _maxSlots;

        private readonly List<AppliedStatusEffectSlotUIController> _spawnedSlots = new();
        private IReadOnlyList<AppliedStatusEffectSlotUIController> SpawnedSlots => _spawnedSlots;

        private void Awake()
        {
            if (this.PlayerStatusEffectController == null) _playerStatusEffectController = UnityEngine.Object.FindFirstObjectByType<StatusEffectController>();
        }
        private void OnEnable()
        {
            if (this.PlayerStatusEffectController != null)
            {
                this.PlayerStatusEffectController.OnRuntimeListChanged += this.RefreshUI;
                this.PlayerStatusEffectController.OnRuntimeValuesChanged += this.RefreshUI;
            }

            this.RefreshUI();
        }
        private void OnDisable()
        {
            if (this.PlayerStatusEffectController != null)
            {
                this.PlayerStatusEffectController.OnRuntimeListChanged -= this.RefreshUI;
                this.PlayerStatusEffectController.OnRuntimeValuesChanged -= this.RefreshUI;
            }
        }


        private void RefreshUI()
        {
            this.ClearSlots();

            if (this.PlayerStatusEffectController == null || this.SlotPrefabController == null) return;

            IReadOnlyList<RuntimeStatusEffect> runtimeStatusEffects = this.PlayerStatusEffectController.RuntimeStatusEffects;
            if (runtimeStatusEffects == null || runtimeStatusEffects.Count == 0) return;

            int startIndex = Mathf.Max(0, runtimeStatusEffects.Count - this.MaxSlots);
            int slotIndex = 0;

            for (int i = startIndex; i < runtimeStatusEffects.Count; i++)
            {
                RuntimeStatusEffect runtimeStatusEffect = runtimeStatusEffects[i];
                if (runtimeStatusEffect == null || runtimeStatusEffect.Profile == null) continue;

                AppliedStatusEffectSlotUIController newSlot = UnityEngine.Object.Instantiate(this.SlotPrefabController, this.transform);
                newSlot.name = $"AppliedStatusEffectSlot_{slotIndex}_{runtimeStatusEffect.Profile.name}";
                newSlot.SetIcon(runtimeStatusEffect.Profile.Icon);

                RectTransform rectTransform = newSlot.transform as RectTransform;
                if (rectTransform != null)
                {
                    Vector2 anchoredPosition = rectTransform.anchoredPosition;
                    anchoredPosition.x = this.StartX + (slotIndex * this.StepX);
                    rectTransform.anchoredPosition = anchoredPosition;
                }
                else
                {
                    Vector3 localPosition = newSlot.transform.localPosition;
                    localPosition.x = this.StartX + (slotIndex * this.StepX);
                    newSlot.transform.localPosition = localPosition;
                }

                _spawnedSlots.Add(newSlot);
                slotIndex++;
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