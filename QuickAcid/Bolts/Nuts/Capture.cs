namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Capture<T>(this string key, Func<T> func, Func<T, string> stringify = null)
	{
		return state =>
		{
			// var value = state.Remember(key, factory);

			// if (stringify != null)
			// 	state.GetExecutionContext().SetReportingMessage(key, stringify(value));

			return QAcidResult.Some(state, state.Remember(key, func));
		};
		// return
		// 	state =>
		// 	{
		// 		if (state.CurrentPhase == QAcidPhase.ShrinkingInputs || state.CurrentPhase == QAcidPhase.ShrinkingExecutions)
		// 		{
		// 			var value1 = state.Memory.ForThisExecution().Get<T>(key);
		// 			return QAcidResult.Some(state, value1);
		// 		}
		// 		var value2 = func();
		// 		state.Memory.ForThisExecution().Set(key, value2);
		// 		return QAcidResult.Some(state, value2);
		// 	};
	}
}
