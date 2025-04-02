namespace QuickAcid
{
    public static partial class QAcid
    {
        public static QAcidRunner<T> Sequence<T>(this string key, params QAcidRunner<T>[] runners)
        {
            var memory = new SimpleMemory();
            return
                s =>
                {
                    int value;
                    if (s.IsNormalRun())
                    {
                        value = memory.Get(key, -1);
                        value++;
                        if (value >= runners.Length)
                            value = 0;
                        memory.Set(key, value);
                    }
                    else
                        value = memory.Get<int>(key);
                    return runners[value](s);
                };
        }
    }
}
