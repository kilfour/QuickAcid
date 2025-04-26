using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Choose<T>(this string key, params QAcidRunner<T>[] runners)
	{

		return state =>
			{
				var index = state.Remember(key, () => MGen.Int(0, runners.Length).Generate(), ReportingIntent.Never);
				return runners[index](state);
			};
	}

	// public static QAcidRunner<Acid> ChooseWisely(
	// this string key,
	// params (Func<bool> Guard, QAcidRunner<Acid> Runner)[] choices)
	// {
	// 	return state =>
	// 	{
	// 		// Always remember the INDEX among *all* choices, not filtered ones
	// 		var index = state.Remember(key, () =>
	// 		{
	// 			var validChoices = choices
	// 				.Select((choice, i) => (Choice: choice, Index: i))
	// 				.Where(c => c.Choice.Guard())
	// 				.ToList();

	// 			if (!validChoices.Any())
	// 				return -1; // Special marker

	// 			var picked = MGen.Int(0, validChoices.Count).Generate();
	// 			return validChoices[picked].Index; // Remember original all-choices index
	// 		});

	// 		if (index == -1)
	// 			return QAcidResult.None<Acid>(state);

	// 		// Replay: use original choices list, ignore current guards
	// 		return choices[index].Runner(state);
	// 	};
	// }
}