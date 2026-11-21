using UnityEngine;

namespace FlameOfTheLost
{
    public class TransformDebugger : MonoBehaviour
    {
        private Vector3 _lastPosition;

        private void Awake()
        {
            _lastPosition = transform.position;
            this.LogPosition(nameof(this.Awake));
        }

        private void OnEnable()
        {
            this.LogPosition(nameof(this.OnEnable));
        }

        private void Start()
        {
            this.LogPosition(nameof(this.Start));
        }

        private void Update()
        {
            this.CheckPosition(nameof(this.Update));
        }

        private void FixedUpdate()
        {
            this.CheckPosition(nameof(this.FixedUpdate));
        }

        private void LateUpdate()
        {
            this.CheckPosition(nameof(this.LateUpdate));
        }

        private void CheckPosition(string phase)
        {
            if (this.transform.position == _lastPosition) return;

            Debug.LogWarning
            (
                $"[PLAYER POSITION CHANGED] {phase}\n" +
                $"Frame: {Time.frameCount}\n" +
                $"Before: {_lastPosition}\n" +
                $"After: {this.transform.position}\n" +
                $"Delta: {this.transform.position - _lastPosition}",
                this.gameObject
            );

            _lastPosition = this.transform.position;
        }

        private void LogPosition(string phase)
        {
            Debug.Log
            (
                $"[PLAYER POSITION] {phase} | " +
                $"Frame={Time.frameCount} | " +
                $"Position={this.transform.position}",
                this.gameObject
            );

            _lastPosition = this.transform.position;
        }
    }
}