using Flunt.Notifications;
using System.Diagnostics.CodeAnalysis;

namespace Octo.Core.Domain;

[SuppressMessage(
    "Major Code Smell",
    "S4035:Classes implementing 'IEquatable<T>' should be sealed",
    Justification = "Base domain entity class intended to be inherited; concrete entities share identity semantics through this base.")]
public abstract class Entity<TId> : Notifiable<Notification>, IEquatable<Entity<TId>>
{    
    public TId Id { get; protected set; } = default!;

    protected Entity(TId id)
    {
        if (EqualityComparer<TId>.Default.Equals(id, default!))
            throw new ArgumentException("O ID da entidade não pode ser nulo ou padrão.", nameof(id));

        Id = id;
    }

    protected Entity() { }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);
    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id!);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);
}