using QuickAcid.Tests._Tools.ThePress;

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
		var script =
			from counter in "counter".Tracked(() => new Counter())
			from act in "act".Act(() => counter.Val++)
			from _s1 in "if".SpecIf(() => counter.Val == 0, () => true)
			from _s2 in "s".Spec(() => counter.Val > 4)
			select Acid.Test;

		TheJournalist.Exposes(() => QState.Run(script)
			.WithOneRun()
			.And(5.ExecutionsPerRun()));

	}

	[Fact(Skip = "not sure this is what we want")]
	public void SpecIf_ShouldSkipAfterPriorFailure_RealisticCrashPattern()
	{
		var predicateRan = false;

		var script =
			from counter in "counter".Tracked(() => new Counter())
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

		var article = TheJournalist.Exposes(() => QState.Run(script)
			.WithOneRun()
			.And(3.ExecutionsPerRun()));

		Assert.False(predicateRan);
	}

	[Fact(Skip = "not sure this is what we want")]
	public void SpecIf_AfterFailingAct_ShouldBeSkipped()
	{
		var predicateRan = false;

		var script =
			from system in "system".Tracked(() => new object())
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

		var article = TheJournalist.Exposes(() => QState.Run(script)
			.WithOneRun()
			.AndOneExecutionPerRun());

		Assert.False(predicateRan);
	}
}