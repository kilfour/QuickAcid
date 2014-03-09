using System;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests
{
	public class BuggingTheHouse
	{
		public class BugHouse1
		{
			private int count;
			public bool Run(int a)
			{
				if (count++ == 2 && a == 6) throw new Exception(); return true;
			}
		}

		[Fact]
		public void BugHouse1Error()
		{
			var test =
				from a in MGen.Int(0, 10).ShrinkableInput("a")
				from bughouse in QAcid.OnceOnlyInput("bughouse", () => new BugHouse1())
				from output in QAcid.Act("bughouse.Run", () => bughouse.Run(a))
				from spec in QAcid.Spec("returns true", () => output)
				select Unit.Instance;
			test.Verify(10, 100);
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

		[Fact]
		public void BugHouse2Error()
		{
			var test =
				from a in MGen.Int(0, 10).ShrinkableInput("a")
				from bughouse in QAcid.OnceOnlyInput("bughouse", () => new BugHouse2())
				from output in QAcid.Act("bughouse.Run", () => bughouse.Run(a))
				from spec in QAcid.Spec("returns true", () => output)
				select Unit.Instance;
			test.Verify(10, 100);
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

		[Fact]
		public void BugHouse3Error()
		{
			var test =
			    from a in MGen.Int(0, 10).ShrinkableInput("a")
				from bughouse in QAcid.OnceOnlyInput("bughouse", () => new BugHouse3())
				from output in QAcid.Act("bughouse.Run", () => bughouse.Run(a))
				from spec in QAcid.Spec("returns true", () => output)
				select Unit.Instance;
			test.Verify(10, 100);
		}
	}
}