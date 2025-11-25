using UnityEngine;

namespace FlameOfTheLost
{
    public class DamageReceivingHandler : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private HealthController _healthController;
        private HealthController HealthController
        {
            get => _healthController;
            set => _healthController = value;
        }

        private void Awake()
        {
            if (!this.HealthController) this.HealthController = this.GetComponentInParent<HealthController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this.HealthController == null || this.HealthController.Dead) return; 

            if (other.TryGetComponent<IDamager>(out var damagerObj))
            {
                var damager = damagerObj;
                if (!this.HealthController || !(this.HealthController is IDamageable target)) return;

                // Character Faction filter
                if (damager is DamageDealingHandler dd && !dd.CanHit(target)) return;

                // Direction = from damager origin to this
                Vector2 dir = (this.transform.position - damager.OriginTransform.position).normalized;
                target.TakeDamage(damager.DamageAmount, dir, damager.KnockbackForce);

                //Debug.Log($"[DRH] ENTER: other={other.name} (path={other.transform.GetHierarchyPath()}) " +
                //          $"this={this.name} (path={this.transform.GetHierarchyPath()})");

                // Notify damager
                if (damager is DamageDealingHandler dd2) dd2.OnSuccessfulHit();
            }
        }
    }
}
