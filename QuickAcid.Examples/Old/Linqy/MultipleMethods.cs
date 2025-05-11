using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Examples
{
	public class MultipleMethods
	{
		public class BugHouse
		{
			private string? bug;
			public bool RunInt(int a)
			{
				bug += "1";
				if (bug.EndsWith("1221") && a == 6)
					throw new Exception();
				return true;
			}

			public bool RunString(string a)
			{
				bug += "2";
				if (bug.EndsWith("122") && a == "p")
					throw new Exception();
				return true;
			}
		}

		[Fact(Skip = "Explicit")]
		public void BugHouseError()
		{
			var script =
				from bughouse in "bughouse".Tracked(() => new BugHouse())
				from funcOne in
					"Choose".Choose(
						from i in "int".Input(MGen.Int(0, 10))
						from runInt in "bughouse.RunInt".Act(() => bughouse.RunInt(i))
						from specOne in "returns true".Spec(() => runInt)
						select Acid.Test,
						from str in "string".Input(MGen.String(1, 1))
						from runString in "bughouse.RunString".Act(() => bughouse.RunString(str))
						from specTwo in "returns true".Spec(() => runString)
						select Acid.Test)
				select Acid.Test;

			100.Times(() => new QState(script).Testify(100));
		}
	}
}