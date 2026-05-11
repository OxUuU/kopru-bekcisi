using System;
using System.Collections.Generic;
using KopruBekcisi.Gameplay.Documents;
using KopruBekcisi.Gameplay.NPCs;

namespace KopruBekcisi.Gameplay.Codex
{
    public class CodexEntry
    {
        public Kingdom Kingdom;
        public string SealCode;
        public string FirstSeenNPC;
        public int SightingCount;
    }

    public class CodexService
    {
        readonly Dictionary<Kingdom, CodexEntry> _entries = new();

        public IReadOnlyDictionary<Kingdom, CodexEntry> Entries => _entries;

        public event Action<CodexEntry> OnNewKingdomDiscovered;
        public event Action<CodexEntry> OnSightingRecorded;

        public bool RegisterSighting(NPCInstance npc, DocumentInstance doc)
        {
            if (doc == null) return false;
            bool isNew = !_entries.TryGetValue(doc.IssuingKingdom, out var entry);
            if (isNew)
            {
                entry = new CodexEntry
                {
                    Kingdom = doc.IssuingKingdom,
                    SealCode = doc.SealCode,
                    FirstSeenNPC = npc?.DisplayName,
                    SightingCount = 1,
                };
                _entries[doc.IssuingKingdom] = entry;
                OnNewKingdomDiscovered?.Invoke(entry);
            }
            else
            {
                entry.SightingCount++;
                OnSightingRecorded?.Invoke(entry);
            }
            return isNew;
        }
    }
}
