using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Examples.Old.Linqy
{
	public class BuggingTheHouse
	{
		public class BugHouse1
		{
			private int count;
			public bool Run(int a)
			{
				if (count++ == 2 && a == 1) throw new Exception(); return true;
			}
		}

		[Fact(Skip = "Explicit")]
		public void BugHouse1Error()
		{
			var run =
				from a in "a".Shrinkable(MGen.Int(0, 10))
				from bughouse in "bughouse".AlwaysReported(() => new BugHouse1())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			run.Verify(100, 100);
		}

		public class BugHouse2
		{
			private int count;
			public bool Run(int a)
			{
				if (a == 6) count++;
				if (count == 3) throw new Exception(); return true;
			}
		}

		[Fact(Skip = "explicit")]
		public void BugHouse2Error()
		{
			var run =
				from a in "a".Shrinkable(MGen.Int(0, 10))
				from bughouse in "bughouse".AlwaysReported(() => new BugHouse2())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			var report = run.ReportIfFailed(1, 50);
			if (report != null)
				Assert.Fail(report.ToString());
		}

		public class BugHouse3
		{
			private int count;
			public bool Run(int a)
			{
				if (a == 6 && count != 3) count++;
				if (count >= 3) count++;
				if (count == 5) throw new Exception(); return true;
			}
		}

		[Fact(Skip = "Explicit")]
		public void BugHouse3Error()
		{
			var run =
				from a in "a".Shrinkable(MGen.Int(0, 10))
				from bughouse in "bughouse".AlwaysReported(() => new BugHouse3())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			run.Verify(100, 100);
		}
	}
}