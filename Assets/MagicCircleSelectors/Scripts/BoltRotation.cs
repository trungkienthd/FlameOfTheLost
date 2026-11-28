using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleSelector
{
    public class BoltRotation : MonoBehaviour
    {
        [SerializeField]
        float rotationLifeMin = .1f;

        [SerializeField]
        float rotationLifeMax = 5f;

        Vector3 originAngle;
        Vector3 targetAngle;

        float rotationLife;

        float timer = 0;

        // Start is called before the first frame update
        void Start()
        {
            originAngle = Vector3.zero;
            targetAngle = Vector3.zero;

            SetRotationLife();
            SetRandomRotation();
            SetRotationTarget();
        }

        // Update is called once per frame
        void Update()
        {
            if (timer <= rotationLife)
            {
                transform.localEulerAngles = Vector3.Lerp(originAngle, targetAngle, timer / rotationLife);
                timer += Time.deltaTime;
            }
            else
            {
                timer -= rotationLife;
                SetRotationLife();
                SetRandomRotation();
                SetRotationTarget();
            }
        }

        void SetRandomRotation()
        {
            originAngle.z = Random.Range(-180, 180);
            transform.localEulerAngles = originAngle;
        }

        void SetRotationTarget()
        {
            targetAngle.z = Random.Range(-180, 180);
        }

        void SetRotationLife()
        {
            rotationLife = Random.Range(rotationLifeMin, rotationLifeMax);
        }
    }
}
