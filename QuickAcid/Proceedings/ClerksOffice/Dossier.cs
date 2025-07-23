namespace QuickAcid.Proceedings.ClerksOffice;

public record Dossier(
    string? FailingSpec,
    Exception? Exception,
    string? AssayerSpec,
    int OriginalRunExecutionCount,
    List<int> ExecutionNumbers,
    int ShrinkCount,
    int Seed);
