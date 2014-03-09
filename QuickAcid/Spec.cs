using System;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid
{
	public static partial class QAcid
	{
		public static QAcidRunner<Unit> Spec(this string key, Func<bool> func)
		{
			return Spec((object) key, func);
		}

		public static QAcidRunner<Unit> Spec(object key, Func<bool> func)
		{
			return s =>
			       	{
						if(s.Reporting)
						{
							if (s.FailingSpec == key)
								s.LogReport(string.Format("Failing Spec : '{0}'.", key));
							return new QAcidResult<Unit>(s, Unit.Instance);
						}

						if (s.FailingSpec != null && s.FailingSpec != key)
						{
							return new QAcidResult<Unit>(s, Unit.Instance);
						}

						if(s.Verifying && s.FailingSpec == null)
						{
							return new QAcidResult<Unit>(s, Unit.Instance);	
						}

						var result = func();
						if (!result)
						{
							s.FailingSpec = key;
							s.Failed = true;
						}
			       		return new QAcidResult<Unit>(s, Unit.Instance);
			       	};
		}
	}
}