using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KopruBekcisi.Core.Save;
using KopruBekcisi.Economy;
using KopruBekcisi.Karma;
using KopruBekcisi.Gameplay.Codex;
using KopruBekcisi.Gameplay.NPCs;

namespace KopruBekcisi.Core
{
    [DefaultExecutionOrder(-1000)]
    public class GameDirector : MonoBehaviour
    {
        public static GameDirector Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureSingleton()
        {
            if (Instance != null) return;
            var go = new GameObject("GameDirector (Auto)");
            go.AddComponent<GameDirector>();
        }

        [SerializeField] string bridgeSceneName = "Bridge";
        [SerializeField] string homeSceneName = "Home";
        [SerializeField] bool autoAdvanceFromBoot = true;
        [SerializeField] int startingGold = 20;

        public GameStateMachine StateMachine { get; private set; }
        public EconomyService Economy { get; private set; }
        public KarmaService Karma { get; private set; }
        public CodexService Codex { get; private set; }
        public int DayNumber { get; set; } = 1;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Economy = new EconomyService(startingGold);
            Karma = new KarmaService();
            Codex = new CodexService();

            StateMachine = new GameStateMachine(GameState.Boot, BuildTransitions());
            StateMachine.StateChanged += OnStateChanged;
        }

        void Start()
        {
            if (autoAdvanceFromBoot)
                StateMachine.TryTransition(GameState.MainMenu);
        }

        static IEnumerable<(GameState, GameState)> BuildTransitions()
        {
            return new (GameState, GameState)[]
            {
                (GameState.Boot,           GameState.MainMenu),
                (GameState.MainMenu,       GameState.DayIntro),
                (GameState.MainMenu,       GameState.Ending),
                (GameState.DayIntro,       GameState.BridgeShift),
                (GameState.BridgeShift,    GameState.DayEnd),
                (GameState.DayEnd,         GameState.HomeEvening),
                (GameState.HomeEvening,    GameState.NightDecision),
                (GameState.NightDecision,  GameState.NextDay),
                (GameState.NightDecision,  GameState.Ending),
                (GameState.NextDay,        GameState.DayIntro),
                (GameState.NextDay,        GameState.Ending),
            };
        }

        void OnStateChanged(GameState from, GameState to)
        {
            Debug.Log($"[GameDirector] {from} -> {to}");

            switch (to)
            {
                case GameState.BridgeShift:
                    LoadSingleIfDifferent(bridgeSceneName);
                    break;
                case GameState.HomeEvening:
                    LoadSingleIfDifferent(homeSceneName);
                    break;
            }
        }

        static void LoadSingleIfDifferent(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            if (SceneManager.GetActiveScene().name == sceneName) return;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        public void RequestState(GameState next)
        {
            if (!StateMachine.TryTransition(next))
                Debug.LogWarning($"[GameDirector] Geçersiz transition: {StateMachine.Current} -> {next}");
        }

        // Sahne ile state'i senkronlamak için validation bypass.
        public void JumpTo(GameState next) => StateMachine.ForceTransition(next);

        // ── Save / Load ───────────────────────────────────────────────────
        public void SaveSnapshot()
        {
            var data = new SaveData
            {
                version = 1,
                dayNumber = DayNumber,
                gold = Economy.Gold,
                karma = Karma.Score,
                savedAtIso = System.DateTime.Now.ToString("o"),
            };
            foreach (var k in Codex.Entries.Keys) data.seenKingdoms.Add(k.ToString());
            SaveService.Save(data);
            Debug.Log($"[GameDirector] Save: gün {data.dayNumber}, altın {data.gold}, karma {data.karma}, {data.seenKingdoms.Count} krallık. Yol: {SaveService.GetSavePath()}");
        }

        public bool LoadSnapshot()
        {
            if (!SaveService.Exists()) return false;
            var data = SaveService.Load<SaveData>();
            DayNumber = data.dayNumber;
            // Ekonomi reset + apply
            int delta = data.gold - Economy.Gold;
            if (delta > 0) Economy.Add(delta);
            else if (delta < 0) Economy.TrySpend(-delta);
            // Karma fark uygula
            int karmaDelta = data.karma - Karma.Score;
            if (karmaDelta != 0) Karma.Adjust(karmaDelta);
            // Codex restore (kingdom isimleri)
            foreach (var name in data.seenKingdoms)
            {
                if (System.Enum.TryParse<Kingdom>(name, out var k))
                {
                    Codex.RegisterSighting(null, new KopruBekcisi.Gameplay.Documents.DocumentInstance
                    {
                        IssuingKingdom = k,
                        SealCode = $"(restored) {k}",
                    });
                }
            }
            Debug.Log($"[GameDirector] Load: gün {data.dayNumber}, altın {data.gold}, karma {data.karma}, {data.seenKingdoms.Count} krallık.");
            return true;
        }

        public void ResetSave()
        {
            SaveService.Delete();
            Debug.Log("[GameDirector] Save dosyası silindi.");
        }
    }
}
