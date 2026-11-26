using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public class NPCDialoguePanelPlaybackHandler : NPCDialoguePanelSubHandlerBase
    {
        // --- Data / Configs ---
        //  1. Reading Duration
        [Header("Configs - Reading Duration")]
        [SerializeField, Min(1f)] private float _wordsPerMinute = 220f;
        [SerializeField, Min(0f)] private float _baseReadingDuration = 0.75f;
        [SerializeField, Min(0.1f)] private float _minReadingDuration = 2f;
        [SerializeField, Min(0.1f)] private float _maxReadingDuration = 20f;
        public float WordsPerMinute => _wordsPerMinute;
        public float BaseReadingDuration => _baseReadingDuration;
        public float MinReadingDuration => _minReadingDuration;
        public float MaxReadingDuration => _maxReadingDuration;

        // --- Runtime ---
        //  1. Playback State
        [Header("Runtime - Playback")]
        [SerializeField] private bool _isPlaying;
        [SerializeField] private int _currentSegmentIndex = -1;
        [SerializeField] private int _segmentCount;
        [SerializeField] private float _currentReadingDuration;
        public bool IsPlaying => _isPlaying;
        public int CurrentSegmentIndex => _currentSegmentIndex;
        public int SegmentCount => _segmentCount;
        public float CurrentReadingDuration => _currentReadingDuration;

        public bool IsFinalSegment => this.SegmentCount > 0 && this.CurrentSegmentIndex == this.SegmentCount - 1;

        //  2. Coroutine
        private Coroutine _playbackCoroutine;
        private int _playbackVersion;

        // --- Events ---
        public event Action<string> DialogueSegmentChanged;
        public event Action DialoguePlaybackCompleted;

        // --- Public APIs ---
        //  1. Playback Lifecycle
        public void StartPlayback(NPCDialogueNodeData node)
        {
            this.StopPlayback();

            if (node == null) return;

            IReadOnlyList<string> segments = node.DialogueSegments;

            _segmentCount = segments != null
                ? segments.Count
                : 0;

            if (this.SegmentCount == 0)
            {
                int playbackVersion = _playbackVersion;

                this.DialogueSegmentChanged?.Invoke(string.Empty);

                if (playbackVersion == _playbackVersion)
                {
                    this.DialoguePlaybackCompleted?.Invoke();
                }

                return;
            }

            _isPlaying = true;

            int version = _playbackVersion;

            Coroutine coroutine = this.StartCoroutine(this.PlayDialogueSegments(segments, version));

            if (version == _playbackVersion && this.IsPlaying)
            {
                _playbackCoroutine = coroutine;
            }
            else if (coroutine != null)
            {
                this.StopCoroutine(coroutine);
            }
        }

        public void StopPlayback()
        {
            _playbackVersion++;

            if (_playbackCoroutine != null)
            {
                this.StopCoroutine(_playbackCoroutine);

                _playbackCoroutine = null;
            }

            _isPlaying = false;
            _currentSegmentIndex = -1;
            _segmentCount = 0;
            _currentReadingDuration = 0f;
        }

        //  2. Reading Duration
        public float CalculateReadingDuration(string segment)
        {
            int wordCount = this.CountWords(segment);

            float wordsPerMinute = Mathf.Max(1f, this.WordsPerMinute);

            float readingDuration = this.BaseReadingDuration + (wordCount * 60f / wordsPerMinute);

            float minDuration = Mathf.Max(0.1f, this.MinReadingDuration);
            float maxDuration = Mathf.Max(minDuration, this.MaxReadingDuration);

            return Mathf.Clamp(readingDuration, minDuration, maxDuration);
        }

        // --- Helpers ---
        //  1. Playback
        private IEnumerator PlayDialogueSegments(IReadOnlyList<string> segments, int playbackVersion)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                if (playbackVersion != _playbackVersion) yield break;

                _currentSegmentIndex = i;

                string segment = segments[i] ?? string.Empty;

                _currentReadingDuration = this.CalculateReadingDuration(segment);

                this.DialogueSegmentChanged?.Invoke(segment);

                if (playbackVersion != _playbackVersion) yield break;

                yield return new WaitForSecondsRealtime(this.CurrentReadingDuration);
            }

            if (playbackVersion != _playbackVersion) yield break;

            _isPlaying = false;
            _currentReadingDuration = 0f;
            _playbackCoroutine = null;

            this.DialoguePlaybackCompleted?.Invoke();
        }

        //  2. Reading Duration
        private int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            return text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        // --- FOTLMonoBehaviour Overrides ---
        protected override void CleanupStates()
        {
            this.StopPlayback();

            base.CleanupStates();
        }
    }
}