using QuickAcid.CodeGen;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{

	public static QAcidRunner<T> Stashed<T>(this string key, Func<T> func)
	{
		return
			state =>
				{
					return QAcidResult.Some(state, state.Memory.StoreStashed(key, func));
				};
	}
}
