using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleSelector
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        float billboardAmount;

        [SerializeField]
        bool lockXAxis = true;

        [SerializeField]
        bool flipNegYAxis = true;

        Camera mainCamera;

        Vector3 origRotation;
        Vector3 newRotation;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (billboardAmount >= 1f)
            {
                origRotation = transform.eulerAngles;
                newRotation = mainCamera.transform.eulerAngles;


                if (lockXAxis)
                {
                    newRotation.x = 0;
                }

                if (flipNegYAxis && origRotation.y >= 180)
                {
                    newRotation.y += 180;
                }

                transform.eulerAngles = Vector3.Lerp(origRotation, newRotation, billboardAmount);
            }

        }
    }
}
