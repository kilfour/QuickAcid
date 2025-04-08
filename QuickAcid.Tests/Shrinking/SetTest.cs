﻿using QuickMGenerate;
using QuickAcid.Fluent;
using QuickAcid;


namespace QuickAcid.Tests.Shrinking
{
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

	public class SetTest : QAcidLoggingFixture
	{
		[Fact]
		public void ReportsError()
		{
			var lastRemovedInt = 0;
			var report =
				SystemSpecs.Define()
					.AlwaysReported(K.Set, () => new List<int>(), l => string.Join(" ", l.Select(item => item.ToString()).ToArray()))
					.Fuzzed(K.IntToAdd, MGen.Int(2, 4))
					.Fuzzed(K.IntToRemove, MGen.Int(2, 4))
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
					.PickOne()
					.DumpItInAcid()
					.KeepOneEyeOnTheTouchStone()
					.AndCheckForGold(30, 50);

			Assert.NotNull(report);

			if (report != null)
				Assert.Fail(report.ToString());
		}
	}
}
