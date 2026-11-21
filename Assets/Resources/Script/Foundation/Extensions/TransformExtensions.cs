using System.Text;
using UnityEngine;

namespace FlameOfTheLost
{
    public static class TransformExtensions
    {
        public static string GetHierarchyPath(this Transform t)
        {
            var stringBuilder = new StringBuilder(t.name);
            while (t.parent != null)
            {
                t = t.parent;
                stringBuilder.Insert(0, t.name + "/");
            }
            return stringBuilder.ToString();
        }
    }
}
