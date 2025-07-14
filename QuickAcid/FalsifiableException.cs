using QuickAcid.Reporting;

namespace QuickAcid;

public class FalsifiableException : Exception
{
	public Report QAcidReport { get; private set; }
	public FalsifiableException(Report report)
		: base(report.ToString()) { QAcidReport = report; }
	public FalsifiableException(Report report, Exception? exception)
		: base(report.ToString(), exception) { QAcidReport = report; }
}