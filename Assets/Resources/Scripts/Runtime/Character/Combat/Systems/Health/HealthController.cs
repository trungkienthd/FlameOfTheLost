using System;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class HealthController : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;
        [SerializeField] private bool _dead;
        [SerializeField] private CharacterFaction _characterFaction = CharacterFaction.Neutral;
        public int MaxHealth => _maxHealth;
        public int CurrentHealth
        {
            get => _currentHealth;
            private set => _currentHealth = Mathf.Clamp(value, 0, this.MaxHealth);
        }
        public bool Dead
        {
            get => _dead;
            private set => _dead = value;
        }
        public CharacterFaction CharacterFaction => _characterFaction;

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
        public event Action<int, int> OnHealthChanged;
        public event Action<int> OnDamageTaken;
        public event Action<int> OnHealed;
        public event Action OnDeath;

        private void Awake()
        {
            this.AnimationHandler = this.GetComponentInChildren<CharacterAnimationHandler>();
            this.KnockbackHandler = this.GetComponent<KnockbackHandler>();
            this.Rigidbody2D = this.GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            this.Dead = false;
            this.CurrentHealth = Mathf.Clamp(this.CurrentHealth <= 0 ? this.MaxHealth : this.CurrentHealth, 0, this.MaxHealth);
            this.OnHealthChanged?.Invoke(this.CurrentHealth, this.MaxHealth);
        }

        public void TakeDamage(int damageAmount, Vector2 hitDir, float knockbackForce = 0f)
        {
            if (this.Dead) return;

            int dmgAmount = Mathf.RoundToInt(Mathf.Max(0, damageAmount) * this.IncomingDamageMultiplier);
            if (dmgAmount == 0) return;

            // Implement damage logic here
            // Debug.Log($"Taking {dmgAmount} damage from direction {hitDir} with knockback force {knockbackForce}");

            this.CurrentHealth = Mathf.Max(0, this.CurrentHealth - dmgAmount);
            this.OnDamageTaken?.Invoke(dmgAmount);
            this.OnHealthChanged?.Invoke(this.CurrentHealth, this.MaxHealth);

            if (knockbackForce > 0f) this.KnockbackHandler.ApplyKnockback(hitDir, knockbackForce);

            if (this.CurrentHealth <= 0)
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
                Debug.Log($"Current health: {this.CurrentHealth}");
            }
        }

        public void Heal(int amount)
        {
            if (this.Dead) return;

            int healAmount = Mathf.Max(0, amount);
            if (healAmount == 0) return;

            int beforeHealHealth = this.CurrentHealth;
            this.CurrentHealth = Mathf.Min(this.MaxHealth, this.CurrentHealth + healAmount);
            
            int deltaHealth = this.CurrentHealth - beforeHealHealth;
            if (deltaHealth > 0)
            {
                this.OnHealed?.Invoke(deltaHealth);
                this.OnHealthChanged?.Invoke(this.CurrentHealth, this.MaxHealth);
            }

            Debug.Log($"Healed {healAmount}. Current health: {this.CurrentHealth}");
        }

        public void Kill()
        {
            if (this.Dead) return;

            this.CurrentHealth = 0;
            this.Dead = true;

            this.AnimationHandler?.SetDead(true);
            if (this.Rigidbody2D != null) this.Rigidbody2D.linearVelocity = Vector2.zero; // Stop movement on death

            this.OnHealthChanged?.Invoke(this.CurrentHealth, this.MaxHealth);
            this.OnDeath?.Invoke();
        }

        // Optional Debug
        public void TestHit()
        {
            this.TakeDamage(5, Vector2.left, 4f);
        }
    }
}

