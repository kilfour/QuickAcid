namespace QuickAcid.Bolts;

public class QAcidResult<TValue>(QState state, TValue value)
{
	public QState State { get; set; } = state;
	public TValue Value { get; set; } = value;
}

public class QAcidResult
{
	public static QAcidResult<TValue> Some<TValue>(QState state, TValue value)
	{
		return new QAcidResult<TValue>(state, value);
	}

	public static QAcidResult<TValue> None<TValue>(QState state)
	{
		return new QAcidResult<TValue>(state, default);
	}

	public static QAcidResult<Acid> AcidOnly(QState state)
	{
		return Some(state, Acid.Test);
	}
}

