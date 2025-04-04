namespace QuickAcid.CodeGen
{
    public enum RunnerType { TrackedInputRunner, SpecRunner, ActionRunner }

    public struct Clue
    {
        public string Key { get; set; }
        public RunnerType RunnerType { get; set; }
        public Func<string, Memory.Access, string> SeeWhereItLeads { get; set; }
    }

    public class XMarksTheSpot
    {
        public List<Clue> TheMap = [];
        public Tracker TheTracker;
    }

    public struct Tracker
    {
        public string Key { get; set; }
        public RunnerType RunnerType { get; set; }
    }
}
