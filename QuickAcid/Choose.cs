﻿using QuickAcid.Bolts.TheyCanFade;
using QuickFuzzr;

namespace QuickAcid;

public static partial class QAcidCombinators
{
	public static QAcidScript<T> Choose<T>(this string key, params QAcidScript<T>[] scripts)
	{

		return state =>
			{
				var index = state.Remember(key, () => Fuzz.Int(0, scripts.Length)(state.FuzzState).Value, ReportingIntent.Never);
				return scripts[index](state);
			};
	}
}