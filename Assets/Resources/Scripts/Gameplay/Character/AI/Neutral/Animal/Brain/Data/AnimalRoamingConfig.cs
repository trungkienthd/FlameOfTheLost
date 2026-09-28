using UnityEngine;

namespace FlameOfTheLost
{
    [CreateAssetMenu(fileName = "Chr_AI_Ntr_Animal_Roam_New", menuName = "Data/FlameOfTheLost/Character/AI/Neutral/Animal/Brain/RoamingConfig")]
    public class AnimalRoamingConfig : CharacterDataAsset
    {
        // --- Passive State Durations ---
        //  1. Idle
        [Header("Idle Duration")]
        [SerializeField, Min(0f)] private float _minIdleDuration = 2f;
        [SerializeField, Min(0f)] private float _maxIdleDuration = 5f;
        public float MinIdleDuration => _minIdleDuration;
        public float MaxIdleDuration => _maxIdleDuration;

        //  2. Wandering
        [Header("Wandering Duration")]
        [SerializeField, Min(0f)] private float _minWalkDuration = 3f;
        [SerializeField, Min(0f)] private float _maxWalkDuration = 7f;
        public float MinWalkDuration => _minWalkDuration;
        public float MaxWalkDuration => _maxWalkDuration;

        //  3. Graze
        [Header("Graze Duration")]
        [SerializeField, Min(0f)] private float _minGrazeDuration = 3f;
        [SerializeField, Min(0f)] private float _maxGrazeDuration = 7f;
        public float MinGrazeDuration => _minGrazeDuration;
        public float MaxGrazeDuration => _maxGrazeDuration;

        //  4. Lie Down
        [Header("Lie Down Duration")]
        [SerializeField, Min(0f)] private float _lieDownDuration = 1f;
        public float LieDownDuration => _lieDownDuration;

        //  5. Rest / Sleep Branch
        [Header("Rest / Sleep Branch")]
        [SerializeField, Range(0f, 1f)] private float _restChance = 0.7f;
        public float RestChance => _restChance;
        public float SleepChance => 1f - this.RestChance;

        //  6. Rest
        [Header("Rest Duration")]
        [SerializeField, Min(0f)] private float _minRestDuration = 3f;
        [SerializeField, Min(0f)] private float _maxRestDuration = 6f;
        public float MinRestDuration => _minRestDuration;
        public float MaxRestDuration => _maxRestDuration;

        //  6. Sleep
        [Header("Sleep Duration")]
        [SerializeField, Min(0f)] private float _minSleepDuration = 8f;
        [SerializeField, Min(0f)] private float _maxSleepDuration = 15f;
        public float MinSleepDuration => _minSleepDuration;
        public float MaxSleepDuration => _maxSleepDuration;

        //  7. Stand Up
        [Header("Stand Up Duration")]
        [SerializeField, Min(0f)] private float _standUpDuration = 1f;
        public float StandUpDuration => _standUpDuration;

        // --- Public APIs ---
        public float GetRandomIdleDuration()
        {
            return this.GetRandomDuration(this.MinIdleDuration, this.MaxIdleDuration);
        }
        public float GetRandomWalkDuration()
        {
            return this.GetRandomDuration(this.MinWalkDuration, this.MaxWalkDuration);
        }
        public float GetRandomGrazeDuration()
        {
            return this.GetRandomDuration(this.MinGrazeDuration, this.MaxGrazeDuration);
        }
        public float GetRandomRestDuration()
        {
            return this.GetRandomDuration(this.MinRestDuration, this.MaxRestDuration);
        }
        public float GetRandomSleepDuration()
        {
            return this.GetRandomDuration(this.MinSleepDuration, this.MaxSleepDuration);
        }
        public bool ShouldRestAfterLieDown()
        {
            return Random.value < this.RestChance;
        }

        // --- Helpers ---
        private float GetRandomDuration(float min, float max)
        {
            return UnityEngine.Random.Range
            (
                Mathf.Min(min, max),
                Mathf.Max(min, max)
            );
        }
    }
}