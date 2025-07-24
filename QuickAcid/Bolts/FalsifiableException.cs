using System.Diagnostics;
using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;


namespace QuickAcid.Bolts;

public class FalsifiableException : Exception
{
	public CaseFile CaseFile { get; private set; }

	public FalsifiableException(CaseFile caseFile)
		: base(Environment.NewLine + TheClerk.Transcribes(AddTestMethodInfo(caseFile)))
	{
		CaseFile = caseFile;
	}
	public FalsifiableException(CaseFile caseFile, Exception? exception)
		: base(Environment.NewLine + TheClerk.Transcribes(AddTestMethodInfo(caseFile)), exception)
	{
		CaseFile = caseFile;
	}

	private static CaseFile AddTestMethodInfo(CaseFile caseFile)
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
				caseFile.Verdict.AddTestMethodDisposition(
					new TestMethodInfoDeposition(
						method?.Name ?? "unknown_test", file, frame.GetFileLineNumber()));
				return caseFile;
			}
		}
		return caseFile;
	}
}