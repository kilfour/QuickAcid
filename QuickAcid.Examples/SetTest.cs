using QuickAcid.Fluent;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Examples;

public static class K
{
	public static QKey<ICollection<int>> Set => QKey<ICollection<int>>.New("Set");
	public static QKey<int> IntToAdd => QKey<int>.New("IntToAdd");
	public static QKey<int> IntToRemove => QKey<int>.New("IntToRemove");
}

public static class C
{
	public static ICollection<int> Set(this QAcidContext ctx) => ctx.Get(K.Set);
	public static int IntToAdd(this QAcidContext ctx) => ctx.Get(K.IntToAdd);
	public static int IntToRemove(this QAcidContext ctx) => ctx.Get(K.IntToRemove);
}

public class SetTest
{
	[Fact(Skip = "Demo")]
	//[Fact]
	public void ReportsError()
	{
		var report =
			SystemSpecs.Define()
				.Tracked(K.Set, () => new List<int>())
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
				.DumpItInAcid()
				.KeepOneEyeOnTheTouchStone()
				.AndCheckForGold(30, 50);
		if (report != null)
			Assert.Fail(report.ToString());
	}


	[Fact(Skip = "investigate")]
	public void ReportsErrorTODO()
	{
		var report =
			SystemSpecs.Define()
				.Tracked(K.Set, () => new List<int>())
				.Fuzzed(K.IntToAdd, MGen.Int(0, 10))
				.Fuzzed(K.IntToRemove, MGen.Int(0, 10))
				.Options(opt => [
					opt.Do("Add", ctx => ctx.Set().Add(ctx.IntToAdd()))
							.Expect("Set contains added int")
							.Ensure(ctx => ctx.Get(K.Set).Contains(ctx.IntToAdd())),
						opt.Do("Remove", ctx =>
							{
								var toRemove = ctx.IntToRemove();
								ctx.Get(K.Set).Remove(toRemove);
							})
							.Expect("Set does not contain removed int")
							.Ensure(ctx => !ctx.Get(K.Set).Contains(ctx.IntToRemove()))
				])
				.DumpItInAcid()
				.AndCheckForGold(30, 50);
		Assert.NotNull(report);

		var actionEntries = report.Entries.OfType<ReportExecutionEntry>();
		var inputEntries = report.Entries.OfType<ReportInputEntry>();
		Assert.Equal(3, actionEntries.Count());
		Assert.Equal(3, inputEntries.Count());
	}
}
