using UnityEngine;

namespace FlameOfTheLost
{
    public class TopLeftResourcesUIController : MonoBehaviour
    {
        [Header("Gameplay Sources")]
        [SerializeField] private HealthController _health;
        [SerializeField] private CharacterResourcesController _resources;
        private HealthController PlayerHealthController
        {
            get => _health;
            set => _health = value;
        }
        private CharacterResourcesController PlayerResourcesController
        {
            get => _resources;
            set => _resources = value;
        }

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

        [Header("Settings")]
        [SerializeField] private bool _autoFindIfNull = true;
        private bool AutoFindIfNull => _autoFindIfNull;

        // --- Public quick binders (handy if you swap player runtime) ---
        public void BindSources(HealthController health, CharacterResourcesController resources)
        {
            // Unhook previous
            this.UnsubscribeAll();

            this.PlayerHealthController = health;
            this.PlayerResourcesController = resources;

            // Send + subscribe now
            this.SendAll();
            this.SubscribeAll();
        }

        public void BindBars(ResourceBarUIController hp, ResourceBarUIController fp, ResourceBarUIController stamina)
        {
            this.HPBar = hp;
            this.FPBar = fp;
            this.StaminaBar = stamina;

            // This will fill current values if sources exist
            this.SendAll();
        }

        // --- Unity lifecycle ---
        private void Awake()
        {
            if (this.AutoFindIfNull)
            {
                // Try to check if sources on the Player in scene are not set
                if (this.PlayerHealthController == null) 
                    Debug.LogWarning($"[{this.name}] TopLeftResourcesUIController: Player's HealthController is not found");
                if (this.PlayerResourcesController == null) 
                    Debug.LogWarning($"[{this.name}] TopLeftResourcesUIController: Player's CharacterResourcesController is not found.");

                // Try to find bars by common names if not set
                if (this.HPBar == null) this.HPBar = this.transform.GetChild(0).GetComponent<ResourceBarUIController>();
                if (this.FPBar == null) this.FPBar = this.transform.GetChild(1).GetComponent<ResourceBarUIController>();
                if (this.StaminaBar == null) this.StaminaBar = this.transform.GetChild(2).GetComponent<ResourceBarUIController>();
            }
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
            if (this.PlayerHealthController != null)
            {
                try
                {
                    this.PlayerHealthController.OnHPChanged -= this.HandleHealthChanged;
                    this.PlayerHealthController.OnHPChanged += this.HandleHealthChanged;
                }
                catch { /* Ignore if already subscribed */ }
            }

            if (this.PlayerResourcesController != null)
            {
                try
                {
                    this.PlayerResourcesController.OnFPChanged -= this.HandleFPChanged;
                    this.PlayerResourcesController.OnFPChanged += this.HandleFPChanged;
                }
                catch { /* Ignore if already subscribed */ }

                try
                {
                    this.PlayerResourcesController.OnStaminaChanged -= this.HandleStaminaChanged;
                    this.PlayerResourcesController.OnStaminaChanged += this.HandleStaminaChanged;
                }
                catch { /* Ignore if already subscribed */ }
            }
        }

        public void UnsubscribeAll()
        { 
            if (this.PlayerHealthController != null)
            {
                try
                {
                    this.PlayerHealthController.OnHPChanged -= this.HandleHealthChanged;
                }
                catch { /* Ignore if not subscribed */ }
            }

            if (this.PlayerResourcesController != null)
            {
                try
                {
                    this.PlayerResourcesController.OnFPChanged -= this.HandleFPChanged;
                }
                catch { /* Ignore if not subscribed */ }

                try
                {
                    this.PlayerResourcesController.OnStaminaChanged -= this.HandleStaminaChanged;
                }
                catch { /* Ignore if not subscribed */ }
            }
        }

        // --- Initial fill ---
        public void SendAll()
        {
            // Read and push each bar (if possible)
            if (this.HPBar != null && this.PlayerHealthController != null)
            {
                this.ReadHealth(out int currentHP, out int maxHP);
                TopLeftResourcesUIController.PushToBar(this.HPBar, currentHP, maxHP);
            }

            if (this.FPBar != null && this.PlayerResourcesController != null)
            {
                this.ReadFP(out int currentFP, out int maxFP);
                TopLeftResourcesUIController.PushToBar(this.FPBar, currentFP, maxFP);
            }

            if (this.StaminaBar != null && this.PlayerResourcesController != null)
            {
                this.ReadStamina(out int currentStamina, out int maxStamina);
                TopLeftResourcesUIController.PushToBar(this.StaminaBar, currentStamina, maxStamina);
            }
        }

        // --- Event handlers (C# event path) ---
        private void HandleHealthChanged(int currentValue, int maxValue) => TopLeftResourcesUIController.PushToBar(this.HPBar, currentValue, maxValue);
        private void HandleFPChanged(int currentValue, int maxValue) => TopLeftResourcesUIController.PushToBar(this.FPBar, currentValue, maxValue);
        private void HandleStaminaChanged(int currentValue, int maxValue) => TopLeftResourcesUIController.PushToBar(this.StaminaBar, currentValue, maxValue);

        // --- Public hooks (for UnityEvent path) ---
        public void OnHealthChanged(int currentValue, int maxValue) => this.HandleHealthChanged(currentValue, maxValue);
        public void OnFPChanged(int currentValue, int maxValue) => this.HandleFPChanged(currentValue, maxValue);
        public void OnStaminaChanged(int currentValue, int maxValue) => this.HandleStaminaChanged(currentValue, maxValue);

        // --- READ adapters --- 
        private void ReadHealth(out int currentValue, out int maxValue)
        {
            // Adjust to your HealthController API
            currentValue = this.PlayerHealthController.CurrentHP;
            maxValue = this.PlayerHealthController.MaxHP;
        }
        private void ReadFP(out int currentValue, out int maxValue)
        {
            // Adjust to your CharacterResourcesController API
            currentValue = this.PlayerResourcesController.CurrentFP;
            maxValue = this.PlayerResourcesController.MaxFP;
        }
        private void ReadStamina(out int currentValue, out int maxValue)
        {
            // Adjust to your CharacterResourcesController API
            currentValue = this.PlayerResourcesController.CurrentStamina;
            maxValue = this.PlayerResourcesController.MaxStamina;
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
                bar.SendMessage("SetNormalized", t, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
