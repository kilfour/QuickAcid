using QuickMGenerate;

namespace QuickAcid.Bolts.Nuts
{
	public static partial class QAcid
	{
		public static QAcidRunner<T> Choose<T>(this string key, params QAcidRunner<T>[] runners)
		{
			return
				s =>
				{
					int value;
					if (s.IsNormalRun())
					{
						value = MGen.Int(0, runners.Length).Generate();
						var thisActionsMemory = s.Memory.ForThisAction();
						thisActionsMemory.Set(key, value);
						thisActionsMemory.MarkAsIrrelevant<T>(key);
					}
					value = s.Memory.ForThisAction().Get<int>(key);
					return runners[value](s);
				};
		}
	}
}