using UnityEngine;
using UnityEngine.InputSystem;

namespace KopruBekcisi.CameraRig
{
    public class CameraTestMover : MonoBehaviour
    {
        [SerializeField] float speed = 6f;
        [SerializeField] bool enabled_ = true;

        void Update()
        {
            if (!enabled_) return;
            if (Keyboard.current == null) return;

            float h = 0f, v = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v += 1f;

            if (h == 0f && v == 0f) return;
            var delta = new Vector3(h, v, 0f).normalized * (speed * Time.deltaTime);
            transform.position += delta;
        }
    }
}
