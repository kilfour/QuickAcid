using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Examples
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

		[Fact(Skip = "Explicit")]
		public void ReportsError()
		{
			var intBetweenZeroAndTen = MGen.Int(0, 10);

			var ints =
				from numberOfInts in intBetweenZeroAndTen
				from list in intBetweenZeroAndTen.Many(numberOfInts).ToList()
				select list;

			var listDeleter = new ListDeleter();

			var script =
				from list in "input list".Input(ints)
				from toRemove in "to remove".Input(intBetweenZeroAndTen)
				from output in "listDeleter.DoingMyThing".Act(() => listDeleter.DoingMyThing(list, toRemove))
				from spec in "int removed".Spec(() => !output.Contains(toRemove))
				select Acid.Test;
			10.Times(() => new QState(script).Testify(10));
		}
	}
}
