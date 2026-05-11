namespace KopruBekcisi.Gameplay.Verdict
{
    public enum ViolationCategory
    {
        Textual,
        Visual,
        Behavioral
    }

    public enum ViolationKind
    {
        ExpiredPassport,
        MissingField,
        ForgedSeal,
        MagicalAuraSuppressed,
        ClassMismatch,
        KingdomBan,
        WantedFugitive
    }

    public class Violation
    {
        public ViolationKind Kind;
        public ViolationCategory Category;
        public string Description;
        public bool RequiresInspectionTool;

        public override string ToString() => $"[{Category}] {Description}";
    }
}
