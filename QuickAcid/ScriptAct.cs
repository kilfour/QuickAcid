using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using QuickAcid.Bolts;
using QuickAcid.TheyCanFade;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using StringExtensionCombinators;

namespace QuickAcid;

public static partial class Script
{
	public record ActBuilder<TTypedInput> where TTypedInput : TypedScript
	{
		private readonly Func<bool> predicate;

		public ActBuilder(Func<bool> predicate) { this.predicate = predicate; }

		public QAcidScript<TValue> With<TValue>(Func<TValue> func)
			=> TypedScript.LabelFromType(typeof(TTypedInput)).ActIf(predicate, func);
	}

	public static QAcidScript<Acid> Act<TTypedInput>(Action action)
		where TTypedInput : Act
		=> TypedScript.LabelFromType(typeof(TTypedInput)).Act(action);

	public static ActBuilder<TTypedInput> Act<TTypedInput>()
		where TTypedInput : Act => new(() => true);

	public static QAcidScript<Acid> ActIf<TTypedInput>(Func<bool> predicate, Action action)
		where TTypedInput : Act
		=> TypedScript.LabelFromType(typeof(TTypedInput)).ActIf(predicate, action);

	public static ActBuilder<TTypedInput> ActIf<TTypedInput>(Func<bool> predicate)
		where TTypedInput : Act => new(predicate);
}
