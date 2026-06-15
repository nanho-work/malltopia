using UnityEngine;

namespace Malltopia
{
    [DisallowMultipleComponent]
    public sealed class PrototypeFaceCamera : MonoBehaviour
    {
        public Vector3 eulerOffset;

        private void LateUpdate()
        {
            Camera targetCamera = Camera.main;
            if (targetCamera == null)
            {
                return;
            }

            transform.rotation = targetCamera.transform.rotation * Quaternion.Euler(eulerOffset);
        }
    }
}
