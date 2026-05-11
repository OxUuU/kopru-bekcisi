namespace KopruBekcisi.Gameplay.Day
{
    public class DayStats
    {
        public int Processed;
        public int Correct;
        public int Wrong;
        public int GoldEarned;
        public int GoldLost;
        public int KarmaSwing;
        public int NewKingdomsDiscovered;

        public float Accuracy => Processed == 0 ? 0f : (float)Correct / Processed;
    }
}
