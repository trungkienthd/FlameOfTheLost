using UnityEngine;

namespace FlameOfTheLost
{
    public class UICanvasHUDResourceBarGroupController : FOTLMonoBehaviour
    {
        [Header("Gameplay Resources (Player)")]
        [SerializeField] private CharacterResourcesController _resources;
        private CharacterResourcesController PlayerResourcesController => _resources;

        [Header("UI Bars")]
        [SerializeField] private UICanvasHUDResourceBarController _hpBar;
        [SerializeField] private UICanvasHUDResourceBarController _fpBar;
        [SerializeField] private UICanvasHUDResourceBarController _staminaBar;
        private UICanvasHUDResourceBarController HPBar
        {
            get => _hpBar;
            set => _hpBar = value;
        }
        private UICanvasHUDResourceBarController FPBar
        {
            get => _fpBar;
            set => _fpBar = value;
        }
        private UICanvasHUDResourceBarController StaminaBar
        {
            get => _staminaBar;
            set => _staminaBar = value;
        }

        private bool _hasBoundExternalReferences;
        private bool HasBoundExternalReferences
        {
            get => _hasBoundExternalReferences;
            set => _hasBoundExternalReferences = value;
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            if (!this.HasBoundExternalReferences) return;

            this.SubscribeAll();
            this.SendAll();
        }
        private void OnDisable()
        {
            this.UnsubscribeAll();
        }

        public void SubscribeAll()
        {
            if (this.PlayerResourcesController != null)
            {
                if (this.PlayerResourcesController.HP != null)
                {
                    this.PlayerResourcesController.HP.OnValueChanged -= this.HandleHPChanged;
                    this.PlayerResourcesController.HP.OnValueChanged += this.HandleHPChanged;
                }

                if (this.PlayerResourcesController.FP != null)
                {
                    this.PlayerResourcesController.FP.OnValueChanged -= this.HandleFPChanged;
                    this.PlayerResourcesController.FP.OnValueChanged += this.HandleFPChanged;
                }

                if (this.PlayerResourcesController.Stamina != null)
                {
                    this.PlayerResourcesController.Stamina.OnValueChanged -= this.HandleStaminaChanged;
                    this.PlayerResourcesController.Stamina.OnValueChanged += this.HandleStaminaChanged;
                }
            }
        }

        public void UnsubscribeAll()
        { 
            if (this.PlayerResourcesController != null)
            {
                if (this.PlayerResourcesController.HP       != null) this.PlayerResourcesController.HP.OnValueChanged       -= this.HandleHPChanged;
                if (this.PlayerResourcesController.FP       != null) this.PlayerResourcesController.FP.OnValueChanged       -= this.HandleFPChanged;
                if (this.PlayerResourcesController.Stamina  != null) this.PlayerResourcesController.Stamina.OnValueChanged  -= this.HandleStaminaChanged;
            }
        }

        // --- Initial fill ---
        public void SendAll()
        {
            if (this.PlayerResourcesController == null) return;

            // Read and push each bar (if possible)
            if (this.HPBar != null && this.PlayerResourcesController.HP != null)
            {
                this.ReadHealth(out int currentHP, out int maxHP);
                UICanvasHUDResourceBarGroupController.PushToBar(this.HPBar, currentHP, maxHP);
            }

            if (this.FPBar != null && this.PlayerResourcesController.FP != null)
            {
                this.ReadFP(out int currentFP, out int maxFP);
                UICanvasHUDResourceBarGroupController.PushToBar(this.FPBar, currentFP, maxFP);
            }

            if (this.StaminaBar != null && this.PlayerResourcesController.Stamina != null)
            {
                this.ReadStamina(out int currentStamina, out int maxStamina);
                UICanvasHUDResourceBarGroupController.PushToBar(this.StaminaBar, currentStamina, maxStamina);
            }
        }

        // --- Event handlers (C# event path) ---
        private void HandleHPChanged(int currentValue, int maxValue) => UICanvasHUDResourceBarGroupController.PushToBar(this.HPBar, currentValue, maxValue);
        private void HandleFPChanged(int currentValue, int maxValue) => UICanvasHUDResourceBarGroupController.PushToBar(this.FPBar, currentValue, maxValue);
        private void HandleStaminaChanged(int currentValue, int maxValue) => UICanvasHUDResourceBarGroupController.PushToBar(this.StaminaBar, currentValue, maxValue);

