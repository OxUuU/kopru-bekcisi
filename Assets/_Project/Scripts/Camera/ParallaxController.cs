using UnityEngine;

namespace KopruBekcisi.CameraRig
{
    public class ParallaxController : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform;
        [SerializeField, Range(0f, 1f)] float parallaxFactor = 0.5f;
        [SerializeField] bool pixelSnap = true;
        [SerializeField] int pixelsPerUnit = 16;

        Vector3 _previousCameraPos;
        bool _initialized;

        void Start()
        {
            TryInit();
        }

        void TryInit()
        {
            if (_initialized) return;
            if (cameraTransform == null)
            {
                var cam = UnityEngine.Camera.main;
                if (cam == null) cam = FindFirstObjectByType<UnityEngine.Camera>();
                if (cam != null) cameraTransform = cam.transform;
            }
            if (cameraTransform != null)
            {
                _previousCameraPos = cameraTransform.position;
                _initialized = true;
            }
        }

        void LateUpdate()
        {
            if (!_initialized) TryInit();
            if (cameraTransform == null) return;

            var delta = cameraTransform.position - _previousCameraPos;
            var newPos = transform.position + new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);

            if (pixelSnap && pixelsPerUnit > 0)
            {
                float unit = 1f / pixelsPerUnit;
                newPos.x = Mathf.Round(newPos.x / unit) * unit;
                newPos.y = Mathf.Round(newPos.y / unit) * unit;
            }

            transform.position = newPos;
            _previousCameraPos = cameraTransform.position;
        }
    }
}
