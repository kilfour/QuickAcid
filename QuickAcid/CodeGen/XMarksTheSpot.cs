using QuickAcid.Bolts;

namespace QuickAcid.CodeGen
{
    public enum RunnerType { AlwaysReportedInputRunner, SpecRunner, ActionRunner }

    public class Clue
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

        public override string ToString()
        {
            return $"{this.RunnerType.ToString()} {Key}";
        }
    }
    //public enum RunnerType { AlwaysReportedInputRunner, SpecRunner, ActionRunner }
}
