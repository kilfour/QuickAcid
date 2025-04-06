using QuickAcid.Nuts;

namespace QuickAcid
{
	public delegate QAcidResult<TValue> QAcidRunner<TValue>(QAcidState state);
}