using QuickAcid.Nuts;

namespace QuickAcid.Nuts.Bolts
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