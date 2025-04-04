using QuickAcid.Reporting;
using QuickAcid.Tests.ZheZhools;
using QuickMGenerate;

namespace QuickAcid.Tests.Bugs
{
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
			var run = AcidTestRun.FailedRun(3,
				from bugHouse in "BugHouse".TrackedInput(() => new BugHouse())
				from bugHouseRun in "BugHouse.Run".Act(bugHouse.Run)
				select Acid.Test);
			run.NumberOfReportEntriesIs(3);

			var entry1 = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
			Assert.Equal("(tracked) BugHouse", entry1.Key);

			var entry2 = run.GetReportEntryAtIndex<QAcidReportActEntry>(1);
			Assert.Equal("BugHouse.Run", entry2.Key);
			Assert.Null(entry2.Exception);

			var entry3 = run.GetReportEntryAtIndex<QAcidReportActEntry>(2);
			Assert.Equal("BugHouse.Run", entry3.Key);
			Assert.NotNull(entry3.Exception);

		}

		[Fact]
		public void Report_Should_Contain_All_Expected_Entries_For_Bounded_Behavior()
		{
			var run = AcidTestRun.FailedRun(1,
				from maxSize in "max size".TrackedInput(() => 1)
				from list in "set".TrackedInput(() => new List<int> { 42 }, l => "[ " + string.Join(", ", l) + " ]")
				from toAdd in "to add".ShrinkableInput(MGen.Int(-1000, 1000))
				from add in "add".Act(() => list.Add(toAdd))
				from spec in "Count <= MaxSize".Spec(() => list.Count <= 1)
				select Acid.Test);

			run.NumberOfReportEntriesIs(4);

			var entry0 = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
			Assert.Equal("(tracked) max size", entry0.Key);
			Assert.Equal("1", entry0.Value?.ToString());

			var entry1 = run.GetReportEntryAtIndex<QAcidReportInputEntry>(1);
			Assert.Equal("(tracked) set", entry1.Key);
			Assert.Equal("[ 42, 1000 ]", entry1.Value);

			var entry2 = run.GetReportEntryAtIndex<QAcidReportActEntry>(2);
			Assert.Equal("add", entry2.Key);

			var entry3 = run.GetReportEntryAtIndex<QAcidReportSpecEntry>(3);
			Assert.Equal("Count <= MaxSize", entry3.Key);
		}

		[Fact]
		public void Report_Should_Contain_All_Expected_Entries_For_Bounded_Behavior_Simple_Incorrect_Report()
		{
			var run = AcidTestRun.FailedRun(1,
				from maxSize in "max size".TrackedInput(() => 1)
				from list in "set".TrackedInput(() => new List<int> { 42 }, l => "[ " + string.Join(", ", l) + " ]")
				from toAdd in "to add".ShrinkableInput(MGen.Int(-1000, 1000))
				from add in "add".Act(() => list.Add(toAdd))
				from spec in "Count <= MaxSize".Spec(() => list.Count <= 1)
				select Acid.Test);

			run.NumberOfReportEntriesIs(4);

			var entry0 = run.GetReportEntryAtIndex<QAcidReportInputEntry>(0);
			Assert.Equal("(tracked) max size", entry0.Key);
			Assert.Equal("1", entry0.Value?.ToString());

			var entry1 = run.GetReportEntryAtIndex<QAcidReportInputEntry>(1);
			Assert.Equal("(tracked) set", entry1.Key);
			Assert.Equal("[ 42, 1000 ]", entry1.Value);

			var entry2 = run.GetReportEntryAtIndex<QAcidReportActEntry>(2);
			Assert.Equal("add", entry2.Key);

			var entry3 = run.GetReportEntryAtIndex<QAcidReportSpecEntry>(3);
			Assert.Equal("Count <= MaxSize", entry3.Key);
		}
	}
}