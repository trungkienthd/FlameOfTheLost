using UnityEngine;

namespace FlameOfTheLost
{
    public class PlayerPhysicsDebugger : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.LogWarning
            (
                $"[Player Physics] Collision entered with " +
                $"'{collision.collider.name}' " +
                $"on GameObject '{collision.gameObject.name}'.",
                collision.collider
            );
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Debug.Log
                (
                    $"[Player Physics] Contact with '{collision.collider.name}' | " +
                    $"Point={contact.point} | " +
                    $"Normal={contact.normal}",
                    collision.collider
                );
            }
        }
    }
}
