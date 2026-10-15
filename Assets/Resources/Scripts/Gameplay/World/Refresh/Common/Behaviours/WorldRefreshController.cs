using System.Linq;
using UnityEngine;

namespace FlameOfTheLost
{
    public class WorldRefreshController : FOTLMonoBehaviour
    {
        public void RefreshForPlayerRespawn()
        {
            this.RefreshWorld
            (
                new WorldRefreshContext(isSaveReload: false, isPlayerRespawn: true)
            );
        }

        public void RefreshForSaveReload()
        {
            this.RefreshWorld
            (
                new WorldRefreshContext(isSaveReload: true, isPlayerRespawn: false)
            );
        }

        private void RefreshWorld(WorldRefreshContext context)
        {
            MonoBehaviour[] behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            IWorldRefreshable[] refreshables = behaviours
                .OfType<IWorldRefreshable>()
                .OrderBy(refreshable => refreshable.WorldRefreshPriority)
                .ToArray();

            for (int i = 0; i < refreshables.Length; i++)
            {
                refreshables[i].RefreshWorldContent(context);
            }
        }
    }
}