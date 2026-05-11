using System.Collections.Generic;
using System.Text;
using KopruBekcisi.Economy;
using KopruBekcisi.Gameplay.Documents;
using KopruBekcisi.Gameplay.NPCs;
using KopruBekcisi.Gameplay.Rules;
using KopruBekcisi.Karma;

namespace KopruBekcisi.Gameplay.Verdict
{
    public class VerdictResult
    {
        public bool WasCorrect;
        public int GoldDelta;
        public int KarmaDelta;
        public string Feedback;
        public List<Violation> Violations;
    }

    public static class VerdictResolver
    {
        const int TaxApproval = 5;
        const int FineDeny = 1;
        const int BountyDetain = 15;

        public static VerdictResult Resolve(NPCInstance npc, DocumentInstance doc, VerdictAction action,
            EconomyService economy, KarmaService karma)
        {
            var violations = RuleService.GetViolations(npc, doc);
            bool anyViolation = violations.Count > 0;
            bool wanted = npc.IsWanted;

            var result = new VerdictResult { Violations = violations };
            var feedback = new StringBuilder();

            switch (action)
            {
                case VerdictAction.Approve:
                    if (!anyViolation)
                    {
                        result.WasCorrect = true;
                        result.GoldDelta = TaxApproval;
                        feedback.AppendLine($"İçeri alındı. +{TaxApproval} altın vergi.");
                    }
                    else
                    {
                        result.WasCorrect = false;
                        result.GoldDelta = -10;
                        result.KarmaDelta = -1;
                        feedback.AppendLine("İHMAL! İhlal varken içeri aldın. -10 altın, karma -1.");
                        AppendViolations(feedback, violations);
                    }
                    break;

                case VerdictAction.Deny:
                    if (anyViolation && !wanted)
                    {
                        result.WasCorrect = true;
                        result.GoldDelta = FineDeny;
                        feedback.AppendLine($"Geri gönderildi. +{FineDeny} altın geçiş ücreti.");
                    }
                    else if (!anyViolation)
                    {
                        result.WasCorrect = false;
                        result.KarmaDelta = -1;
                        feedback.AppendLine("Masum biri çevrildi. Karma -1.");
                    }
                    else
                    {
                        result.WasCorrect = false;
                        feedback.AppendLine("Aranan biri serbest bırakıldı! Engizisyon haberdar oluyor.");
                    }
                    break;

                case VerdictAction.Detain:
                    if (wanted || anyViolation)
                    {
                        result.WasCorrect = true;
                        result.GoldDelta = BountyDetain;
                        feedback.AppendLine($"Tutuklandı. +{BountyDetain} altın ödül.");
                    }
                    else
                    {
                        result.WasCorrect = false;
                        result.KarmaDelta = -2;
                        feedback.AppendLine("Masum biri tutuklandı! Karma -2.");
                    }
                    break;

                case VerdictAction.Execute:
                    if (wanted)
                    {
                        result.WasCorrect = true;
                        result.GoldDelta = BountyDetain * 2;
                        result.KarmaDelta = -1;
                        feedback.AppendLine("İdam edildi. Tehlike bertaraf, ama kan döküldü. Karma -1.");
                    }
                    else
                    {
                        result.WasCorrect = false;
                        result.KarmaDelta = -5;
                        feedback.AppendLine("Adam öldürüldü! Karma -5. Engizisyon dikkat etmeye başladı.");
                    }
                    break;
            }

            if (result.GoldDelta > 0) economy?.Add(result.GoldDelta);
            else if (result.GoldDelta < 0) economy?.TrySpend(-result.GoldDelta);
            if (result.KarmaDelta != 0) karma?.Adjust(result.KarmaDelta);

            result.Feedback = feedback.ToString().TrimEnd();
            return result;
        }

        static void AppendViolations(StringBuilder sb, List<Violation> violations)
        {
            foreach (var v in violations)
                sb.AppendLine($"  • {v}");
        }
    }
}
