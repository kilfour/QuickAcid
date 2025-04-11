namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Capture<T>(this string key, Func<T> func, Func<T, string> stringify = null)
	{
		return
			state =>
			{
				if (state.ShrinkingInputs || state.ShrinkingExecutions) // PHASERS ON STUN
				{
					var value1 = state.Memory.ForThisAction().Get<T>(key);
					return QAcidResult.Some(state, value1);
				}
				var value2 = func();
				state.Memory.ForThisAction().Set(key, value2);
				state.Memory.ForThisAction().MarkAsIrrelevant<T>(key);
				return QAcidResult.Some(state, value2);
			};
	}
}
