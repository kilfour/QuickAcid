using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid
{
	public delegate QAcidResult<TValue> QAcidRunner<TValue>(QAcidState state);
}