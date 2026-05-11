using UnityEngine;
using UnityEngine.InputSystem;
using KopruBekcisi.Gameplay.Day;

namespace KopruBekcisi.UI
{
    public class CodexPanel : MonoBehaviour
    {
        [SerializeField] DayManager dayManager;
        bool _show;

        void Awake()
        {
            if (dayManager == null) dayManager = FindFirstObjectByType<DayManager>();
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame)
                _show = !_show;
        }

        void OnGUI()
        {
            if (!_show) return;
            GUILayout.BeginArea(new Rect(Screen.width - 320, 8, 312, 280), GUI.skin.box);
            GUILayout.Label("<b>SEMBOL DEFTERİ  (F2)</b>");
            if (dayManager?.Codex != null)
            {
                if (dayManager.Codex.Entries.Count == 0)
                {
                    GUILayout.Label("(henüz mühür kaydedilmedi)");
                }
                else
                {
                    foreach (var kv in dayManager.Codex.Entries)
                    {
                        var e = kv.Value;
                        GUILayout.Label($"<b>{e.Kingdom}</b>  ×{e.SightingCount}");
                        GUILayout.Label($"  Mühür: {e.SealCode}");
                        GUILayout.Label($"  İlk: {e.FirstSeenNPC}");
                        GUILayout.Space(2);
                    }
                }
            }
            GUILayout.EndArea();
        }
    }
}
