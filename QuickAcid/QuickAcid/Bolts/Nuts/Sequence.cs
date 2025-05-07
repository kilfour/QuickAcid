namespace QuickAcid.Bolts.Nuts;

public static partial class QAcidCombinators
{
    public static QAcidRunner<T> Sequence<T>(this string key, params QAcidRunner<T>[] runners)
    {
        var counter = 0;
        return state =>
            {
                var factory = () =>
                    {
                        var value = counter;
                        counter = (counter + 1) % runners.Length;
                        return value;
                    };

                var index = state.Remember(key, factory);
                return runners[index](state);
            };
    }
}
