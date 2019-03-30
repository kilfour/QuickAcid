namespace QuickAcid
{
    public static partial class QAcid
	{
	    public static void Verify(this QAcidRunner<Acid> runner)
	    {
	        new QAcidState(runner).Run(1);
        }

        public static void Verify(this QAcidRunner<Acid> runner, int loops, int actions)
		{
			for (int i = 0; i < loops; i++)
			{
				var state = new QAcidState(runner);
				state.Run(actions);
				if (state.Failed)
					break;
			}
		}

	    public static void Verify(this QAcidRunner<Acid> runner, NumberOfRuns numberOfRuns, ActionsPerRun actionsPerRun)
	    {
	        for (int i = 0; i < numberOfRuns.Value; i++)
	        {
	            var state = new QAcidState(runner);
	            state.Run(actionsPerRun.Value);
	            if (state.Failed)
	                break;
	        }
	    }

	    public static NumberOfRuns Runs(this int number)
	    {
	        return new NumberOfRuns(number);
	    }

	    public static ActionsPerRun ActionsPerRun(this int number)
	    {
	        return new ActionsPerRun(number);
	    }
    }

    public class NumberOfRuns
    {
        public int Value { get; }
        public NumberOfRuns(int value) { Value = value; }
    }

    public class ActionsPerRun
    {
        public int Value { get; }
        public ActionsPerRun(int value) { Value = value; }
    }
}
