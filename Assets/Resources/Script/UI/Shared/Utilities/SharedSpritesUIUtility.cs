using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class SharedSpritesUIUtility
    {
        private const string ROYAL_GREEN_SPRITES_PATH = "Arts/Sprites/UI/NRBGUI_RoyalGreen";
        private const string DIGIT_PREFIX = "NRBGUI_Number_";

        private static Dictionary<string, Sprite> _spriteNameMap;
        private static Dictionary<int, Sprite> _digitSpriteMap;
        private static bool _isInitialized = false;

        private static void Initialize()
        {
            if (_isInitialized) return;

            _spriteNameMap = new Dictionary<string, Sprite>();
            _digitSpriteMap = new Dictionary<int, Sprite>(10);

            Sprite[] sprites = Resources.LoadAll<Sprite>(ROYAL_GREEN_SPRITES_PATH);
            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogError($"[SharedSpritesUIUtility] No sprites found at path: {ROYAL_GREEN_SPRITES_PATH}");
                _isInitialized = true;
                return;
            }

            foreach (Sprite sprite in sprites)
            {
                if (sprite == null) continue;

                _spriteNameMap[sprite.name] = sprite;

                if (sprite.name.StartsWith(DIGIT_PREFIX))
                {
                    string digitString = sprite.name.Substring(DIGIT_PREFIX.Length);

                    if (int.TryParse(digitString, out int digit) && digit >= 0 && digit <= 9)
                    {
                        _digitSpriteMap[digit] = sprite;
                    }
                }
            }

            // Validation (important for debugging atlas issues)
            for (int i = 0; i <= 9; i++)
            {
                if (!_digitSpriteMap.ContainsKey(i))
                {
                    Debug.LogError($"[SharedSpritesUIUtility] Missing digit sprite: {i}");
                }
            }

            _isInitialized = true;
        }

        public static Sprite GetSprite(string spriteName)
        {
            if (string.IsNullOrWhiteSpace(spriteName))
            {
                Debug.LogError("[SharedSpritesUIUtility] Sprite name is null or empty.");
                return null;
            }

            SharedSpritesUIUtility.Initialize();

            if (_spriteNameMap.TryGetValue(spriteName, out Sprite sprite))
            {
                return sprite;
            }

            Debug.LogError($"[SharedSpritesUIUtility] Sprite '{spriteName}' not found in sheet '{ROYAL_GREEN_SPRITES_PATH}'.");
            return null;
        }

        /// <summary>
        /// Get sprite for a single digit (0–9)
        /// </summary>
        public static Sprite GetDigitSprite(int digit)
        {
            if (digit < 0 || digit > 9)
            {
                Debug.LogError($"[SharedSpritesUIUtility] Invalid digit: {digit}");
                return null;
            }

            SharedSpritesUIUtility.Initialize();

            return _digitSpriteMap.TryGetValue(digit, out var sprite) ? sprite : null;
        }

        /// <summary>
        /// Get sprites for a number (0–99)
        /// Returns 1 or 2 sprites depending on value
        /// </summary>
        public static List<Sprite> GetNumberSprites(int number)
        {
            SharedSpritesUIUtility.Initialize();

            List<Sprite> result = new List<Sprite>(2);

            if (number < 0) number = 0;
            if (number > 99) number = 99;

            if (number < 10)
            {
                result.Add(SharedSpritesUIUtility.GetDigitSprite(number));
            }
            else
            {
                int tens = number / 10;
                int ones = number % 10;

                result.Add(SharedSpritesUIUtility.GetDigitSprite(tens));
                result.Add(SharedSpritesUIUtility.GetDigitSprite(ones));
            }

            return result;
        }
    }
}