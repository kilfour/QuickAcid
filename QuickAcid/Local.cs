namespace QuickAcid
{
    public static partial class QAcid
    {
        public static QAcidRunner<T> LocalVar<T>(this string key, Func<T> func)
        {
            return
                s =>
                {
                    if (s.Reporting || s.Shrinking || s.Verifying)
                    {
                        var value1 = s.Memory.Get<T>(key);
                        return new QAcidResult<T>(s, value1) { Key = key };
                    }
                    var value2 = func();
                    s.Memory.Set(key, value2);
                    return new QAcidResult<T>(s, value2) { Key = key };
                };
        }
    }
}