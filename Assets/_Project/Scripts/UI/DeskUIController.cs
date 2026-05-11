using UnityEngine;
using UnityEngine.UI;
using KopruBekcisi.Gameplay.Day;
using KopruBekcisi.Gameplay.Documents;
using KopruBekcisi.Gameplay.Inspection;
using KopruBekcisi.Gameplay.NPCs;
using KopruBekcisi.Gameplay.Verdict;

namespace KopruBekcisi.UI
{
    public class DeskUIController : MonoBehaviour
    {
        [SerializeField] DayManager dayManager;
        [SerializeField] InspectionController inspection;
        [SerializeField] GameObject documentPanel;
        [SerializeField] Text documentText;
        [SerializeField] Text greetingText;
        [SerializeField] Text feedbackText;
        [SerializeField] Text statusText;
        [SerializeField] Button approveButton;
        [SerializeField] Button denyButton;
        [SerializeField] Button detainButton;
        [SerializeField] Button executeButton;
        [SerializeField] Image magnifierIndicator;
        [SerializeField] Image lanternIndicator;

        DocumentInstance _currentDoc;
        NPCInstance _currentNpc;

        void Awake()
        {
            if (dayManager == null) dayManager = FindFirstObjectByType<DayManager>();
            if (inspection == null) inspection = FindFirstObjectByType<InspectionController>();
        }

        void OnEnable()
        {
            if (dayManager == null) return;
            dayManager.OnNPCApproached += HandleNPCApproached;
            dayManager.OnVerdictResolved += HandleVerdictResolved;
            dayManager.OnGoldChanged += HandleAnyChange;
            dayManager.OnKarmaChanged += HandleAnyChange;
            dayManager.OnDayEnded += HandleDayEnded;

            if (inspection != null) inspection.OnToolChanged += HandleToolChanged;

            if (approveButton) approveButton.onClick.AddListener(() => SubmitVerdict(VerdictAction.Approve));
            if (denyButton) denyButton.onClick.AddListener(() => SubmitVerdict(VerdictAction.Deny));
            if (detainButton) detainButton.onClick.AddListener(() => SubmitVerdict(VerdictAction.Detain));
            if (executeButton) executeButton.onClick.AddListener(() => SubmitVerdict(VerdictAction.Execute));

            UpdateStatus();
            HandleToolChanged(inspection?.ActiveTool ?? InspectionTool.None);
        }

        void OnDisable()
        {
            if (dayManager == null) return;
            dayManager.OnNPCApproached -= HandleNPCApproached;
            dayManager.OnVerdictResolved -= HandleVerdictResolved;
            dayManager.OnGoldChanged -= HandleAnyChange;
            dayManager.OnKarmaChanged -= HandleAnyChange;
            dayManager.OnDayEnded -= HandleDayEnded;
            if (inspection != null) inspection.OnToolChanged -= HandleToolChanged;

            if (approveButton) approveButton.onClick.RemoveAllListeners();
            if (denyButton) denyButton.onClick.RemoveAllListeners();
            if (detainButton) detainButton.onClick.RemoveAllListeners();
            if (executeButton) executeButton.onClick.RemoveAllListeners();
        }

        void SubmitVerdict(VerdictAction a)
        {
            // Approve + visible bribe = otomatik rüşvet alımı (sessiz)
            if (a == VerdictAction.Approve && _currentDoc != null && _currentDoc.HasHiddenBribe && IsLanternActive())
            {
                int bribe = _currentDoc.BribeAmount;
                var dir = KopruBekcisi.Core.GameDirector.Instance;
                if (dir != null)
                {
                    dir.Economy.Add(bribe);
                    dir.Karma.Adjust(-1);
                    Debug.Log($"[Bribe] +{bribe} altın cebe atıldı, karma -1.");
                }
            }
            inspection?.ClearTool();
            dayManager.SubmitVerdict(a);
        }

        bool IsMagnifierActive() => inspection != null && inspection.ActiveTool == InspectionTool.Magnifier;
        bool IsLanternActive() => inspection != null && inspection.ActiveTool == InspectionTool.Lantern;

