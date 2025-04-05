namespace QuickAcid.InterFace;

public class SpecBuilder
{
    private readonly string label;
    private Func<bool> condition = () => true;
    private Func<bool> assertion;

    public SpecBuilder(string label)
    {
        this.label = label;
    }

    public SpecBuilder OnlyWhen(Func<bool> condition)
    {
        this.condition = condition;
        return this;
    }

    // public QAcidRunner<Acid> Assert(Func<bool> assertion)
    // {
    //     return condition()
    //         ? label.Spec(assertion)
    //         : QAcidRunner<Acid>.Skip<Acid>();
    // }
}
