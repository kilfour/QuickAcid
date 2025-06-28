using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Choose<T>(this string key, params QAcidScript<T>[] scripts)
	{

		return state =>
			{
				var index = state.Remember(key, () => MGen.Int(0, scripts.Length).Generate(), ReportingIntent.Never);
				return scripts[index](state);
			};
	}
}