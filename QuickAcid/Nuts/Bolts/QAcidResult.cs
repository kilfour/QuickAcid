using QuickAcid.Nuts;

namespace QuickAcid
{
	public class QAcidResult<TValue>
	{
		public QAcidState State { get; set; }
		public TValue Value { get; set; }
		public string Key { get; set; }
		public QAcidResult(QAcidState state, TValue value)
		{
			State = state;
			Value = value;
		}
	}

	public static class AcidResult
	{
		// public static QAcidResult<Acid> Pass(QAcidState state)
		//     => new(state, Acid.Test);

		// public static QAcidResult<Acid> Fail(QAcidState state)
		//     => new(state.Failed(), Acid.Test);

		public static QAcidRunner<Acid> Skip()
			=> state => new(state, Acid.Test); // maybe track "skipped" somehow?
	}
}