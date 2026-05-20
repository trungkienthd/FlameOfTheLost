using UnityEngine;

namespace FlameOfTheLost
{
    public class HUDResourcesPanelController : MonoBehaviour
    {
        [Header("Gameplay Resources (Player)")]
        [SerializeField] private CharacterResourcesController _resources;
        private CharacterResourcesController PlayerResourcesController => _resources;

        [Header("UI Bars")]
        [SerializeField] private ResourceBarUIController _hpBar;
        [SerializeField] private ResourceBarUIController _fpBar;
        [SerializeField] private ResourceBarUIController _staminaBar;
        private ResourceBarUIController HPBar
        {
            get => _hpBar;
            set => _hpBar = value;
        }
        private ResourceBarUIController FPBar
        {
            get => _fpBar;
            set => _fpBar = value;
        }
        private ResourceBarUIController StaminaBar
        {
            get => _staminaBar;
            set => _staminaBar = value;
        }

        // --- Unity Callbacks ---
        private void Awake()
        {
            if (this.PlayerResourcesController == null)
            {
                GameObject player = GameObject.Find("Player");
                if (player != null) _resources = player.GetComponentInChildren<CharacterResourcesController>();
                if (this.PlayerResourcesController == null) Debug.LogWarning($"[{this.gameObject.name}.TopLeftResourcesPanelUIController] Player's CharacterResourcesController is not found.");
            }

            // Try to find bars by common names if not set
            if (this.HPBar == null) this.HPBar = this.transform.GetChild(0).GetComponent<ResourceBarUIController>();
            if (this.FPBar == null) this.FPBar = this.transform.GetChild(1).GetComponent<ResourceBarUIController>();
            if (this.StaminaBar == null) this.StaminaBar = this.transform.GetChild(2).GetComponent<ResourceBarUIController>();

        }

        private void OnEnable()
        {
            this.SendAll();
            this.SubscribeAll();
        }

        private void OnDisable()
        {
            this.UnsubscribeAll();
        }

        public void SubscribeAll()
        {
            if (this.PlayerResourcesController != null)
            {
                try
                {
                    this.PlayerResourcesController.HP.OnValueChanged -= this.HandleHealthChanged;
                    this.PlayerResourcesController.HP.OnValueChanged += this.HandleHealthChanged;
                }
                catch { /* Ignore if already subscribed */}

                try
                {
                    this.PlayerResourcesController.FP.OnValueChanged -= this.HandleFPChanged;
                    this.PlayerResourcesController.FP.OnValueChanged += this.HandleFPChanged;
                }
                catch { /* Ignore if already subscribed */ }

                try
                {
                    this.PlayerResourcesController.Stamina.OnValueChanged -= this.HandleStaminaChanged;
                    this.PlayerResourcesController.Stamina.OnValueChanged += this.HandleStaminaChanged;
                }
                catch { /* Ignore if already subscribed */ }
            }
        }

        public void UnsubscribeAll()
        { 
            if (this.PlayerResourcesController != null)
            {
                try
                {
                    this.PlayerResourcesController.HP.OnValueChanged -= this.HandleHealthChanged;
                }
                catch { /* Ignore if not subscribed */}

                try
                {
                    this.PlayerResourcesController.FP.OnValueChanged -= this.HandleFPChanged;
                }
                catch { /* Ignore if not subscribed */ }

                try
                {
                    this.PlayerResourcesController.Stamina.OnValueChanged -= this.HandleStaminaChanged;
                }
                catch { /* Ignore if not subscribed */ }
            }
        }

        // --- Initial fill ---
        public void SendAll()
        {
            // Read and push each bar (if possible)
            if (this.HPBar != null && this.PlayerResourcesController != null)
            {
                this.ReadHealth(out int currentHP, out int maxHP);
                HUDResourcesPanelController.PushToBar(this.HPBar, currentHP, maxHP);
            }

            if (this.FPBar != null && this.PlayerResourcesController != null)
            {
                this.ReadFP(out int currentFP, out int maxFP);
                HUDResourcesPanelController.PushToBar(this.FPBar, currentFP, maxFP);
            }

            if (this.StaminaBar != null && this.PlayerResourcesController != null)
            {
                this.ReadStamina(out int currentStamina, out int maxStamina);
                HUDResourcesPanelController.PushToBar(this.StaminaBar, currentStamina, maxStamina);
            }
        }

        // --- Event handlers (C# event path) ---
        private void HandleHealthChanged(int currentValue, int maxValue) => HUDResourcesPanelController.PushToBar(this.HPBar, currentValue, maxValue);
        private void HandleFPChanged(int currentValue, int maxValue) => HUDResourcesPanelController.PushToBar(this.FPBar, currentValue, maxValue);
        private void HandleStaminaChanged(int currentValue, int maxValue) => HUDResourcesPanelController.PushToBar(this.StaminaBar, currentValue, maxValue);

        // --- Public hooks (for UnityEvent path) ---
        public void OnHealthChanged(int currentValue, int maxValue) => this.HandleHealthChanged(currentValue, maxValue);
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
        private static void PushToBar(ResourceBarUIController bar, int currentValue, int maxValue)
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
    }
}
