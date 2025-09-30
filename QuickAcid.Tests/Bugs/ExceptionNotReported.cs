using QuickAcid.Tests._Tools.ThePress;


namespace QuickAcid.Tests.Bugs;

public class ExceptionNotReported
{
	private class BugHouse
	{
		private int count;
		public bool Run()
		{
			if (count++ == 1) throw new Exception();
			return true;
		}
	}

	[Fact]
	public void Example()
	{
		var script =
			from bugHouse in "BugHouse".Tracked(() => new BugHouse())
			from bugHouseRun in "BugHouse.Run".Act(bugHouse.Run)
			select Acid.Test;

		var article = TheJournalist.Exposes(() => QState.Run("bughouse", script)
			.WithOneRun()
			.And(2.ExecutionsPerRun()));

		Assert.NotNull(article.Exception());

		Assert.Equal(2, article.Execution(1).Read().Times);
		Assert.Equal("BugHouse", article.Execution(1).Tracked(1).Read().Label);
		Assert.Equal("BugHouse.Run", article.Execution(1).Action(1).Read().Label);
	}
}
