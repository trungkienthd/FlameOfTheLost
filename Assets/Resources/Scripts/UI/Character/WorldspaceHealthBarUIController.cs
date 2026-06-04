using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public class WorldspaceHealthBarUI : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private HPController _targetHP;        // Auto-finds in parent if null
        [SerializeField] private Slider _bar;                     // Drag Bar (Slider)
        [SerializeField] private TextMeshProUGUI _healthText;     // Drag HealthText (TMP)
        [SerializeField] private CanvasGroup _canvasGroup;        // Auto-fills on Awake
        private HPController TargetHP
        {
            get => _targetHP;
            set => _targetHP = value;
        }
        private Slider BarSlider
        {
            get => _bar;
            set => _bar = value;
        }
        private TextMeshProUGUI HPText
        {
            get => _healthText;
            set => _healthText = value;
        }
        private CanvasGroup CanvasGroup
        {
            get => _canvasGroup;
            set => _canvasGroup = value;
        }

        [Header("Layout")]
        [Tooltip("Local offset from parent (keep Z=0 for 2D).")]
        [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 1.1f, 0f);
        private Vector3 WorldOffset => _worldOffset;

        [Header("Visibility")]
        [SerializeField] private bool _alwaysVisible = false;
        [Min(0f)][SerializeField] private float _autoHideDelay = 1.5f;
        [Min(0f)][SerializeField] private float _fadeDuration = 0.15f;
        private bool AlwaysVisible => _alwaysVisible;
        private float AutoHideDelay => _autoHideDelay;
        private float FadeDuration => _fadeDuration;

        [Header("Behavior")]
        [Tooltip("If true, reads CurrentHealth/MaxHealth each frame ONLY when no health event fired that frame.")]
        [SerializeField] private bool _usePollingFallback = true;
        [SerializeField] private bool _mutateFromDeltaEvents = false;
        private bool UsePollingFallback => _usePollingFallback;
        private bool MutateFromDeltaEvents => _mutateFromDeltaEvents;

        // State (int-based to match your events)
        private int _current = 0;
        private int _max = 1;
        private int CurrentHP
        {
            get => _current;
            set => _current = value;
        }
        private int MaxHP
        { 
            get => _max;
            set => _max = value;
        }

        private Coroutine _fadeRoutine;
        private float _hideAtTime = -1f;
        private Coroutine FadeRoutineRef
        { 
            get => _fadeRoutine;
            set => _fadeRoutine = value;
        }
        private float HideAtTime
        {
            get => _hideAtTime;
            set => _hideAtTime = value;
        }

        // Event/Poll coordination
        private bool _eventDrivenUpdateReceived = false;
        private bool EventDrivenUpdateReceived
        {
            get => _eventDrivenUpdateReceived;
            set => _eventDrivenUpdateReceived = value;
        }

        // Optional cached reflection for initial read / polling fallback
        private PropertyInfo _piCurrent;
        private PropertyInfo _piMax;
        private PropertyInfo PropertyInfoCurrent
        {
            get => _piCurrent;
            set => _piCurrent = value;
        }
        private PropertyInfo PropertyInfoMax
        {
            get => _piMax;
            set => _piMax = value;
        }

        private float NormalizedHPState => this.MaxHP > 0 ? (float)this.CurrentHP / this.MaxHP : 0f;

        private void Awake()
        {
            if (this.CanvasGroup == null) this.CanvasGroup = this.GetComponent<CanvasGroup>();
            if (this.TargetHP == null) this.TargetHP = this.GetComponentInParent<HPController>();

            if (this.BarSlider == null) this.BarSlider = this.GetComponentInChildren<Slider>(true);
            if (this.HPText == null) this.HPText = this.GetComponentInChildren<TextMeshProUGUI>(true);

            if (this.BarSlider != null)
            {
                this.BarSlider.minValue = 0f;
                this.BarSlider.maxValue = 1f;
                this.BarSlider.wholeNumbers = false;
                this.BarSlider.interactable = false;
            }

            this.transform.localPosition = this.WorldOffset;

            // Cache property infos if present (optional)
            if (this.TargetHP != null)
            {
                var t = this.TargetHP.GetType();
                this.PropertyInfoCurrent = t.GetProperty("CurrentHealth", BindingFlags.Public | BindingFlags.Instance);
                this.PropertyInfoMax = t.GetProperty("MaxHealth", BindingFlags.Public | BindingFlags.Instance);

                // Initial sync (non-fatal if properties don’t exist)
                // this.TryReadTarget(out _current, out _max);
                int current, max;
                if (this.TryReadTarget(out current, out max))
                {
                    max = Mathf.Max(1, max);
                    current = Mathf.Clamp(current, 0, max);

                    this.MaxHP = max;
                    this.CurrentHP = current;
                }

                if (this.MaxHP <= 0) this.MaxHP = 1;
                this.CurrentHP = Mathf.Clamp(this.CurrentHP, 0, this.MaxHP);
                this.UpdateVisuals();
            }
        }

        private void OnEnable()
        {
            this.TrySubscribe();
            this.RefreshImmediate();
        }

        private void OnDisable()
        {
            this.TryUnsubscribe();
        }

        private void LateUpdate()
        {
            // Polling fallback in case your controller doesn’t fire events for some changes
            if (this.UsePollingFallback && !this.EventDrivenUpdateReceived && this.TargetHP != null && (this.PropertyInfoCurrent != null || this.PropertyInfoMax != null))
            {
                int current, max;
                if (this.TryReadTarget(out current, out max))
                {
                    max = Mathf.Max(1, max);
                    current = Mathf.Clamp(current, 0, max);
                    if (current != this.CurrentHP || max != this.MaxHP)
                    {
                        this.CurrentHP = current; this.MaxHP = max;
                        this.UpdateVisuals();
                        if (!this.AlwaysVisible) this.ShowThenMaybeAutoHide();
                    }
                }
            }

            // Auto-hide timer
            if (!this.AlwaysVisible && this.HideAtTime > 0f && Time.time >= this.HideAtTime)
            {
                this.HideAtTime = -1f;
                this.FadeTo(0f, this.FadeDuration);
            }

            // Reset flag for next frame
            this.EventDrivenUpdateReceived = false;
        }

        private void TrySubscribe()
        {
            if (this.TargetHP == null) return;

            // Your exact event names/signatures:
            this.TargetHP.OnValueChanged += this.OnHealthChanged;
            this.TargetHP.OnDamageTaken += this.OnDamageTaken;
            this.TargetHP.OnHealed += this.OnHealedInternal;
            this.TargetHP.OnDeath += this.OnDeathInternal;
        }

        private void TryUnsubscribe()
        {
            if (this.TargetHP == null) return;

            this.TargetHP.OnValueChanged -= this.OnHealthChanged;
            this.TargetHP.OnDamageTaken -= this.OnDamageTaken;
            this.TargetHP.OnHealed -= this.OnHealedInternal;
            this.TargetHP.OnDeath -= this.OnDeathInternal;
        }

        // ---- Event handlers (int-based) ----
        private void OnHealthChanged(int current, int max)
        {
            this.EventDrivenUpdateReceived = true;

            this.MaxHP = Mathf.Max(1, max);
            this.CurrentHP = Mathf.Clamp(current, 0, this.MaxHP);
            this.UpdateVisuals();
            this.ShowThenMaybeAutoHide();
        }

        private void OnDamageTaken(int amount)
        {
            this.EventDrivenUpdateReceived = true;

            if (this.MutateFromDeltaEvents) // mutate only if you DON'T also get OnHealthChanged
            {
                this.CurrentHP = Mathf.Clamp(this.CurrentHP - Mathf.Max(0, amount), 0, this.MaxHP);
                this.UpdateVisuals();
            }
            this.ShowThenMaybeAutoHide();
        }

        private void OnHealedInternal(int amount)
        {
            this.EventDrivenUpdateReceived = true;

            if (this.MutateFromDeltaEvents) // mutate only if you DON'T also get OnHealthChanged
            {
                this.CurrentHP = Mathf.Clamp(this.CurrentHP + Mathf.Max(0, amount), 0, this.MaxHP);
                this.UpdateVisuals();
            }
            this.ShowThenMaybeAutoHide();
        }

        private void OnDeathInternal()
        {
            this.EventDrivenUpdateReceived = true;

            this.CurrentHP = 0;
            this.UpdateVisuals();
            this.ShowThenMaybeAutoHide(visibleOverrideSeconds: 0.5f);
        }

        // ---- UI helpers ----
        private void RefreshImmediate()
        {
            this.UpdateVisuals();
            if (this.AlwaysVisible)
            {
                this.SetAlpha(1f);
            }
            else
            {
                // Hidden if full, visible otherwise
                this.SetAlpha(this.NormalizedHPState < 0.999f ? 1f : 0f);
            }
        }

        private void UpdateVisuals()
        {
            if (this.BarSlider != null)
            {
                float t = this.NormalizedHPState;
                if (!Mathf.Approximately(this.BarSlider.value, t)) this.BarSlider.value = t;
            }

            if (this.HPText != null)
            {
                this.HPText.text = $"{this.CurrentHP} / {this.MaxHP}";
            }
        }

        private void ShowThenMaybeAutoHide(float? visibleOverrideSeconds = null)
        {
            if (this.AlwaysVisible)
            {
                this.SetAlpha(1f);
                return;
            }

            this.FadeTo(1f, this.FadeDuration);

            if (visibleOverrideSeconds.HasValue)
            {
                this.HideAtTime = Time.time + Mathf.Max(0f, visibleOverrideSeconds.Value);
                return;
            }

            bool isFull = this.CurrentHP >= this.MaxHP;
            this.HideAtTime = Time.time + (isFull ? 0.5f : this.AutoHideDelay);
        }

        private void FadeTo(float targetAlpha, float duration)
        {
            if (!this.isActiveAndEnabled) return;
            if (this.FadeRoutineRef != null) this.StopCoroutine(this.FadeRoutineRef);
            this.FadeRoutineRef = this.StartCoroutine(this.FadeRoutine(targetAlpha, duration));
        }

        private IEnumerator FadeRoutine(float target, float duration)
        {
            float start = this.CanvasGroup.alpha;
            if (Mathf.Approximately(start, target))
            {
                this.CanvasGroup.alpha = target;
                yield break;
            }

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = duration > 0f ? t / duration : 1f;
                this.CanvasGroup.alpha = Mathf.Lerp(start, target, k);
                yield return null;
            }
            this.CanvasGroup.alpha = target;
        }

        private void SetAlpha(float a)
        {
            if (this.FadeRoutineRef != null) 
            {
                this.StopCoroutine(this.FadeRoutineRef); 
                this.FadeRoutineRef = null; 
            }
            this.CanvasGroup.alpha = a;
        }

        // ---- Optional: initial read / polling via properties if they exist ----
        private bool TryReadTarget(out int current, out int max)
        {
            current = this.CurrentHP;
            max = this.MaxHP;

            if (this.TargetHP == null) return false;

            bool gotAny = false;

            if (this.PropertyInfoCurrent != null)
            {
                object v = this.PropertyInfoCurrent.GetValue(this.TargetHP, null);
                if (v is int ci) 
                {
                    current = ci; 
                    gotAny = true; 
                }
            }
            if (this.PropertyInfoMax != null)
            {
                object v = this.PropertyInfoMax.GetValue(this.TargetHP, null);
                if (v is int mi) 
                {
                    max = mi; 
                    gotAny = true; 
                }
            }
            return gotAny;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.CanvasGroup == null) this.CanvasGroup = GetComponent<CanvasGroup>();
            if (this.BarSlider != null)
            {
                this.BarSlider.minValue = 0f;
                this.BarSlider.maxValue = 1f;
                this.BarSlider.wholeNumbers = false;
                this.BarSlider.interactable = false;
                this.BarSlider.navigation = new Navigation 
                {
                    mode = Navigation.Mode.None 
                };
            }
        }
#endif
    }
}
