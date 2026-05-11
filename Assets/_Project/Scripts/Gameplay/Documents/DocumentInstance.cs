using KopruBekcisi.Gameplay.NPCs;

namespace KopruBekcisi.Gameplay.Documents
{
    public enum DocumentType { Passport, Manifest, Permit }

    public class DocumentInstance
    {
        public DocumentType Type;
        public string HolderName;
        public Race HolderRace;
        public Kingdom IssuingKingdom;
        public NpcClass StatedClass;
        public string IssueDate;
        public string ExpiryDate;
        public string Purpose;
        public string SealCode;

        // Textual ihlaller (gözle metinden okunabilir)
        public bool IsExpired;
        public bool HasMissingField;

        // Visual ihlaller (M8'de mercek/fener altında görünür; şimdilik metin ipucu)
        public bool HasForgedSeal;

        // M8'de mercek altında: NPC büyü aurası taşıyor ama belge sıradan sınıf gösteriyor
        public bool BetraysHiddenAura;

        // Emanet Çantası — sadece fener altında parlar
        public bool HasHiddenBribe;
        public int BribeAmount;
    }
}
