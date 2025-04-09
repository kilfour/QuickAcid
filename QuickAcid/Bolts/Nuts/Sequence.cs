namespace QuickAcid.Bolts.Nuts;

public static partial class QAcid
{
    public static QAcidRunner<T> Sequence<T>(this string key, params QAcidRunner<T>[] runners)
    {
        var counter = 0;
        var max = runners.Count();
        var memory = new Dictionary<string, int>();
        return
            s =>
            {
                int value;
                if (s.IsNormalRun()) // PHASERS ON STUN
                {
                    value = counter;
                    memory[key] = counter;
                    counter++;
                    if (counter >= max)
                        counter = 0;
                }
                else
                    value = memory[key];
                return runners[value](s);
            };
    }
}
