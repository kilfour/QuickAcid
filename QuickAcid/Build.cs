using System;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<Unit> Build(Action action)
		{
			return
				s =>
					{
						action();
						return new QAcidResult<Unit>(s, Unit.Instance);
					};
		}
	}
}