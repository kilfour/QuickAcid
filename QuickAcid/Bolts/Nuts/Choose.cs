﻿using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;
using QuickMGenerate;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Choose<T>(this string key, params QAcidRunner<T>[] runners)
	{
		return state =>
			{
				var index = state.Remember(key, () => MGen.Int(0, runners.Length).Generate());
				return runners[index](state);
			};
	}
}