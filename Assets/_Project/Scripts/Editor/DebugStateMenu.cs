#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using KopruBekcisi.Core;

namespace KopruBekcisi.EditorTools
{
    public static class DebugStateMenu
    {
        const string Root = "KopruBekcisi/Debug/State";

        [MenuItem(Root + "/Print Current", priority = 0)]
        static void PrintCurrent() => Log(() => $"Current state: {GameDirector.Instance.StateMachine.Current}");

        [MenuItem(Root + "/Go to MainMenu", priority = 100)]
        static void GoMainMenu() => Request(GameState.MainMenu);

        [MenuItem(Root + "/Go to DayIntro", priority = 101)]
        static void GoDayIntro() => Request(GameState.DayIntro);

        [MenuItem(Root + "/Go to BridgeShift", priority = 102)]
        static void GoBridgeShift() => Request(GameState.BridgeShift);

        [MenuItem(Root + "/Go to DayEnd", priority = 103)]
        static void GoDayEnd() => Request(GameState.DayEnd);

        [MenuItem(Root + "/Go to HomeEvening", priority = 104)]
        static void GoHomeEvening() => Request(GameState.HomeEvening);

        [MenuItem(Root + "/Go to NightDecision", priority = 105)]
        static void GoNightDecision() => Request(GameState.NightDecision);

        [MenuItem(Root + "/Go to NextDay", priority = 106)]
        static void GoNextDay() => Request(GameState.NextDay);

        [MenuItem(Root + "/Run Smoke Test (Boot → Bridge → Home)", priority = 200)]
        static void SmokeTest()
        {
            if (!RequireRunning()) return;
            EditorApplication.delayCall += () => Request(GameState.DayIntro);
            EditorApplication.delayCall += () => Request(GameState.BridgeShift);
            EditorApplication.delayCall += () => Request(GameState.DayEnd);
            EditorApplication.delayCall += () => Request(GameState.HomeEvening);
            Debug.Log("[DebugStateMenu] Smoke test sıralandı: DayIntro → BridgeShift → DayEnd → HomeEvening");
        }

        static void Request(GameState s)
        {
            if (!RequireRunning()) return;
            GameDirector.Instance.RequestState(s);
        }

        static bool RequireRunning()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("[DebugStateMenu] Önce Play moduna geç.");
                return false;
            }
            if (GameDirector.Instance == null)
            {
                Debug.LogWarning("[DebugStateMenu] GameDirector.Instance yok. Boot sahnesinde Bootstrapper'ı kontrol et.");
                return false;
            }
            return true;
        }

        static void Log(System.Func<string> message)
        {
            if (!RequireRunning()) return;
            Debug.Log("[DebugStateMenu] " + message());
        }
    }
}
#endif
