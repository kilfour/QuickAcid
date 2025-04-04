using QuickMGenerate;

namespace QuickAcid.Examples
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
				from a in "a".ShrinkableInput(MGen.Int(0, 10))
				from bughouse in "bughouse".TrackedInput(() => new BugHouse1())
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

		[Fact(Skip = "Explicit")]
		public void BugHouse2Error()
		{
			var run =
				from a in "a".ShrinkableInput(MGen.Int(0, 10))
				from bughouse in "bughouse".TrackedInput(() => new BugHouse2())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			run.Verify(100, 100);
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
				from a in "a".ShrinkableInput(MGen.Int(0, 10))
				from bughouse in "bughouse".TrackedInput(() => new BugHouse3())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			run.Verify(100, 100);
		}
	}
}