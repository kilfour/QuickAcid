﻿using System.Collections.Generic;
using System.Linq;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests
{
	public class DeletingFromAList
	{
		public class ListDeleter
		{
			public IList<int> DoingMyThing(IList<int> theList, int iNeedToBeRemoved)
			{
				var result = theList.ToList();
				result.Remove(iNeedToBeRemoved);
				return result;
			}
		}

		[Fact]
		public void ReportsError()
		{
			var intBetweenZeroAndTen = MGen.Int(0, 10);

			var ints =
				from numberOfInts in intBetweenZeroAndTen
				from list in intBetweenZeroAndTen.Many(numberOfInts).ToList()
				select list;

			var listDeleter = new ListDeleter();

			var test =
				from list in ints.ToShrinkableInput("input list")
				from toRemove in intBetweenZeroAndTen.ToInput("to remove")
				from output in QAcid.Act("listDeleter.DoingMyThing", () => listDeleter.DoingMyThing(list, toRemove))
				from spec in QAcid.Spec("int removed", () => !output.Contains(toRemove))
				select Unit.Instance;

			test.Verify(10, 10);
		}
	}
}