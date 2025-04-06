using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator)
		{
			return ShrinkableInput<T>(key, generator, _ => true);
		}

		public static QAcidRunner<T> ShrinkableInput<T>(this string key, Generator<T> generator, Func<T, bool> shrinkingGuard)
		{
			return state =>
				{
					if (state.Shrinking && !state.Shrunk.ForThisAction().ContainsKey(key))
					{
						var value = state.Memory.ForThisAction().Get<T>(key);
						var shrunk = Shrink.Input(state, key, value, obj => shrinkingGuard((T)obj));
						if (shrunk as string == "Irrelevant")
							state.Memory.ForThisAction().MarkAsIrrelevant<T>(key);
						else
							state.Memory.ForThisAction().AddReportingMessage<T>(key, shrunk.ToString()!);
						var valueAfterShrinking = state.Memory.ForThisAction().Get<T>(key);
						return new QAcidResult<T>(state, valueAfterShrinking) { Key = key };
					}

					if (state.Verifying)
					{
						return new QAcidResult<T>(state, state.Memory.ForThisAction().Get<T>(key)) { Key = key };
					}

					var value2 = generator.Generate();
					state.Memory.ForThisAction().Set(key, value2);
					return new QAcidResult<T>(state, value2);
				};
		}
	}
}
