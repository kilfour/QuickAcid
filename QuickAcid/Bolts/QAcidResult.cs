namespace QuickAcid.Bolts;

public class QAcidResult<TValue>(QAcidState state, TValue value)
{
	public QAcidState State { get; set; } = state;
	public TValue Value { get; set; } = value;
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
		return Some(state, Acid.Test);
	}
}

