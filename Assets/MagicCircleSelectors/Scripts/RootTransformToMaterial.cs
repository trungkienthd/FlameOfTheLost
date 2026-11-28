using UnityEngine;

namespace CircleSelector
{
    [ExecuteInEditMode]
    public class RootTransformToMaterial : MonoBehaviour
    {
        [SerializeField]
        Material mainMaterial;

        [SerializeField]
        bool usePropertyBlock;

        SpriteRenderer[] spriteRenderers;
        MaterialPropertyBlock propBlock;

        // Start is called before the first frame update
        void OnEnable()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            propBlock = new MaterialPropertyBlock();
        }

        // Update is called once per frame
        void Update()
        {
            if (spriteRenderers != null && spriteRenderers.Length > 0)
            {
                for (int i = 0; i < spriteRenderers.Length; i++)
                {
                    if (!usePropertyBlock && Application.isPlaying)
                    {
                        spriteRenderers[i].material.SetVector("_RootTransform", transform.position);
                    }
                    else
                    {
                        spriteRenderers[i].GetPropertyBlock(propBlock);
                        propBlock.SetVector("_RootTransform", transform.position);
                        spriteRenderers[i].SetPropertyBlock(propBlock);
                    }
                }
            }
        }
    }
}
