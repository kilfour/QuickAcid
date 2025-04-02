using QuickAcid.Shrinking;
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
							   if (shrunk != "Irrelevant")
							   {
								   var entry =
									   new QAcidReportInputEntry(key)
									   {
										   Value = shrunk
									   };
								   state.LogReport(entry);
							   }
							   return new QAcidResult<T>(state, state.Memory.Get<T>(key)) { Key = key };
						   }

						   if (state.Shrinking && !state.Shrunk.ContainsKey(key))
						   {
							   var value = state.Memory.Get<T>(key);
							   Shrink.Input(state, key, value);
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
	}
}
