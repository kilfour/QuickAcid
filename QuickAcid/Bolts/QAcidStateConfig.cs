using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.Bolts;

public record QAcidStateConfig(
    bool Verbose = false,
    bool ShrinkingActions = false,
    IArtery? Diagnose = null);