        void HandleNPCApproached(NPCInstance npc, DocumentInstance doc)
        {
            _currentNpc = npc;
            _currentDoc = doc;
            inspection?.ClearTool();
            if (documentPanel) documentPanel.SetActive(true);
            if (greetingText)
            {
                string g = $"\"{npc.GreetingLine}\"\n— {npc.DisplayName}";
                if (!string.IsNullOrEmpty(npc.ExtraMonologue)) g += "\n\n" + npc.ExtraMonologue;
                greetingText.text = g;
            }
            RefreshDocumentText();
            if (feedbackText) feedbackText.text = "";
            SetButtonsInteractable(true);
            UpdateStatus();
        }

        void HandleVerdictResolved(VerdictResult result)
        {
            if (feedbackText) feedbackText.text = result.Feedback;
            if (feedbackText) feedbackText.color = result.WasCorrect ? new Color(0.65f, 0.85f, 0.5f) : new Color(0.85f, 0.45f, 0.45f);
            SetButtonsInteractable(false);
            UpdateStatus();
        }

        void HandleAnyChange(int _) => UpdateStatus();

        void HandleDayEnded()
        {
            if (documentPanel) documentPanel.SetActive(false);
            if (greetingText) greetingText.text = "<b>MESAİ BİTTİ</b>\nKuyruk boşaldı. Yüzbaşı Halrik raporunu bekliyor.";
            if (feedbackText)
            {
                var s = dayManager.Stats;
                feedbackText.text =
                    $"=== GÜN {dayManager.DayNumber} RAPORU ===\n" +
                    $"İşlem: {s.Processed}    Doğru: {s.Correct}    Hatalı: {s.Wrong}    İsabet: %{(s.Accuracy * 100):F0}\n" +
                    $"Kazanılan altın: +{s.GoldEarned}    Ceza/Kayıp: -{s.GoldLost}    Net kasa: {dayManager.Gold}\n" +
                    $"Karma değişimi: {(s.KarmaSwing >= 0 ? "+" : "")}{s.KarmaSwing}    Mevcut karma: {dayManager.Karma}\n" +
                    $"Sembol Defterinde {s.NewKingdomsDiscovered} krallık kayıtlı.";
            }
            UpdateStatus();
        }

        void HandleToolChanged(InspectionTool t)
        {
            RefreshDocumentText();
        }

        void UpdateStatus()
        {
            if (statusText == null || dayManager == null) return;
            statusText.text = $"Gün {dayManager.DayNumber}  |  Altın: {dayManager.Gold}  |  Karma: {dayManager.Karma}  |  Sırada: {dayManager.RemainingInQueue}";
        }

        void SetButtonsInteractable(bool b)
        {
            if (approveButton) approveButton.interactable = b;
            if (denyButton) denyButton.interactable = b;
            if (detainButton) detainButton.interactable = b;
            if (executeButton) executeButton.interactable = b;
        }

        void RefreshDocumentText()
        {
            if (_currentDoc == null || documentText == null) return;
            documentText.text = FormatDocument(_currentDoc, IsMagnifierActive(), IsLanternActive());
        }

        static string FormatDocument(DocumentInstance d, bool magnifier, bool lantern)
        {
            string s = "PASAPORT\n\n";
            s += $"Ad: {d.HolderName}\n";
            s += $"Irk: {d.HolderRace}\n";
            s += $"Krallık: {d.IssuingKingdom}\n";
            s += $"Sınıf: {d.StatedClass}\n";
            s += $"Veriliş: {d.IssueDate}\n";
            s += $"Bitiş: {d.ExpiryDate}";
            if (d.IsExpired) s += "  <b>(SÜRE GEÇMİŞ)</b>";
            s += $"\nAmaç: {d.Purpose}\n";
            s += $"Mühür: {d.SealCode}";
            if (d.HasForgedSeal && lantern) s += "  <b>(SAHTE — fener altında parlamıyor)</b>";
            else if (d.HasForgedSeal) s += "  (silik?)";
            if (d.HasMissingField) s += "\n!!! Eksik alan var !!!";

            // Tool görüntüleri
            if (magnifier && d.BetraysHiddenAura)
                s += "\n\n<b>[MERCEK] AURA TESPİT EDİLDİ</b>\nTaşıyıcı belge gizliyor — büyücü.";
            if (lantern && d.HasHiddenBribe)
                s += $"\n\n<b>[FENER] GİZLİ ALTIN PARLIYOR</b>\nBelge arasına {d.BribeAmount} altın yerleştirilmiş.";
            return s;
        }
    }
}
