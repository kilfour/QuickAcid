using QuickAcid.Nuts;
using QuickAcid.Reporting;

namespace QuickAcid.FluentContextAware;

// Let’s wrap this up and get the Report!
public class Wendy : Bob<Acid>
{
    public Wendy(QAcidRunner<Acid> runner)
        : base(runner) { }

    public QAcidReport AndCheckForGold(int runs, int actions)
    {
        for (int i = 0; i < runs; i++)
        {
            var state = new QAcidState(runner);
            state.Run(actions);
            if (state.Failed)
                return state.GetReport();
        }
        return null!;
    }
}