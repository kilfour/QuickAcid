using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Bolts;

public class QAcidResult<TValue>
{
	public QAcidState State { get; set; }
	public TValue Value { get; set; }
	public QAcidResult(QAcidState state, TValue value)
	{
		State = state;
		Value = value;
	}

	public static QAcidResult<TValue> Some(QAcidState state, TValue value)
	{
		return new QAcidResult<TValue>(state, value);
	}

	public static QAcidResult<TValue> None(QAcidState state)
	{
		return new QAcidResult<TValue>(state, default);
	}

}