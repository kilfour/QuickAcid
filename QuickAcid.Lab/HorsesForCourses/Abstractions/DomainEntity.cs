using System.Runtime.CompilerServices;

namespace QuickAcid.Lab.HorsesForCourses.Abstractions;

public interface IDomainEntity { }

public abstract class DomainEntity<T> : IDomainEntity, IEquatable<DomainEntity<T>>
{
    public Id<T> Id { get; init; } = Id<T>.Empty;

    public bool IsTransient => Id.Equals(Id<T>.Empty);

    public bool Equals(DomainEntity<T>? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        // Transients are never equal to anything except themselves.
        if (IsTransient || other.IsTransient) return false;

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as DomainEntity<T>);

    public override int GetHashCode()
        // Use identity hash for transients; stable value hash once persisted.
        => IsTransient ? RuntimeHelpers.GetHashCode(this) : Id.GetHashCode();

    public static bool operator ==(DomainEntity<T>? left, DomainEntity<T>? right)
        => Equals(left, right);

    public static bool operator !=(DomainEntity<T>? left, DomainEntity<T>? right)
        => !Equals(left, right);

    public override string ToString() => $"{typeof(T).Name}#{(IsTransient ? "∅" : Id.Value.ToString())}";
}

