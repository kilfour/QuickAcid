using System.Diagnostics;
using QuickAcid.Reporting;

namespace QuickAcid;

public class FalsifiableException : Exception
{
	public Report QAcidReport { get; private set; }

	public FalsifiableException(Report report)
		: base(AddTestMethodInfo(report).ToString())
	{
		QAcidReport = report;
	}
	public FalsifiableException(Report report, Exception? exception)
		: base(AddTestMethodInfo(report).ToString(), exception)
	{
		QAcidReport = report;
	}

	private static Report AddTestMethodInfo(Report report)
	{
		var quickAcidAssembly = typeof(Acid).Assembly;
		var trace = new StackTrace(fNeedFileInfo: true);
		foreach (var frame in trace.GetFrames() ?? Array.Empty<StackFrame>())
		{
			var file = frame.GetFileName();
			if (!string.IsNullOrEmpty(file))
			{
				var method = frame.GetMethod();
				if (method == null) continue;
				var declaringAssembly = method.DeclaringType?.Assembly;
				if (declaringAssembly == quickAcidAssembly) continue;
				report.AddTestMethodInfo(method?.Name ?? "unknown_test", file, frame.GetFileLineNumber());
				return report;
			}
		}
		return report;
	}
}