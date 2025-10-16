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
	public static QAcidScript<Acid> Spec<TTypedInput>(Func<bool> condition)
		where TTypedInput : Spec
		=> TypedScript.LabelFromType(typeof(TTypedInput)).Spec(condition);
}