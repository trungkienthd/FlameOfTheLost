using System;
using System.IO;
using UnityEngine;

namespace FlameOfTheLost
{
    public class GameSaveFileRepository
    {
        // --- Constants ---
        private const string SaveFileName = "savegame.json";

        // --- Properties ---
        private string SavePath => Path.Combine(Application.persistentDataPath, GameSaveFileRepository.SaveFileName);

        // --- Public APIs ---
        //  1. Save
        public bool Save(GameSaveData data)
        {
            if (data == null) return false;

            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(this.SavePath, json);

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
        }

        //  2. Load
        public bool TryLoad(out GameSaveData data)
        {
            data = null;

            if (!File.Exists(this.SavePath)) return false;

            try
            {
                string json = File.ReadAllText(this.SavePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.LogWarning($"[{nameof(GameSaveFileRepository)}] Save file is empty.");
                    return false;
                }

                data = JsonUtility.FromJson<GameSaveData>(json);

                return data != null;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return false;
            }
        }

        //  3. Query
        public bool HasSave()
        {
            return File.Exists(this.SavePath);
        }

        //  4. Delete
        public void DeleteSave()
        {
            if (!File.Exists(this.SavePath)) return;

            try
            {
                File.Delete(this.SavePath);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
