using System;
using System.Collections.Generic;
using System.Linq;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Tests
{
	public static class GeneratorExt
	{
		public static Generator<List<T>> ToList<T>(this Generator<IEnumerable<T>> generator)
		{
			return s => new Result<List<T>>(generator(s).Value.ToList(), s);
		}

		public static Generator<T> SameValueAlways<T>(this Generator<T> generator)
		{
			var func = Memoize(s => generator(s).Value);
			return s => new Result<T>(func(s), s);

		}

		public static Func<State, T> Memoize<T>(this Func<State, T> f)
		{
			T value = default(T);
			bool hasValue = false;
			return s =>
			       	{
			       		if (!hasValue)
			       		{
			       			hasValue = true;
			       			value = f(s);
			       		}
			       		return value;
			       	};
		}
	}
}