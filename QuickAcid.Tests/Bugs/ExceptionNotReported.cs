﻿using QuickAcid.Reporting;


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

		var ex = Assert.Throws<FalsifiableException>(() =>
			QState.Run(script)
				.WithOneRun()
				.And(2.ExecutionsPerRun()));

		var report = ex.QAcidReport;
		Assert.NotNull(report.Exception);

		var entryAR1 = report.FirstOrDefault<ReportTrackedEntry>();
		Assert.NotNull(entryAR1);
		Assert.Equal("BugHouse", entryAR1.Key);

		var entryAR2 = report.SecondOrDefault<ReportTrackedEntry>();
		Assert.NotNull(entryAR2);
		Assert.Equal("BugHouse", entryAR2.Key);

		var entry2 = report.FirstOrDefault<ReportExecutionEntry>();
		Assert.NotNull(entry2);
		Assert.Equal("BugHouse.Run", entry2.Key);

		var entry3 = report.SecondOrDefault<ReportExecutionEntry>();
		Assert.NotNull(entry3);
		Assert.Equal("BugHouse.Run", entry3.Key);


	}
}
