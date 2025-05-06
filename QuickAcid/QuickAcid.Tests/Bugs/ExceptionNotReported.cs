using QuickAcid.Reporting;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;


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
		var run =
			from bugHouse in "BugHouse".AlwaysReported(() => new BugHouse())
			from bugHouseRun in "BugHouse.Run".Act(bugHouse.Run)
			select Acid.Test;

		var report = new QState(run).Observe(3);

		var entry1 = report.FirstOrDefault<ReportAlwaysReportedInputEntry>();
		Assert.NotNull(entry1);
		Assert.Equal("BugHouse", entry1.Key);

		var entryAR2 = report.SecondOrDefault<ReportAlwaysReportedInputEntry>();
		Assert.NotNull(entry1);
		Assert.Equal("BugHouse", entry1.Key);

		var entry2 = report.FirstOrDefault<ReportExecutionEntry>();
		Assert.NotNull(entry2);
		Assert.Equal("BugHouse.Run", entry2.Key);
		Assert.Null(entry2.Exception);

		var entry3 = report.SecondOrDefault<ReportExecutionEntry>();
		Assert.NotNull(entry3);
		Assert.Equal("BugHouse.Run", entry3.Key);
		Assert.NotNull(entry3.Exception);

	}
}
