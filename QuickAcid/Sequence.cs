using QuickMGenerate;

namespace QuickAcid
{
    public static partial class QAcid
    {
        public static QAcidRunner<T> Sequence<T>(this string key, params QAcidRunner<T>[] runners)
        {
            return
                s =>
                {
                    int value;
                    if (s.IsNormalRun())
                    {
                        value = s.GlobalMemory.Get(key, -1);
                        value++;
                        s.GlobalMemory.Set(key, value);
                    }
                    else
                        value = s.GlobalMemory.Get<int>(key);
                    return runners[value](s);
                };
        }
    }
}
