using System;
using System.Linq;
using System.Collections.Generic;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator)
		{
			return state =>
			       	{
			       		if (state.Reporting)
			       		{
			       			var shrunk = state.Shrunk.Get<string>(key);
			       			if(shrunk!= "Irrelevant")
			       				state.LogReport(string.Format("'{0}' : {1}.", key, shrunk));
							return new QAcidResult<T>(state, state.Memory.Get<T>(key)) { Key = key };
			       		}

			       		if (state.Shrinking && !state.Shrunk.ContainsKey(key))
			       		{
			       			var value = state.Memory.Get<T>(key);
			       			ShrinkInput(state, key, value);
							return new QAcidResult<T>(state, state.Memory.Get<T>(key)) { Key = key };
			       		}

			       		if (state.Verifying)
			       		{
							return new QAcidResult<T>(state, state.Memory.Get<T>(key)) { Key = key };
			       		}

			       		var value2 = generator.Generate();
			       		state.Memory.Set(key, value2);
			       		return new QAcidResult<T>(state, value2);
			       	};
		}

		private readonly static Dictionary<Type, object[]> PrimitiveValues = 
			new Dictionary<Type, object[]>
				{
					{typeof(int), new object[] { -1, 0, 1 }},
					{typeof(string), new object[] { null, "", new string('-', 256), new string('-', 1024) }},
				};

		private static void ShrinkInput<T>(QAcidState state, object key, T value)
		{
			var shrunk = "Busy";
			state.Shrunk.Set(key, shrunk);
			if (typeof(IEnumerable<int>).IsAssignableFrom(typeof(T)))
			{
				shrunk = ShrinkIEnumerable(state, key, value);
			}
			var primitiveKey = PrimitiveValues.Keys.FirstOrDefault(k => k.IsAssignableFrom(typeof (T)));
			if(primitiveKey != null)
			{
				shrunk = ShrinkPrimitive(state, key, value, PrimitiveValues[primitiveKey]);
			}
			state.Shrunk.Set(key, shrunk);
		}

		private static string ShrinkIEnumerable<T>(QAcidState state, object key, T value)
		{
			var theList = ((IEnumerable<int>)value).ToList();
			int index = 0;
			while (index < theList.Count)
			{
				var ix = index;
				var before = theList[ix];
				var primitiveVals = new[] {-1, 0, 1};
				var removed = false;
				foreach (var primitiveVal in primitiveVals.Where(p => !p.Equals(before)))
				{
					theList[ix] = primitiveVal;
					var shrinkstate = state.ShrinkRun(key, theList);
					if (shrinkstate)
					{
						theList.RemoveAt(index);
						removed = true;
						break;
					}
				}
				if(!removed)
				{
					theList[ix] = before;
					index++;
				}
			}
			return string.Format("[ {0} ]", string.Join(", ", theList.Select(v => v.ToString())));
		}

		private static string ShrinkPrimitive(QAcidState state, object key, object value, IEnumerable<object> primitiveVals)
		{
			return
				primitiveVals
					.Select(primitiveVal => state.ShrinkRun(key, primitiveVal))
					.Any(shrinkstate => shrinkstate)
					? "Irrelevant"
					: value.ToString();
		}
	}
}
