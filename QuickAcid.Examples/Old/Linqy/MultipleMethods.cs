﻿using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;

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
			var run =
				from bughouse in "bughouse".AlwaysReported(() => new BugHouse())
				from funcOne in
					"Choose".Choose(
						from i in "int".ShrinkableInput(MGen.Int(0, 10))
						from runInt in "bughouse.RunInt".Act(() => bughouse.RunInt(i))
						from specOne in "returns true".Spec(() => runInt)
						select Acid.Test,
						from str in "string".ShrinkableInput(MGen.String(1, 1))
						from runString in "bughouse.RunString".Act(() => bughouse.RunString(str))
						from specTwo in "returns true".Spec(() => runString)
						select Acid.Test)
				select Acid.Test;

			run.Verify(100, 100);
		}
	}
}