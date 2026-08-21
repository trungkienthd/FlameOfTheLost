using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class CharacterTerrainContactHandler : CharacterBehaviourControllerBase
    {
        // --- States ---
        //  1. Traversal type
        public CharacterTraversalType TraversalType => this.CharacterRootController.IdentityController.TraversalType;

        //  2. References 
        [Header("References")]
        [SerializeField] private StatusEffectController _statusEffectController;
        [SerializeField] private CharacterCombatStatusEffectApplicationHandler _statusEffectApplicationHandler;
        private StatusEffectController StatusEffectController => _statusEffectController;
        private CharacterCombatStatusEffectApplicationHandler StatusEffectApplicationHandler => _statusEffectApplicationHandler;

        [Header("Refresh")]
        [SerializeField, Min(0.02f)] private float _terrainStatusEffectRefreshInterval = 0.15f;
        public float TerrainStatusEffectRefreshInterval => _terrainStatusEffectRefreshInterval;

        //  4. Runtime
        private readonly HashSet<TerrainController> _touchingTerrains = new();

        private TerrainController _currentTerrainCtrl;
        private TerrainController CurrentTerrainCtrl => _currentTerrainCtrl;

        private float _nextTerrainStatusEffectRefreshTime;
        private float NextTerrainStatusEffectRefreshTime => _nextTerrainStatusEffectRefreshTime;

        // --- Unity Callbacks ---
        protected override void Awake()
        {
            base.Awake();

            if (this.StatusEffectController == null) _statusEffectController = this.GetComponentInChildren<StatusEffectController>();
            if (this.StatusEffectApplicationHandler == null) _statusEffectApplicationHandler = this.GetComponentInChildren<CharacterCombatStatusEffectApplicationHandler>();
        }
        private void Update()
        {
            if (this.CurrentTerrainCtrl == null) return;
            if (Time.time < this.NextTerrainStatusEffectRefreshTime) return;

            _nextTerrainStatusEffectRefreshTime = Time.time + this.TerrainStatusEffectRefreshInterval;

            this.ApplyCurrentTerrainStatusEffects();
        }

        // --- Public APIs ---
        public void RegisterTerrain(TerrainController terrainCtrl)
        {
            if (terrainCtrl == null) return;
            if (terrainCtrl.Profile == null) return;

            _touchingTerrains.Add(terrainCtrl);
            this.RefreshCurrentTerrain();
        }

        public void UnregisterTerrain(TerrainController terrainCtrl)
        {
            if (terrainCtrl == null) return;

            _touchingTerrains.Remove(terrainCtrl);

            if (this.CurrentTerrainCtrl == terrainCtrl)
            {
                this.ClearCurrentTerrainStatusEffects();
                _currentTerrainCtrl = null;
            }

            this.RefreshCurrentTerrain();
        }

        // --- Helpers ---
        private void RefreshCurrentTerrain()
        {
            TerrainController bestTerrainCtrl = this.GetHighestPriorityTerrainController();

            if (bestTerrainCtrl == this.CurrentTerrainCtrl) return;

            this.ClearCurrentTerrainStatusEffects();

            _currentTerrainCtrl = bestTerrainCtrl;

            _nextTerrainStatusEffectRefreshTime = 0f;

            this.ApplyCurrentTerrainStatusEffects();
        }

        private TerrainController GetHighestPriorityTerrainController()
        {
            TerrainController bestTerrainCtrl = null;
            int bestPriority = int.MinValue;

            foreach (TerrainController terrainCtrl in _touchingTerrains)
            {
                if (terrainCtrl == null) continue;
                if (terrainCtrl.Profile == null) continue;

                int priority = terrainCtrl.Priority;

                if (bestTerrainCtrl == null || priority > bestPriority)
                {
                    bestTerrainCtrl = terrainCtrl;
                    bestPriority = priority;
                }
            }

            return bestTerrainCtrl;
        }

        private void ApplyCurrentTerrainStatusEffects()
        {
            if (this.CurrentTerrainCtrl == null) return;
            if (this.CurrentTerrainCtrl.Profile == null) return;
            if (this.StatusEffectApplicationHandler == null) return;

            TerrainProfile terrainProfile = this.CurrentTerrainCtrl.Profile;

            if (!terrainProfile.TryGetRule(this.TraversalType, out TerrainStatusEffectRuleEntry rule)) return;
            if (rule == null) return;
            if (!rule.CanEnter) return;

            this.StatusEffectApplicationHandler.TryApplyStatusEffectApplicationsToSelf
            (
                rule.Applications,
                this.CurrentTerrainCtrl.gameObject
            );
        }

        private void ClearCurrentTerrainStatusEffects()
        {
            if (this.CurrentTerrainCtrl == null) return;
            if (this.StatusEffectController == null) return;

            this.StatusEffectController.RemoveStatusEffectsFromSource(this.CurrentTerrainCtrl.gameObject);
        }
    }
}
