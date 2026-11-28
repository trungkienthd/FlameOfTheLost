using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleSelector
{
    public class Selector : MonoBehaviour
    {
        [SerializeField]
        bool colorAllSprites = true;

        [SerializeField]
        Color selectorColor;

        Color prevColor;

        [SerializeField]
        Transform scaleParentOverride;

        [SerializeField]
        bool usePropertyBlock = true;

        [SerializeField]
        float selectedTransitionTime = .5f;

        [SerializeField]
        float selectedTime = 2f;

        [SerializeField]
        float unselectedIntensity = 1;

        [SerializeField]
        float selectedInstensity = 2;

        bool selected;

        Renderer[] renderers;
        SpriteRenderer[] spriteRenderers;
        MaterialPropertyBlock propBlock;

        Transform parent;
        Mesh parentMesh;
        MeshRenderer parentRenderer;
        Vector3 texScale = Vector3.one;

        Coroutine speedCoroutine;

        Animator anim;

        // Start is called before the first frame update
        void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            propBlock = new MaterialPropertyBlock();
            anim = GetComponent<Animator>();

            prevColor = selectorColor;

            UpdateColors();
            GetParent();
            ScaleToParent();
            CenterPosition();
        }

        private void OnEnable()
        {
            SetSelectedState();
            SetSpeed();
        }

        private void Update()
        {
            if (colorAllSprites && prevColor != selectorColor)
            {
                UpdateColors();
            }
        }

        void UpdateColors()
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = selectorColor;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sharedMaterial.SetColor("_MainColor", selectorColor);
            }

            prevColor = selectorColor;
        }

        private void OnTransformParentChanged()
        {
            UpdateParent();
        }

        IEnumerator SpeedTransition()
        {
            float timer = 0;
            float startTime = selected ? 1f : selectedTime;
            float endTime = selected ? selectedTime : 1f;

            while (timer <= selectedTransitionTime)
            {
                anim.SetFloat("RotationSpeed", Mathf.Lerp(startTime, endTime, timer / selectedTransitionTime));

                yield return null;
                timer += Time.deltaTime;
            }
        }

        void SetSpeed()
        {
            anim.SetFloat("RotationSpeed", selected ? selectedTime : 1f);
        }

        public void UpdateParent()
        {
            GetParent();
            ScaleToParent();
            CenterPosition();
        }

        public void Select()
        {
            if (gameObject.activeSelf)
            {
                selected = !selected;
                SetSelectedState();
                StartSpeedTransition();
            }
            
        }

        void SetSelectedState()
        {
            anim.SetBool("Selected", selected);
            SetIntensity();
            
        }

        void StartSpeedTransition()
        {
            if (speedCoroutine != null)
            {
                StopCoroutine(speedCoroutine);
            }

            speedCoroutine = StartCoroutine(SpeedTransition());
        }

        void SetIntensity()
        {
            float intensity = selected ? selectedInstensity : unselectedIntensity;
            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (!usePropertyBlock && Application.isPlaying)
                    {
                        renderers[i].material.SetFloat("_Intensity", intensity);
                    }
                    else
                    {
                        renderers[i].GetPropertyBlock(propBlock);
                        propBlock.SetFloat("_Intensity", intensity);
                        renderers[i].SetPropertyBlock(propBlock);
                    }
                }
            }
        }

        void ScaleToParent()
        {
            if (parentRenderer != null)
            {
                float maxScale = Mathf.Max(parentRenderer.bounds.size.x, parentRenderer.bounds.size.z);

                transform.localScale = Vector3.one * maxScale;
                texScale.x = 1 / maxScale;
                texScale.y = 1;
                texScale.z = 1 / maxScale;

                if (renderers != null && renderers.Length > 0)
                {
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (!usePropertyBlock && Application.isPlaying)
                        {
                            renderers[i].material.SetVector("_TexScale", texScale);
                        }
                        else
                        {
                            renderers[i].GetPropertyBlock(propBlock);
                            propBlock.SetVector("_TexScale", texScale);
                            renderers[i].SetPropertyBlock(propBlock);
                        }
                    }
                }
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }

        void CenterPosition()
        {
            if (parent != null)
            {
                transform.position = parent.transform.position;
            }

        }

        void GetParent()
        {
            parent = scaleParentOverride == null ? transform.parent : scaleParentOverride;

            if (parent != null)
            {
                MeshFilter mf = parent.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    parentMesh = mf.mesh;
                }
                else
                {
                    mf = parent.GetComponentInChildren<MeshFilter>();
                    if (mf != null)
                    {
                        parentMesh = mf.mesh;
                    }
                    else
                    {
                        parentMesh = null;
                    }
                }

                parentRenderer = parent.GetComponent<MeshRenderer>();
                if (parentRenderer == null)
                {
                    parentRenderer = parent.GetComponentInChildren<MeshRenderer>();
                }
            }
            else
            {
                parent = null;
                parentMesh = null;
                parentRenderer = null;
            }
        }

        public Transform ScaleParentOverride
        {
            get
            {
                return scaleParentOverride;
            }
            set
            {
                scaleParentOverride = value;
                UpdateParent();
            }
        }
    }
}
