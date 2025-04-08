using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid
{
	public delegate QAcidResult<TValue> QAcidRunner<TValue>(QAcidState state);
}