using UnityEngine;
using UnityEngine.InputSystem;

namespace KopruBekcisi.Input
{
    public class InputRouter : MonoBehaviour
    {
        public enum InputMode { None, Bridge, Desk, Home }

        [SerializeField] InputActionAsset actions;
        [SerializeField] InputMode initialMode = InputMode.None;

        public InputMode Current { get; private set; } = InputMode.None;

        void Awake()
        {
            if (actions != null)
                foreach (var map in actions.actionMaps) map.Disable();
        }

        void Start()
        {
            if (initialMode != InputMode.None)
                Switch(initialMode);
        }

        public void Switch(InputMode mode)
        {
            if (mode == Current) return;
            if (actions == null)
            {
                Debug.LogWarning("[InputRouter] InputActionAsset atanmamış.");
                return;
            }

            foreach (var map in actions.actionMaps) map.Disable();

            string mapName = mode switch
            {
                InputMode.Bridge => "Bridge",
                InputMode.Desk => "Desk",
                InputMode.Home => "Home",
                _ => null
            };

            if (mapName != null)
            {
                var map = actions.FindActionMap(mapName);
                if (map == null)
                {
                    Debug.LogWarning($"[InputRouter] Action map '{mapName}' bulunamadı.");
                    return;
                }
                map.Enable();
            }

            Current = mode;
        }
    }
}
