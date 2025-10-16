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
	public static InputBuilder<TTypedInput> Input<TTypedInput>()
		where TTypedInput : Input => new();

	public record InputBuilder<TTypedInput>()
		where TTypedInput : TypedScript
	{
		public QAcidScript<TValue> With<TValue>(Generator<TValue> generator)
			=> TypedScript.LabelFromType(typeof(TTypedInput)).Input(generator);
	}
}