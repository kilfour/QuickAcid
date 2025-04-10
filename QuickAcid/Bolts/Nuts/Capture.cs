namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Capture<T>(this string key, Func<T> func, Func<T, string> stringify = null)
	{
		return
			s =>
			{
				if (s.Shrinking || s.ShrinkingExecutions) // PHASERS ON STUN
				{
					var value1 = s.Memory.ForThisAction().Get<T>(key);
					return new QAcidResult<T>(s, value1);
				}
				var value2 = func();
				s.Memory.ForThisAction().Set(key, value2);
				s.Memory.ForThisAction().MarkAsIrrelevant<T>(key);
				return new QAcidResult<T>(s, value2);
			};
	}
}
