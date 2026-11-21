using Flunt.Notifications;

namespace Octo.Core.Domain;

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
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);
    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id!);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);
}