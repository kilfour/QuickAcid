namespace QuickAcid.Phasers;

public class Shifter
{
    private readonly Dictionary<QAcidPhase, PhaseContext> phaseContexts =
        Enum.GetValues<QAcidPhase>().ToDictionary(phase => phase, phase => new PhaseContext(phase));
    public QAcidPhase CurrentPhase { get; private set; } = QAcidPhase.NormalRun;
    public PhaseContext CurrentContext => phaseContexts[CurrentPhase];
    public PhaseContext Phase(QAcidPhase phase) => phaseContexts[phase];
    public bool IsShrinkingExecutions => CurrentPhase == QAcidPhase.ShrinkingExecutions;
    public IDisposable EnterPhase(QAcidPhase phase)
    {
        var previousPhase = CurrentPhase;
        CurrentPhase = phase;
        CurrentContext.Reset();
        return new DisposableAction(() =>
        {
            CurrentPhase = previousPhase;
        });
    }

    public PhaseContext OriginalRun => Phase(QAcidPhase.NormalRun);
}