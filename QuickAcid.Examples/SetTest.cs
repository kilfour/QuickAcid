using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;
using QuickMGenerate;

namespace QuickAcid.Examples
{
	public static class K
	{
		public static QKey<ICollection<int>> Set => QKey<ICollection<int>>.New("Set");
		public static QKey<int> IntToAdd => QKey<int>.New("IntToAdd");
		public static QKey<int> IntToRemove => QKey<int>.New("IntToRemove");
	}

	public class SetTest
	{
		[Fact(Skip = "Demo")]
		public void ReportsError()
		{
			var report =
				SystemSpecs.Define()
					.AlwaysReported(K.Set, () => new List<int>())
					.Fuzzed(K.IntToAdd, MGen.Int(1, 10))
					.Fuzzed(K.IntToRemove, MGen.Int(1, 10))
					.Options(opt => [
						opt.Do("Add", c => c.Get(K.Set).Add(c.Get(K.IntToAdd)))
							.Expect("Set contains added int")
							.Ensure(ctx => ctx.Get(K.Set).Contains(ctx.Get(K.IntToAdd))),
						opt.Do("Remove", c => c.Get(K.Set).Remove(c.Get(K.IntToRemove)))
							.Expect("Set does not contain removed int")
							.Ensure(ctx => !ctx.Get(K.Set).Contains(ctx.Get(K.IntToRemove)))
					])
					.PickOne()
					.DumpItInAcid()
					.AndCheckForGold(30, 50);
			if (report != null)
				Assert.Fail(report.ToString());
		}
	}
}
