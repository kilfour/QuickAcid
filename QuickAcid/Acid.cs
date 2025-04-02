namespace QuickAcid
{
    public struct Acid : IComparable<Acid>, IEquatable<Acid>
    {
        public static Acid Test { get; } = new Acid();

        public static bool operator ==(Acid left, Acid right)
        {
            return true;
        }

        public static bool operator !=(Acid left, Acid right)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Acid;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        int IComparable<Acid>.CompareTo(Acid other)
        {
            return 0;
        }

        bool IEquatable<Acid>.Equals(Acid other)
        {
            return true;
        }
    }
}