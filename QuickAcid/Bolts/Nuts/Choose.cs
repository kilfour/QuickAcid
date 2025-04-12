using QuickMGenerate;

namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
	public static QAcidRunner<T> Choose<T>(this string key, params QAcidRunner<T>[] runners)
	{
		return
			s =>
			{
				int value;
				if (s.IsNormalRun()) // PHASERS ON STUN
				{
					value = MGen.Int(0, runners.Length).Generate();
					var thisActionsMemory = s.Memory.ForThisExecution();
					thisActionsMemory.Set(key, value);
					thisActionsMemory.MarkAsIrrelevant<T>(key); // why ?
				}
				value = s.Memory.ForThisExecution().Get<int>(key);
				return runners[value](s);
			};
	}
}