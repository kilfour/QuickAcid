using System;

namespace QuickAcid
{
	public class FalsifiableException : Exception
	{
		public FalsifiableException(string message)
			: base(message) { }

		public FalsifiableException(string message, Exception exception)
			: base(message, exception) { }
	}
}