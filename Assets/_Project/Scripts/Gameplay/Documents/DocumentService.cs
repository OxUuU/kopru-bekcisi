using KopruBekcisi.Gameplay.NPCs;

namespace KopruBekcisi.Gameplay.Documents
{
    public static class DocumentService
    {
        public static DocumentInstance GenerateFor(NPCInstance npc)
        {
            var doc = new DocumentInstance
            {
                Type = DocumentType.Passport,
                HolderName = npc.DisplayName,
                HolderRace = npc.Race,
                IssuingKingdom = npc.Kingdom,
                StatedClass = npc.NpcClass,
                IssueDate = "12 Çiy Ayı 1247",
                ExpiryDate = "12 Çiy Ayı 1252",
                Purpose = PickPurpose(npc.NpcClass),
                SealCode = SealFor(npc.Kingdom),
            };

            // Mistveil belgeleri sıklıkla sahte mühürlü gelir
            if (npc.Kingdom == Kingdom.Mistveil)
            {
                doc.HasForgedSeal = true;
            }

            // Aldis Renn için süresi dolmuş pasaport (textual)
            if (npc.Id == "npc_005")
            {
                doc.IsExpired = true;
                doc.ExpiryDate = "03 Karaca Ayı 1245";
            }

            // Sevra için gizli büyü aurası (visual; M8 mercek altında görünür)
            if (npc.HasHiddenMagicalAura)
            {
                doc.BetraysHiddenAura = true;
            }

            // Brogur (yalan tüccar) belgesinin arasında 25 altın rüşvet — sadece fener altında parlar
            if (npc.Id == "npc_004")
            {
                doc.HasHiddenBribe = true;
                doc.BribeAmount = 25;
            }

            // Velka (aranan tarikatçı) 50 altın gizli rüşvet
            if (npc.Id == "npc_007")
            {
                doc.HasHiddenBribe = true;
                doc.BribeAmount = 50;
            }

            return doc;
        }

        static string PickPurpose(NpcClass c) => c switch
        {
            NpcClass.Merchant => "Ticaret",
            NpcClass.Pilgrim => "Hac yolculuğu",
            NpcClass.Mercenary => "Sözleşmeli görev",
            NpcClass.Witch => "Şifa",
            NpcClass.Cultist => "Ziyaret",
            _ => "Geçiş",
        };

        static string SealFor(Kingdom k) => k switch
        {
            Kingdom.Valdaris => "VAL-EAGLE-001",
            Kingdom.Thaleros => "THA-STAR-014",
            Kingdom.KaragDun => "KAR-FORGE-007",
            Kingdom.Korthaga => "KOR-HORN-022",
            Kingdom.Mistveil => "(silik) MST-???",
            _ => "?",
        };
    }
}
