using QuickAcid.Reporting;

namespace QuickAcid.Bolts;

public class FalsifiableException : Exception
{
	public Report QAcidReport { get; set; }
	public FalsifiableException(string message)
		: base(message) { }
	public FalsifiableException(string message, Exception exception)
		: base(message, exception) { }
}