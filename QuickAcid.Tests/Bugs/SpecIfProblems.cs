using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid.Tests.Bugs;

public class SpecIfProblems
{
	private class Counter
	{
		public int Val;
	}

	[Fact]
	public void SpecIfBeforeSpec()
	{
		var run =
			from counter in "counter".AlwaysReported(() => new Counter())
			from act in "act".Act(() => counter.Val++)
			from _s1 in "if".SpecIf(() => counter.Val == 0, () => true)
			from _s2 in "s".Spec(() => counter.Val > 4)
			select Acid.Test;

		var report = run.ReportIfFailed(1, 5);
		Assert.NotNull(report);
	}

	[Fact]
	public void SpecIf_ShouldSkipAfterPriorFailure_RealisticCrashPattern()
	{
		var predicateRan = false;

		var run =
			from counter in "counter".AlwaysReported(() => new Counter())
			from act in "act".Act(() => counter.Val++)

				// Fail always, force failure before the SpecIf
			from _s1 in "failing spec".Spec(() => false)

				// Should be skipped — crashes if it runs
			from _s2 in "crashy conditional spec".SpecIf(
				() =>
				{
					predicateRan = true;
					if (counter.Val > 0)
						throw new Exception("This should not run after a failure");
					return false;
				},
				() => true)
			select Acid.Test;

		var report = run.ReportIfFailed(1, 5);

		Assert.NotNull(report); // still failed as expected
		Assert.False(predicateRan); // ✅ predicate must not run
	}

	[Fact]
	public void SpecIf_AfterFailingAct_ShouldBeSkipped()
	{
		var predicateRan = false;

		var run =
			from system in "system".AlwaysReported(() => new object())
			from fail in "fail".Act(() =>
			{
				throw new InvalidOperationException("fail!");
			})
			from _s1 in "should be skipped".SpecIf(
				() =>
				{
					predicateRan = true;
					throw new Exception("predicate should NOT run");
				},
				() => true)
			select Acid.Test;

		var report = run.ReportIfFailed(1, 1);
		Assert.NotNull(report);
		Assert.False(predicateRan); // ✅ only true if your fix is in place
	}
}