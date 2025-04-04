namespace QuickAcid;
public readonly struct Acid : IComparable<Acid>, IEquatable<Acid>
{
    public static Acid Test { get; } = new Acid();
    public static bool operator ==(Acid _, Acid __) => true;
    public static bool operator !=(Acid _, Acid __) => false;
    public override bool Equals(object? obj) => obj is Acid;
    public bool Equals(Acid other) => true;
    public int CompareTo(Acid other) => 0;
    public override int GetHashCode() => 0;
}