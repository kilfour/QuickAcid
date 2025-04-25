using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests;

public class ThreadingStressTests
{
    [Fact(Skip = "Explicit")]
    public async Task Shrinking_Should_Be_Thread_Safe()
    {
        const int parallelRuns = 1;

        var tasks = Enumerable.Range(0, parallelRuns)
            .Select(i => Task.Run(() =>
            {
                using (QAcidDebug.Logging($"log_thread_{Guid.NewGuid():N}.txt"))
                {
                    try
                    {
                        var run =
                            from ignored in "not used".Shrinkable(MGen.Int(1, 100))
                            from trigger in "failing value".Shrinkable(MGen.Int(0, 5))
                            from check in "must fail".Spec(() =>
                                {
                                    QAcidDebug.WriteLine($"[spec eval] trigger = {trigger}");
                                    return trigger == 3;
                                })
                            select Acid.Test;

                        var ex = Record.Exception(() => run.Verify(20, 1));
                        if (ex is FalsifiableException fex)
                        {
                            var msg = fex.ToString();

                            if (!msg.Contains("failing value"))
                                throw new Exception("Expected 'failing value' in report");

                            if (msg.Contains("not used"))
                                throw new Exception("'not used' should have been removed");
                        }
                        else
                        {
                            throw new Exception("Expected a FalsifiableException");
                        }
                    }
                    catch (Exception e)
                    {
                        QAcidDebug.WriteLine($"[ERROR] {e.Message}");
                        throw;
                    }
                }
            }))
            .ToArray();

        await Task.WhenAll(tasks);
    }
}

