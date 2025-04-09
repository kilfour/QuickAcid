using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.CodeGen
{
    public static class CodeGenExtensions
    {
        public static QAcidRunner<T> AddCode<T>(this QAcidRunner<T> runner, Func<string, Access, string> toCode)
        {
            return
                s =>
                {
                    var value = runner(s);
                    if (s.XMarksTheSpot.TheMap.All(a => a.Key != s.XMarksTheSpot.TheTracker.Key))
                        s.XMarksTheSpot.TheMap.Add(
                            new Clue
                            {
                                Key = s.XMarksTheSpot.TheTracker.Key,
                                RunnerType = s.XMarksTheSpot.TheTracker.RunnerType,
                                SeeWhereItLeads = toCode
                            });
                    return value;
                };
        }
    }
}
