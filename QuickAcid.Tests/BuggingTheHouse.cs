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
				from a in "a".ShrinkableInput(MGen.Int(0, 10))
				from bughouse in "bughouse".OnceOnlyInput(() => new BugHouse1())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Unit.Instance;
			test.Verify(100, 100);
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
				from a in "a".ShrinkableInput(MGen.Int(0, 10))
				from bughouse in "bughouse".OnceOnlyInput(() => new BugHouse2())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Unit.Instance;
			test.Verify(100, 100);
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
				from a in "a".ShrinkableInput(MGen.Int(0, 10))
				from bughouse in "bughouse".OnceOnlyInput(() => new BugHouse3())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Unit.Instance;
			test.Verify(100, 100);
		}
	}
}