        // --- Public hooks (for UnityEvent path) ---
        public void OnHealthChanged(int currentValue, int maxValue) => this.HandleHPChanged(currentValue, maxValue);
        public void OnFPChanged(int currentValue, int maxValue) => this.HandleFPChanged(currentValue, maxValue);
        public void OnStaminaChanged(int currentValue, int maxValue) => this.HandleStaminaChanged(currentValue, maxValue);

        // --- READ adapters --- 
        private void ReadHealth(out int currentValue, out int maxValue)
        {
            // Adjust to your CharacterResourcesController API
            currentValue = this.PlayerResourcesController.HP.CurrentValue;
            maxValue = this.PlayerResourcesController.HP.MaxValue;
        }
        private void ReadFP(out int currentValue, out int maxValue)
        {
            // Adjust to your CharacterResourcesController API
            currentValue = this.PlayerResourcesController.FP.CurrentValue;
            maxValue = this.PlayerResourcesController.FP.MaxValue;
        }
        private void ReadStamina(out int currentValue, out int maxValue)
        {
            // Adjust to your CharacterResourcesController API
            currentValue = this.PlayerResourcesController.Stamina.CurrentValue;
            maxValue = this.PlayerResourcesController.Stamina.MaxValue;
        }

        // --- PUSH adapter (works with common patterns) ---
        private static void PushToBar(UICanvasHUDResourceBarController bar, int currentValue, int maxValue)
        {
            if (bar == null) return;

            float t = (maxValue > 0f) ? Mathf.Clamp01((float)currentValue / maxValue) : 0f;

            // Try Set(current,max), then SetNormalized(t); finally a SendMessage fallback.
            bool pushed = false;

            // Try Set(current, max)
            try
            {
                bar.GetType().GetMethod("Set")?.Invoke(bar, new object[] { currentValue, maxValue });
                pushed = true;
            }
            catch { /* Method absent or Signature mismatch: Ignore */}

            if (!pushed)
            {
                // Try SetNormalized(t)
                try
                {
                    bar.GetType().GetMethod("SetNormalized")?.Invoke(bar, new object[] { t });
                    pushed = true;
                }
                catch { /* Method absent or Signature mismatch: Ignore */ }
            }

            if (!pushed)
            {
                // Last resort: allow a MonoBehaviour on the same GO to listen.
                // bar.SendMessage("SetNormalized", t, SendMessageOptions.DontRequireReceiver);
            }
        }

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            // Try to find bars by common names if not set
            if (this.HPBar == null)
            {
                Transform hpBar = this.transform.Find("HPBar");
                if (hpBar != null) this.HPBar = hpBar.GetComponent<UICanvasHUDResourceBarController>();
            }
            if (this.FPBar == null)
            {
                Transform fpBar = this.transform.Find("FPBar");
                if (fpBar != null) this.FPBar = fpBar.GetComponent<UICanvasHUDResourceBarController>();
            }
            if (this.StaminaBar == null)
            {
                Transform staminaBar = this.transform.Find("StaminaBar");
                if (staminaBar != null) this.StaminaBar = staminaBar.GetComponent<UICanvasHUDResourceBarController>();
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (this.HPBar == null)         Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDResourceBarGroupController)}] HPBar reference is missing.");
            if (this.FPBar == null)         Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDResourceBarGroupController)}] FPBar reference is missing.");
            if (this.StaminaBar == null)    Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDResourceBarGroupController)}] StaminaBar reference is missing.");
        }
        protected override void CacheExternalReferences()
        {
            base.CacheExternalReferences();

            if (this.PlayerResourcesController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _resources = player.GetComponentInChildren<CharacterResourcesController>();
            }
        }
        protected override void ValidateExternalReferences()
        {
            base.ValidateExternalReferences();

            if (this.PlayerResourcesController == null) Debug.LogError($"[{this.gameObject.name}.{nameof(UICanvasHUDResourceBarGroupController)}] Player's {nameof(CharacterResourcesController)} reference is missing.");
        }
        protected override void BindExternalReferences()
        {
            base.BindExternalReferences();

            this.SubscribeAll();

            this.HasBoundExternalReferences = true;
        }
        protected override void InitializePregameplayState()
        {
            base.InitializePregameplayState();

            this.SendAll();
        }
    }
}
