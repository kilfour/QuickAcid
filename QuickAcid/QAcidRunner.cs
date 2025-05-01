using QuickAcid.Bolts;

namespace QuickAcid
{
	public delegate QAcidResult<TValue> QAcidRunner<TValue>(QState state);
}