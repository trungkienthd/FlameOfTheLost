using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();
        private List<TKey> KeysList => _keys;
        private List<TValue> ValuesList => _values;

        // --- ISerializationCallbackReceiver ---
        //  1. Save the dictionary to lists
        public void OnBeforeSerialize()
        {
            this.KeysList.Clear();
            this.ValuesList.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                this.KeysList.Add(pair.Key);
                this.ValuesList.Add(pair.Value);
            }
        }

        //  2. Load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (this.KeysList.Count != this.ValuesList.Count)
            {
                Debug.LogError($"[{nameof(SerializableDictionary<TKey, TValue>)}] Serialized key/value count mismatch. Keys={this.KeysList.Count}, Values={this.ValuesList.Count}.");
                return;
            }

            for (int i = 0; i < this.KeysList.Count; i++)
            {
                this[this.KeysList[i]] = this.ValuesList[i];
            }
        }
    }
}
