using System.Collections.Generic;

namespace KopruBekcisi.Gameplay.NPCs
{
    public static class NPCFactory
    {
        public static List<NPCInstance> BuildSampleDayQueue()
        {
            return new List<NPCInstance>
            {
                // 1) Temiz tüccar — geçer
                new NPCInstance
                {
                    Id = "npc_001",
                    DisplayName = "Tomil Hark",
                    Race = Race.Human, Kingdom = Kingdom.Valdaris, NpcClass = NpcClass.Merchant,
                    ClaimedClass = NpcClass.Merchant,
                    GreetingLine = "Selamünaleyküm bekçi. Tüccarım, işim acele.",
                },
                // 2) Temiz hacı — geçer
                new NPCInstance
                {
                    Id = "npc_002",
                    DisplayName = "Ilithra of Thal'eros",
                    Race = Race.Elf, Kingdom = Kingdom.Thaleros, NpcClass = NpcClass.Pilgrim,
                    ClaimedClass = NpcClass.Pilgrim,
                    GreetingLine = "Aydınlık seninle olsun. Hac yolundayım.",
                },
                // 3) TEXTUAL ihlal — Mistveil yasağı + sahte mühür (Visual da tetiklenir)
                new NPCInstance
                {
                    Id = "npc_003",
                    DisplayName = "Gana Wurmskin",
                    Race = Race.Outcast, Kingdom = Kingdom.Mistveil, NpcClass = NpcClass.Civilian,
                    ClaimedClass = NpcClass.Civilian,
                    GreetingLine = "Lütfen bekçi... bir şans ver.",
                },
                // 4) BEHAVIORAL ihlal — "tüccarım" der ama belge "paralı asker" yazar
                new NPCInstance
                {
                    Id = "npc_004",
                    DisplayName = "Brogur Stonebeard",
                    Race = Race.Dwarf, Kingdom = Kingdom.KaragDun,
                    NpcClass = NpcClass.Mercenary,
                    ClaimedClass = NpcClass.Merchant, // YALAN
                    GreetingLine = "Tüccarım, kervanım bekliyor. Hızlı geçirin.",
                },
                // 5) TEXTUAL ihlal — pasaport süresi dolmuş
                new NPCInstance
                {
                    Id = "npc_005",
                    DisplayName = "Aldis Renn",
                    Race = Race.Human, Kingdom = Kingdom.Valdaris, NpcClass = NpcClass.Civilian,
                    ClaimedClass = NpcClass.Civilian,
                    GreetingLine = "Geçen ay yenilemek istemiştim, sıraya yetişemedim...",
                },
                // 6) VISUAL ihlal — gizli büyü aurası (mercek gerekli, M8'de aktif olur; şimdilik metinsel ipucu)
                new NPCInstance
                {
                    Id = "npc_006",
                    DisplayName = "Sevra of the Mist",
                    Race = Race.Human, Kingdom = Kingdom.Valdaris, NpcClass = NpcClass.Civilian,
                    ClaimedClass = NpcClass.Civilian,
                    HasHiddenMagicalAura = true,
                    GreetingLine = "Sıradan bir vatandaşım... görünüyor pek dikkat çekmiyor değil mi?",
                },
                // 7) BEHAVIORAL ihlal — aranan tarikatçı
                new NPCInstance
                {
                    Id = "npc_007",
                    DisplayName = "Velka the Hollow",
                    Race = Race.Human, Kingdom = Kingdom.Valdaris, NpcClass = NpcClass.Cultist,
                    ClaimedClass = NpcClass.Pilgrim, // YALAN
                    IsWanted = true,
                    GreetingLine = "...ben sadece dua etmek için geçiyorum.",
                },
                // 8) Yeni krallık (Korthaga) — Codex'e ilk minotor mührü kaydedilmeli
                new NPCInstance
                {
                    Id = "npc_008",
                    DisplayName = "Karz Boğa-Yelesi",
                    Race = Race.Minotaur, Kingdom = Kingdom.Korthaga, NpcClass = NpcClass.Mercenary,
                    ClaimedClass = NpcClass.Mercenary,
                    GreetingLine = "Sözleşmem var bekçi. Lordum bekliyor.",
                },
                // 9) SCRIPTED — Vela Vell, eski bekçi Marek'in kız kardeşi.
                //    Belgesi temiz; ama hikaye kancası taşır.
                new NPCInstance
                {
                    Id = "npc_009_marek_sister",
                    DisplayName = "Vela Vell",
                    Race = Race.Human, Kingdom = Kingdom.Valdaris, NpcClass = NpcClass.Civilian,
                    ClaimedClass = NpcClass.Civilian,
                    GreetingLine = "Selam... yeni bekçi sen misin?",
                    ExtraMonologue =
                        "<i>Vela bir an duraksıyor.</i>\n\n" +
                        "\"Marek benim ağabeyim. Üç hafta önce bana son mektubunda Sygn'da bir şeyler keşfettiğini yazmıştı — " +
                        "köprünün altında, kalabalığın görmediği bir yerde. O mektuptan sonra haber alamadım.\n\n" +
                        "Sana sormak istiyorum: <b>masanın altına baktın mı?</b> Marek bir şeyler bırakmış olabilir. " +
                        "Eğer bulduysan... lütfen yargılama. Ağabeyim aptal değildir.\"\n\n" +
                        "<i>Belgesi tertemiz. Ama gözleri yorgun ve bir şey saklıyor gibi.</i>",
                },
            };
        }
    }
}
