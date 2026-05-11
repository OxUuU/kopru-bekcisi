using System;
using System.Collections.Generic;
using UnityEngine;
using KopruBekcisi.Core;
using KopruBekcisi.Economy;
using KopruBekcisi.Karma;
using KopruBekcisi.Gameplay.Codex;
using KopruBekcisi.Gameplay.Documents;
using KopruBekcisi.Gameplay.NPCs;
using KopruBekcisi.Gameplay.Verdict;

namespace KopruBekcisi.Gameplay.Day
{
    public enum DayPhase { Idle, AwaitingVerdict, Resolving, DayEnd }

    public class DayManager : MonoBehaviour
    {
        public int DayNumber => GameDirector.Instance != null ? GameDirector.Instance.DayNumber : 1;
        public DayPhase Phase { get; private set; } = DayPhase.Idle;
        public NPCInstance CurrentNPC { get; private set; }
        public DocumentInstance CurrentDocument { get; private set; }
        public int Gold => _economy?.Gold ?? 0;
        public int Karma => _karma?.Score ?? 0;
        public int RemainingInQueue => _queue.Count;
        public CodexService Codex => _codex;
        public DayStats Stats => _stats;

        readonly Queue<NPCInstance> _queue = new Queue<NPCInstance>();
        EconomyService _economy;
        KarmaService _karma;
        CodexService _codex;
        DayStats _stats;

        public event Action<NPCInstance, DocumentInstance> OnNPCApproached;
        public event Action<VerdictResult> OnVerdictResolved;
        public event Action<int> OnGoldChanged;
        public event Action<int> OnKarmaChanged;
        public event Action OnDayEnded;

        void OnEnable()
        {
            var dir = GameDirector.Instance;
            if (dir != null)
            {
                _economy = dir.Economy;
                _karma = dir.Karma;
                _codex = dir.Codex;
            }
            else
            {
                _economy = new EconomyService(20);
                _karma = new KarmaService();
                _codex = new CodexService();
            }
            _economy.GoldChanged += HandleGoldChanged;
            _karma.KarmaChanged += HandleKarmaChanged;
            BeginDay();
        }

        void OnDisable()
        {
            if (_economy != null) _economy.GoldChanged -= HandleGoldChanged;
            if (_karma != null) _karma.KarmaChanged -= HandleKarmaChanged;
        }

        void HandleGoldChanged(int g) => OnGoldChanged?.Invoke(g);
        void HandleKarmaChanged(int k) => OnKarmaChanged?.Invoke(k);

        public void BeginDay()
        {
            _queue.Clear();
            _stats = new DayStats();
            foreach (var n in NPCFactory.BuildSampleDayQueue()) _queue.Enqueue(n);
            Phase = DayPhase.Idle;
            Debug.Log($"[DayManager] Gün {DayNumber} başlıyor. Sıra: {_queue.Count} NPC.");
            CallNext();
        }

        public void CallNext()
        {
            if (_queue.Count == 0)
            {
                EndDay();
                return;
            }
            CurrentNPC = _queue.Dequeue();
            CurrentDocument = DocumentService.GenerateFor(CurrentNPC);
            bool newKingdom = _codex.RegisterSighting(CurrentNPC, CurrentDocument);
            if (newKingdom)
                Debug.Log($"[Codex] Yeni krallık keşfedildi: {CurrentDocument.IssuingKingdom} (mühür: {CurrentDocument.SealCode})");
            Phase = DayPhase.AwaitingVerdict;
            Debug.Log($"[DayManager] Sıradaki: {CurrentNPC.DisplayName} ({CurrentNPC.Kingdom}). Kalan: {_queue.Count}.");
            OnNPCApproached?.Invoke(CurrentNPC, CurrentDocument);
        }

        public void SubmitVerdict(VerdictAction action)
        {
            if (Phase != DayPhase.AwaitingVerdict || CurrentNPC == null) return;
            Phase = DayPhase.Resolving;
            var result = VerdictResolver.Resolve(CurrentNPC, CurrentDocument, action, _economy, _karma);
            _stats.Processed++;
            if (result.WasCorrect) _stats.Correct++; else _stats.Wrong++;
            if (result.GoldDelta > 0) _stats.GoldEarned += result.GoldDelta;
            else if (result.GoldDelta < 0) _stats.GoldLost += -result.GoldDelta;
            _stats.KarmaSwing += result.KarmaDelta;
            Debug.Log($"[DayManager] {action} -> {result.Feedback} (correct={result.WasCorrect})");
            OnVerdictResolved?.Invoke(result);
            CurrentNPC = null;
            CurrentDocument = null;
            CallNext();
        }

        void EndDay()
        {
            Phase = DayPhase.DayEnd;
            _stats.NewKingdomsDiscovered = _codex.Entries.Count;
            Debug.Log($"[DayManager] Gün {DayNumber} bitti. İşlem: {_stats.Processed}, Doğru: {_stats.Correct}, Hatalı: {_stats.Wrong}. Altın={_economy.Gold}, Karma={_karma.Score}.");
            OnDayEnded?.Invoke();
            StartCoroutine(GoHomeAfterDelay(4f));
        }

        System.Collections.IEnumerator GoHomeAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            var dir = GameDirector.Instance;
            if (dir == null) yield break;
            dir.JumpTo(KopruBekcisi.Core.GameState.DayEnd);
            dir.JumpTo(KopruBekcisi.Core.GameState.HomeEvening);
        }

        public void Debug_AddGold(int amount) => _economy.Add(amount);

        public void Debug_SkipNPC()
        {
            if (Phase != DayPhase.AwaitingVerdict) return;
            Debug.Log("[DayManager] DEBUG: NPC atlandı.");
            CurrentNPC = null;
            CurrentDocument = null;
            CallNext();
        }

        public void Debug_EndDay() => EndDay();
    }
}
