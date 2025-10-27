using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class SpriteLoader
    {
        private static readonly Dictionary<string, Sprite> _cache = new();
        private static readonly Dictionary<string, Sprite[]> _sheetCache = new();

        private static Dictionary<string, Sprite> Cache => _cache;
        private static Dictionary<string, Sprite[]> SheetCache => _sheetCache;

        public static Sprite Get(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) return null;
            if (SpriteLoader.Cache.TryGetValue(resourcePath, out var s)) return s;

            s = Resources.Load<Sprite>(resourcePath);
            if (s != null) SpriteLoader.Cache[resourcePath] = s;

            return s;
        }

        public static Sprite[] GetSheet(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath)) return null;

            if (SpriteLoader.SheetCache.TryGetValue(resourcePath, out var arr))
                return arr;

            arr = Resources.LoadAll<Sprite>(resourcePath);
            if (arr is { Length: > 0 }) SpriteLoader.SheetCache[resourcePath] = arr;

            return arr;
        }
    }
}