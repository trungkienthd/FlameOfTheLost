using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HealthController : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int _baseMaxHP = 100;
        [SerializeField] private int _maxHP = 100;
        [SerializeField] private int _currentHP;
        [SerializeField] private bool _dead;
        [SerializeField] private CharacterFactionType _characterFaction = CharacterFactionType.Neutral;
        public int BaseMaxHP => _baseMaxHP;
        public int MaxHP
        {
            get => _maxHP;
            private set => _maxHP = Mathf.Max(0, value);
        }
        public int CurrentHP
        {
            get => _currentHP;
            private set => _currentHP = Mathf.Clamp(value, 0, this.MaxHP);
        }
        public bool Dead
        {
            get => _dead;
            private set => _dead = value;
        }
        public CharacterFactionType CharacterFaction => _characterFaction;

        [Header("Status Effects Modifiers")]
        [SerializeField] private float _incomingDamageMultiplier = 1f;
        public float IncomingDamageMultiplier
        {
            get => _incomingDamageMultiplier;
            set => _incomingDamageMultiplier = Mathf.Max(0f, value);
        }

        [Header("References")]
        [SerializeField] private CharacterAnimationHandler _animationHandler;
        [SerializeField] private KnockbackHandler _knockbackHandler;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        public CharacterAnimationHandler AnimationHandler
        {
            get => _animationHandler;
            private set => _animationHandler = value;
        }
        public KnockbackHandler KnockbackHandler
        {
            get => _knockbackHandler;
            private set => _knockbackHandler = value;
        }
        public Rigidbody2D Rigidbody2D
        {
            get => _rigidbody2D;
            private set => _rigidbody2D = value;
        }

        // Events:
        public event Action<int, int>   OnHPChanged;
        public event Action<int>        OnDamageTaken;
        public event Action<int>        OnHealed;
        public event Action             OnDeath;
        public event Action<int>        OnMaxHPChanged;

        private void Awake()
        {
            this.AnimationHandler   = this.GetComponentInChildren<CharacterAnimationHandler>();
            this.KnockbackHandler   = this.GetComponent<KnockbackHandler>();
            this.Rigidbody2D        = this.GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            this.Dead = false;
            this.CurrentHP = Mathf.Clamp(this.CurrentHP <= 0 ? this.MaxHP : this.CurrentHP, 0, this.MaxHP);
            this.OnHPChanged?.Invoke(this.CurrentHP, this.MaxHP);
        }

        // --- Effective Max HP Management API ---
        public int GetEffectiveMaxDelta()
        {
            return this.MaxHP - this.BaseMaxHP;
        }

        public void SetEffectiveMaxDelta(int delta, bool keepCurrentRatio = false)
        {
            int oldMaxHP = this.MaxHP;
            int newMaxHP = Mathf.Max(0, this.BaseMaxHP + delta);

            if (keepCurrentRatio && oldMaxHP > 0)
            {
                float healthRatio = (float)this.CurrentHP / (float)oldMaxHP;
                this.SetMaxAndCurrentHPs(newMaxHP, Mathf.RoundToInt(newMaxHP * healthRatio));
            }
            else
            {
                this.SetMaxHPAndClampCurrentHP(newMaxHP);
            }
        }

        // Add to the effective delta (convinience)
        public void AddEffectiveMaxDelta(int delta, bool keepCurrentRatio = false)
        {
            int currentDelta = this.GetEffectiveMaxDelta();
            this.SetEffectiveMaxDelta(currentDelta + delta, keepCurrentRatio);
        }

        // Set Max HP and Current HP directly
        public void SetMaxAndCurrentHPs(int newMaxHP, int newCurrentHP)
        {
            int clampedMaxHP = Mathf.Max(0, newMaxHP);
            this.MaxHP = clampedMaxHP;

            int beforeChangeHP = this.CurrentHP;
            this.CurrentHP = Mathf.Clamp(newCurrentHP, 0, clampedMaxHP);

            this.OnMaxHPChanged?.Invoke(this.MaxHP);
            if (this.CurrentHP != beforeChangeHP)
            {
                this.OnHPChanged?.Invoke(this.CurrentHP, this.MaxHP);
            }

            if (this.CurrentHP <= 0 && !this.Dead)
            {
                this.Dead = true;
                this.AnimationHandler?.SetDead(true);
                if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero; // Stop movement on death
                this.OnDeath?.Invoke();
            }
        }

        // Set Max HP and clamp Current HP if needed
        public void SetMaxHPAndClampCurrentHP(int newMaxHP)
        {
            int clampedMaxHP = Mathf.Max(0, newMaxHP);
            this.MaxHP = clampedMaxHP;

            int beforeChangeHP = this.CurrentHP;
            this.CurrentHP = Mathf.Min(this.CurrentHP, clampedMaxHP);

            this.OnMaxHPChanged?.Invoke(this.MaxHP);
            if (this.CurrentHP != beforeChangeHP)
            {
                this.OnHPChanged?.Invoke(this.CurrentHP, this.MaxHP);
            }

            if (this.CurrentHP <= 0 && !this.Dead)
            {
                this.Dead = true;
                this.AnimationHandler?.SetDead(true);
                if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero; // Stop movement on death
                this.OnDeath?.Invoke();
            }
        }

        // --- Incoming-damage Management API ---
        public int ComputeDamageTaken(int rawDamage)
        {
            float scaledDamage = Mathf.Max(0, rawDamage) * this.IncomingDamageMultiplier;
            return Mathf.RoundToInt(scaledDamage);
        }

        public float ApplyIncomingDamageMultiplier(float rawDamage)
        {
            return Mathf.Max(0f, rawDamage) * this.IncomingDamageMultiplier;
        }

        // --- Combat APIs ---
        public void TakeDamage(int damageAmount, Vector2 hitDir, float knockbackForce = 0f)
        {
            if (this.Dead) return;

            int computedDmgAmount = this.ComputeDamageTaken(damageAmount);
            if (computedDmgAmount == 0) return;

            this.CurrentHP = Mathf.Max(0, this.CurrentHP - computedDmgAmount);
            this.OnDamageTaken?.Invoke(computedDmgAmount);
            this.OnHPChanged?.Invoke(this.CurrentHP, this.MaxHP);

            if (knockbackForce > 0f) this.KnockbackHandler.ApplyKnockback(hitDir, knockbackForce);

            if (this.CurrentHP <= 0)
            {
                this.Dead = true;
                this.AnimationHandler?.SetDead(true);

                // Stop movement on death
                this.Rigidbody2D.linearVelocity = Vector2.zero;

                this.OnDeath?.Invoke();
                Debug.Log("Entity is dead.");

                return;
            }
            else
            {
                this.AnimationHandler.TriggerHit();
                Debug.Log($"Current health: {this.CurrentHP}");
            }
        }

        public void Heal(int amount)
        {
            if (this.Dead) return;

            int healAmount = Mathf.Max(0, amount);
            if (healAmount == 0) return;

            int beforeHealHealth = this.CurrentHP;
            this.CurrentHP = Mathf.Min(this.MaxHP, this.CurrentHP + healAmount);
            
            int deltaHealth = this.CurrentHP - beforeHealHealth;
            if (deltaHealth > 0)
            {
                this.OnHealed?.Invoke(deltaHealth);
                this.OnHPChanged?.Invoke(this.CurrentHP, this.MaxHP);
            }

            Debug.Log($"Healed {healAmount}. Current health: {this.CurrentHP}");
        }

        public void Kill()
        {
            if (this.Dead) return;

            this.CurrentHP = 0;
            this.Dead = true;

            this.AnimationHandler?.SetDead(true);
            if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero; // Stop movement on death

            this.OnHPChanged?.Invoke(this.CurrentHP, this.MaxHP);
            this.OnDeath?.Invoke();
        }

        // Optional Debug
        public void TestHit()
        {
            this.TakeDamage(5, Vector2.left, 4f);
        }
    }
}

