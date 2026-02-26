using UnityEngine;

namespace FlameOfTheLost
{
    public class BillboardToCamera2DHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera; // leave empty to use Camera.main
        private Camera MainCamera
        {
            get => _camera;
            set => _camera = value;
        }

        private void LateUpdate()
        {
            if (this.MainCamera == null) this.MainCamera = Camera.main;
            if (this.MainCamera == null) return;

            this.transform.rotation = Quaternion.LookRotation(this.MainCamera.transform.forward, Vector3.up);
        }
    }
}
