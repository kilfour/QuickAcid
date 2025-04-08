using QuickAcid.Nuts;
using QuickAcid.Reporting;

namespace QuickAcid.Fluent;

// Let’s wrap this up and get the Report!
public class Wendy : Bob<Acid>
{
    private bool verbose = false;

    public Wendy(QAcidRunner<Acid> runner)
        : base(runner) { }

    public Wendy KeepOneEyeOnTheTouchStone()
    {
        verbose = true;
        return this;
    }

    public QAcidReport AndCheckForGold(int runs, int actions)
    {
        for (int i = 0; i < runs; i++)
        {
            var state = new QAcidState(runner) { Verbose = verbose };
            state.Run(actions);
            if (state.Failed)
                return state.GetReport();
        }
        return null!;
    }
}