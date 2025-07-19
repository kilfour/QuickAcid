using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Proceedings.ClerksOffice;

public record Dossier(
    string? FailingSpec,
    Exception? Exception,
    int OriginalRunExecutionCount,
    List<int> ExecutionNumbers,
    int ShrinkCount,
    int Seed);
