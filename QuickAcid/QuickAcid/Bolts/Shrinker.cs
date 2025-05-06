

using QuickAcid.Bolts.TheyCanFade;

namespace QuickAcid.Bolts;

public class Shrinker
{
    // private void HandleFailure()
    // {
    //     ExecutionNumbers = [.. ExecutionNumbers.Where(x => x != -1)];
    //     OriginalFailingRunExecutionCount = ExecutionNumbers.Count;
    //     if (AllowShrinking)
    //     {
    //         ShrinkExecutions();
    //         if (Verbose)
    //         {
    //             report.AddEntry(new ReportTitleSectionEntry(["AFTER EXECUTION SHRINKING"]));
    //             report.AddEntry(new ReportRunStartEntry());
    //             AddMemoryToReport(report, false);
    //         }
    //         ShrinkInputs();
    //         if (Verbose)
    //         {
    //             var title = new List<string>(["AFTER INPUT SHRINKING :"]);
    //             title.AddRange([.. GetReportHeaderInfo()]);
    //             report.AddEntry(new ReportTitleSectionEntry(title));
    //             report.AddEntry(new ReportRunStartEntry());
    //         }
    //         else
    //         {
    //             report.AddEntry(new ReportTitleSectionEntry([.. GetReportHeaderInfo()]));
    //             report.AddEntry(new ReportRunStartEntry());
    //         }
    //         AddMemoryToReport(report, true);
    //     }
    //     else
    //     {
    //         report.AddEntry(new ReportTitleSectionEntry([$"The Assayer disagrees: {FailingSpec}."]));
    //     }
    //     if (Exception != null)
    //         report.Exception = Exception;
    // }

    public Memory Memory { get; private set; }
    public List<int> ExecutionNumbers { get; private set; }
    public int CurrentExecutionNumber { get; private set; }

    // private void ShrinkExecutions()
    // {
    //     var max = ExecutionNumbers.Max();
    //     var current = 0;
    //     while (current <= max)
    //     {
    //         using (EnterPhase(QAcidPhase.ShrinkingExecutions))
    //         {
    //             Memory.ResetAllRunInputs();
    //             foreach (var run in ExecutionNumbers.ToList())
    //             {
    //                 CurrentExecutionNumber = run;
    //                 if (run != current)
    //                     Runner(this);
    //                 if (CurrentContext.BreakRun)
    //                     break;
    //             }
    //             if (CurrentContext.Failed && !CurrentContext.BreakRun)
    //             {
    //                 ExecutionNumbers.Remove(current);
    //             }
    //             current++;
    //             shrinkCount++;
    //         }
    //     }
    // }

    // private void ShrinkInputs()
    // {
    //     using (EnterPhase(QAcidPhase.ShrinkingInputs))
    //     {
    //         Memory.ResetAllRunInputs();
    //         foreach (var executionNumber in ExecutionNumbers.ToList())
    //         {
    //             CurrentExecutionNumber = executionNumber;
    //             Runner(this);
    //             shrinkCount++;
    //         }
    //     }
    // }
}