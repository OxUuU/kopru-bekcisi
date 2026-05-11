namespace KopruBekcisi.Gameplay.NPCs
{
    public enum Race { Human, Elf, Dwarf, Minotaur, Outcast }
    public enum Kingdom { Valdaris, Thaleros, KaragDun, Korthaga, Mistveil }
    public enum NpcClass { Civilian, Merchant, Pilgrim, Mercenary, Witch, Cultist }

    public class NPCInstance
    {
        public string Id;
        public string DisplayName;
        public Race Race;
        public Kingdom Kingdom;
        public NpcClass NpcClass;
        public string GreetingLine;

        // NPC'nin sözlü olarak iddia ettiği sınıf — belgesindekinden farklı olabilir (behavioral ihlal)
        public NpcClass ClaimedClass;

        // Aranan / kaçak
        public bool IsWanted;

        // Büyü kullanan biri ama belgesinde sıradan bir sınıf yazıyor
        public bool HasHiddenMagicalAura;

        // Scripted NPC için ek monolog (DeskUI'da NPC selamından sonra gösterilir)
        public string ExtraMonologue;
    }
}
