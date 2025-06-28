namespace QuickAcid;

public static partial class QAcidCombinators
{
    public static QAcidScript<T> Sequence<T>(this string key, params QAcidScript<T>[] scripts)
    {
        var counter = 0;
        return state =>
            {
                var factory = () =>
                    {
                        var value = counter;
                        counter = (counter + 1) % scripts.Length; // wraparound counter
                        return value;
                    };

                var index = state.Remember(key, factory);
                return scripts[index](state);
            };
    }
}
