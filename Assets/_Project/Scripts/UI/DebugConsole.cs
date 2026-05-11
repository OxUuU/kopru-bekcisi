using UnityEngine;
using UnityEngine.InputSystem;
using KopruBekcisi.Core;
using KopruBekcisi.Gameplay.Day;

namespace KopruBekcisi.UI
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField] DayManager dayManager;
        [SerializeField] bool showOnStart = true;
        bool _show;

        void Awake()
        {
            if (dayManager == null) dayManager = FindFirstObjectByType<DayManager>();
            _show = showOnStart;
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
                _show = !_show;
        }

        void OnGUI()
        {
            if (!_show) return;
            GUILayout.BeginArea(new Rect(8, 8, 240, 320), GUI.skin.box);
            GUILayout.Label("<b>DEBUG  (F1 to toggle)</b>");
            if (dayManager != null)
            {
                GUILayout.Label($"Phase: {dayManager.Phase}");
                GUILayout.Label($"NPC: {(dayManager.CurrentNPC?.DisplayName ?? "(none)")}");
                GUILayout.Label($"Sırada: {dayManager.RemainingInQueue}");
                GUILayout.Space(4);
                if (GUILayout.Button("+100 Altın")) dayManager.Debug_AddGold(100);
                if (GUILayout.Button("Skip NPC")) dayManager.Debug_SkipNPC();
                if (GUILayout.Button("End Day")) dayManager.Debug_EndDay();
            }
            else
            {
                GUILayout.Label("(DayManager bulunamadı)");
            }
            GUILayout.Space(6);
            GUILayout.Label("<b>SAVE</b>");
            var dir = GameDirector.Instance;
            if (dir != null)
            {
                if (GUILayout.Button("Save Snapshot")) dir.SaveSnapshot();
                if (GUILayout.Button("Load Snapshot")) { if (!dir.LoadSnapshot()) Debug.Log("[Debug] Save dosyası yok."); }
                if (GUILayout.Button("Reset Save")) dir.ResetSave();
            }
            GUILayout.EndArea();
        }
    }
}
