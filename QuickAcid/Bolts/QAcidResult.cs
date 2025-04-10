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
}

public class QAcidResult
{
	public static QAcidResult<TValue> Some<TValue>(QAcidState state, TValue value)
	{
		return new QAcidResult<TValue>(state, value);
	}

	public static QAcidResult<TValue> None<TValue>(QAcidState state)
	{
		return new QAcidResult<TValue>(state, default);
	}

	public static QAcidResult<Acid> AcidOnly(QAcidState state)
	{
		return QAcidResult.Some<Acid>(state, Acid.Test);
	}
}