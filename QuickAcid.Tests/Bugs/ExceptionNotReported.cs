using System;
using QuickAcid.Tests.ZheZhools;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests.Bugs
{
	public class ExceptionNotReported
	{
	    public class BugHouse
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
                from bugHouse in "BugHouse".OnceOnlyInput(() => new BugHouse())
                from bugHouseRun in "BugHouse.Run".Act(bugHouse.Run)
                select Unit.Instance);
		    run.NumberOfReportEntriesIs(2);
		    var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(1);
		    Assert.Equal("BugHouse.Run", entry.Key);
		    Assert.NotNull(entry.Exception);
        }
	}
}