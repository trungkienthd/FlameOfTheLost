using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlameOfTheLost
{
    public sealed class GameConditionSceneHandler : GameConditionSubBehaviourBase
    {
        // --- Configs ---
        private const string MAIN_MENU_SCENE_NAME = "MainMenu";

        // --- Helpers ---
        //  1. Scene Change Handling
        private void HandleActiveSceneChanged(Scene previousScene, Scene nextScene)
        {
            if (!this.EnsureBound()) return;

            GameConditionController controller = this.GameConditionController;

            if (GameConditionController.Instance != controller) return;

            // a. Invalidate gameplay references from the previous scene.
            controller.HandleGameplaySceneChanged();

            // b. Returning to the Main Menu ends the gameplay session.
            if (nextScene.name == MAIN_MENU_SCENE_NAME)
            {
                UnityEngine.Object.Destroy(controller.gameObject);
            }
        }

        // --- FOTLSubBehaviour Overrides ---
        protected override void OnBound(GameConditionController brainBehaviour)
        {
            base.OnBound(brainBehaviour);

            SceneManager.activeSceneChanged += this.HandleActiveSceneChanged;
        }
        protected override void OnUnbinding(GameConditionController brainBehaviour)
        {
            SceneManager.activeSceneChanged -= this.HandleActiveSceneChanged;

            base.OnUnbinding(brainBehaviour);
        }
    }
}