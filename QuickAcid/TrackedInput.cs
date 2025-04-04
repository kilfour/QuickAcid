using QuickAcid.CodeGen;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> TrackedInput<T>(this string key, Func<T> func)
		{
			return TrackedInput(key, func, t => t == null ? "null" : t.ToString());
		}

		public static QAcidRunner<T> TrackedInput<T>(this string key, Func<T> func, Func<T, string> stringify)
		{
			return
				s =>
					{
						s.XMarksTheSpot.TheTracker =
							new Tracker { Key = key, RunnerType = RunnerType.TrackedInputRunner };

						var value = s.Memory.TrackedInputsPerRun.GetOrAdd(key, func, stringify);
						if (!s.Shrinking)
							s.Memory.AddTrackedInputValueForCurrentRun(key, stringify(value));
						return new QAcidResult<T>(s, value) { Key = key };
					};
		}
	}
}
