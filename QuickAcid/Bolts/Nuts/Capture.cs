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
					var value1 = state.Memory.ForThisExecution().Get<T>(key);
					return QAcidResult.Some(state, value1);
				}
				var value2 = func();
				state.Memory.ForThisExecution().Set(key, value2);
				return QAcidResult.Some(state, value2);
			};
	}
}
