namespace QuickAcid
{
	public class FalsifiableException : Exception
	{
		public QAcidReport QAcidReport { get; set; }

		public FalsifiableException(string message)
			: base(message) { }

		public FalsifiableException(string message, Exception exception)
			: base(message, exception) { }
	}
}