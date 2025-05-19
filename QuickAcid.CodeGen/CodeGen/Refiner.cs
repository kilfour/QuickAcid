using QuickAcid.Bolts;

namespace QuickAcid.CodeGen
{
    public static class Refiner
    {
        public static void TheWohlwillProcess(this QAcidScript<Acid> script, int scopes, int executionsPerScope)
        {
            var failedExceptions = new List<Type>();
            var failedSpecs = new List<string>();
            var reports = new List<string>();
            var tests = new List<string>();
            for (int i = 0; i < scopes; i++)
            {
                var state = new QAcidState(script);
                state.Testify(executionsPerScope);
                if (state.CurrentContext.Failed)
                {
                    if (state.CurrentContext.Exception != null)
                    {
                        var exceptionType = state.CurrentContext.Exception.GetType();
                        if (failedExceptions.All(a => a != exceptionType))
                        {
                            //reports.Add(reports.ToString());
                            tests.Add(Prospector.Pan(state));
                            failedExceptions.Add(exceptionType);
                        }
                    }
                    if (!string.IsNullOrEmpty(state.CurrentContext.FailingSpec))
                    {
                        if (failedSpecs.All(a => a != state.CurrentContext.FailingSpec))
                        {
                            reports.Add(state.GetReport().ToString());
                            tests.Add(Prospector.Pan(state));
                            failedSpecs.Add(state.CurrentContext.FailingSpec);
                        }
                    }
                }
            }
        }
    }
}
