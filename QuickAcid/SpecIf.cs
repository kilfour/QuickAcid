using QuickAcid.CodeGen;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<Acid> SpecIf(this string key, Func<bool> predicate, Func<bool> func)
		{
			return
				state =>
				{
					if (!predicate())
						return new QAcidResult<Acid>(state, Acid.Test);
					return key.Spec(func)(state);
				};
		}
	}
}