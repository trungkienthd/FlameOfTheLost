using System.Collections;
using UnityEngine;

namespace FlameOfTheLost
{
    public class MainMenuTransitionController : MainMenuSubBehaviourBase
    {
        // --- References ---
        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _loadingContainer;
        private CanvasGroup CanvasGroup => _canvasGroup;
        private GameObject LoadingContainer => _loadingContainer;

        // --- Configs ---
        [Header("Configs")]
        [SerializeField, Min(0f)] private float _fadeDuration = 0.5f;
        private float FadeDuration => _fadeDuration;

        // --- Public APIs ---
        public IEnumerator FadeOutRoutine()
        {
            if (this.CanvasGroup == null)
            {
                Debug.LogError($"[{nameof(MainMenuTransitionController)}] CanvasGroup reference is missing.", this);
                yield break;
            }

            this.CanvasGroup.alpha = 0f;
            this.CanvasGroup.interactable = false;
            this.CanvasGroup.blocksRaycasts = true;

            if (this.LoadingContainer != null) this.LoadingContainer.SetActive(false);

            this.gameObject.SetActive(true);

            float elapsed = 0f;
            while (elapsed < this.FadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = this.FadeDuration > 0f
                    ? Mathf.Clamp01(elapsed / this.FadeDuration)
                    : 1f;

                this.CanvasGroup.alpha = progress;

                yield return null;
            }

            this.CanvasGroup.alpha = 1f;
        }

        public void ShowLoading()
        {
            this.gameObject.SetActive(true);

            if (this.CanvasGroup != null)
            {
                this.CanvasGroup.alpha = 1f;
                this.CanvasGroup.blocksRaycasts = true;
            }

            if (this.LoadingContainer != null) this.LoadingContainer.SetActive(true);
        }

        public void Hide()
        {
            if (this.LoadingContainer != null) this.LoadingContainer.SetActive(false);

            this.gameObject.SetActive(false);
        }

        // --- FOTLMonoBehaviour Overrides ---

        protected override void CacheInternalReferences()
        {
            base.CacheInternalReferences();

            if (_canvasGroup == null) _canvasGroup = this.GetComponent<CanvasGroup>();

            if (_loadingContainer == null)
            {
                Transform target = this.transform.Find("LoadingContainer");
                if (target != null) _loadingContainer = target.gameObject;
            }
        }
        protected override void ValidateInternalReferences()
        {
            base.ValidateInternalReferences();

            if (_canvasGroup == null)
            {
                Debug.LogError($"[{nameof(MainMenuTransitionController)}] {nameof(CanvasGroup)} reference is missing.", this);
            }

            if (_loadingContainer == null)
            {
                Debug.LogError($"[{nameof(MainMenuTransitionController)}] {nameof(LoadingContainer)} reference is missing.", this);
            }
        }
        protected override void InitializeInternalState()
        {
            base.InitializeInternalState();

            this.Hide();
        }
    }
}