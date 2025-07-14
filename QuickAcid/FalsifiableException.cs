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
		// Skip frames:
		// 1. This constructor
		// 2. The throw site in QuickAcid
		// 3. Any [StackTraceHidden] methods (they’re still counted!)
		// var stackTrace = new StackTrace(skipFrames: 4, fNeedFileInfo: true);
		// var frame = stackTrace.GetFrame(0);
		// var method = frame?.GetMethod();

		// var testMethod = method?.Name ?? "unknown_test";
		// var sourceFile = frame?.GetFileName() ?? "unknown_file";
		// var lineNumber = frame?.GetFileLineNumber() ?? 0;
		// report.AddTestMethodInfo(testMethod, sourceFile, lineNumber);
		// return report;
	}
}