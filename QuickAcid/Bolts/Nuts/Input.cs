﻿using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts.Nuts
{
	public static partial class QAcid
	{
		// USED TO CAPTURE INPUTS, MAINLY BY BOB
		public static QAcidRunner<T> Input<T>(this string key, Generator<T> generator)
		{
			return Input(key, generator.Generate);
		}

		public static QAcidRunner<T> InputIf<T>(this string key, Func<bool> predicate, Generator<T> generator)
		{
			if (!predicate())
				return s => new QAcidResult<T>(s, default!);
			return Input(key, generator.Generate);
		}

		public static QAcidRunner<T> Input<T>(this string key, Func<T> func, Func<T, string> stringify = null)
		{
			return
				s =>
				{
					if (s.Shrinking || s.ShrinkingExecutions) // PHASERS ON STUN
					{
						var value1 = s.Memory.ForThisAction().Get<T>(key);
						return new QAcidResult<T>(s, value1) { Key = key };
					}
					var value2 = func();
					s.Memory.ForThisAction().Set(key, value2);
					return new QAcidResult<T>(s, value2) { Key = key };
				};
		}
	}
}
