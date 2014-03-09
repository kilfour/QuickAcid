using System;
using QuickMGenerate;
using Xunit;

namespace QuickAcid.Tests
{
	public class MemoTest
	{
		[Fact(Skip="Not Now")]
		public void Memo()
		{
			var list = MGen.Int().SameValueAlways().Many(10).Generate();
			foreach (var i in list)
			{
				Console.WriteLine(i);
			}
		}

		[Fact]
		public void OtherMemo()
		{
			var myGenerator = MGen.Int().SameValueAlways();
			var generator =
				from a in myGenerator
				from b in myGenerator
				select new {a, b};
			var result = generator.Generate();
			Assert.Equal(result.a, result.b);
		}
	}
}