using QuickAcid.Bolts;

namespace QuickAcid.Bolts.Nuts
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
}