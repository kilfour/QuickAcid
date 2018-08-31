using System;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<Acid> Spec(this string key, Func<bool> func)
		{
		    return
		        state =>
		        {
		            if (state.Reporting)
		            {
		                if (state.FailingSpec == key)
		                {
		                    state.LogReport(new QAcidReportSpecEntry(key));
		                }
		                return new QAcidResult<Acid>(state, Acid.Test);
		            }

		            if (state.FailingSpec != null && state.FailingSpec != key)
		            {
		                return new QAcidResult<Acid>(state, Acid.Test);
		            }

		            if (state.Verifying && state.FailingSpec == null)
		            {
		                return new QAcidResult<Acid>(state, Acid.Test);
		            }

		            var result = func();
		            if (!result)
		            {
		                state.SpecFailed(key);
		            }
		            return new QAcidResult<Acid>(state, Acid.Test);
		        };
		}
	}
}