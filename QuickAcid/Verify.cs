using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static void Verify(this QAcidRunner<Unit> runner, int loops,int actions)
		{
			for (int i = 0; i < loops; i++)
			{
				var state = new QAcidState(runner);
				state.Run(actions);
				if (state.Failed)
					break;
			}
		}

	}
}
