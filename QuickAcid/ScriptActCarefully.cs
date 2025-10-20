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
	public record ActCarefullyBuilder<TTypedInput> where TTypedInput : TypedScript
	{
		// private readonly Func<bool> predicate;

		// public ActCarefullyBuilder(Func<bool> predicate) { this.predicate = predicate; }

		public QAcidScript<QAcidDelayedResult<TValue>> With<TValue>(Func<TValue> func)
			=> TypedScript.LabelFromType(typeof(TTypedInput)).ActCarefully(func);
	}

	public static QAcidScript<QAcidDelayedResult> ActCarefully<TTypedInput>(Action action)
		where TTypedInput : Act
		=> TypedScript.LabelFromType(typeof(TTypedInput)).ActCarefully(action);

	public static ActCarefullyBuilder<TTypedInput> ActCarefully<TTypedInput>()
		where TTypedInput : Act => new();

}
