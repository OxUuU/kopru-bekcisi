using System.Collections.Generic;
using KopruBekcisi.Gameplay.Documents;
using KopruBekcisi.Gameplay.NPCs;
using KopruBekcisi.Gameplay.Verdict;

namespace KopruBekcisi.Gameplay.Rules
{
    public static class RuleService
    {
        // Gün 1 yasakları — ileride RuleSetSO olarak gelir
        static readonly HashSet<Kingdom> Day1BannedKingdoms = new() { Kingdom.Mistveil };

        public static List<Violation> GetViolations(NPCInstance npc, DocumentInstance doc)
        {
            var list = new List<Violation>();

            // ── Textual (metinsel) ────────────────────────────────────────
            if (doc.IsExpired)
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.ExpiredPassport,
                    Category = ViolationCategory.Textual,
                    Description = "Pasaportun süresi dolmuş.",
                });
            }
            if (doc.HasMissingField)
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.MissingField,
                    Category = ViolationCategory.Textual,
                    Description = "Belgede eksik alan var.",
                });
            }
            if (Day1BannedKingdoms.Contains(doc.IssuingKingdom))
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.KingdomBan,
                    Category = ViolationCategory.Textual,
                    Description = $"{doc.IssuingKingdom} krallığı bugün geçici olarak yasak.",
                });
            }

            // ── Visual (sahte mühür / aura) ───────────────────────────────
            if (doc.HasForgedSeal)
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.ForgedSeal,
                    Category = ViolationCategory.Visual,
                    Description = "Mühür sahte (silik / yanlış desen).",
                    RequiresInspectionTool = true,
                });
            }
            if (doc.BetraysHiddenAura)
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.MagicalAuraSuppressed,
                    Category = ViolationCategory.Visual,
                    Description = "NPC büyü taşıyor; belge bunu gizliyor (mercek gerekli).",
                    RequiresInspectionTool = true,
                });
            }

            // ── Behavioral (söylenen ↔ belge çelişkisi) ───────────────────
            if (npc.ClaimedClass != doc.StatedClass)
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.ClassMismatch,
                    Category = ViolationCategory.Behavioral,
                    Description = $"NPC kendini '{npc.ClaimedClass}' olarak tanıttı, belge '{doc.StatedClass}' diyor.",
                });
            }
            if (npc.IsWanted)
            {
                list.Add(new Violation
                {
                    Kind = ViolationKind.WantedFugitive,
                    Category = ViolationCategory.Behavioral,
                    Description = "Aranan kişi (engizisyon listesi).",
                });
            }

            return list;
        }

        public static bool HasAnyViolation(NPCInstance npc, DocumentInstance doc) => GetViolations(npc, doc).Count > 0;
    }
}
