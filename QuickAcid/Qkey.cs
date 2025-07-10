namespace QuickAcid
{
    public readonly struct QKey<T>
    {
        public string Label { get; }
        public QKey(string label) { Label = label; }
        public override string ToString() => Label;
        public static QKey<T> New(string label) => new QKey<T>(label);
    }
}
