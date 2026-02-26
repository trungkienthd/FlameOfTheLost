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

        // Save the dictionary to lists
        public void OnBeforeSerialize()
        {
            this.KeysList.Clear();
            this.ValuesList.Clear();
            foreach (var kvp in this)
            {
                this.KeysList.Add(kvp.Key);
                this.ValuesList.Add(kvp.Value);
            }
        }

        // Load dictionary from lists
        public void OnAfterDeserialize()
        {
            this.Clear();

            if (this.KeysList.Count != this.ValuesList.Count)
                throw new System.Exception($"There are {this.KeysList.Count} keys and {this.ValuesList.Count} values after deserialization. Make sure that both key and value types are serializable.");

            for (int i = 0; i < this.KeysList.Count; i++)
            {
                this[this.KeysList[i]] = this.ValuesList[i];
            }
        }
    }
}
