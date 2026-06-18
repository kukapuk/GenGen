namespace GenGen.ECS;

/// <summary>
/// Уникальный идентификатор объекта в мире.
/// </summary>
public readonly struct Entity(int id) : IEquatable<Entity>
{
	public int Id { get; } = id;

	public static readonly Entity None = new(-1);

	public bool Equals(Entity other) => Id == other.Id;
	public override bool Equals(object? obj) => obj is Entity e && Equals(e);
	public override int GetHashCode() => Id;
	public override string ToString() => $"Entity({Id})";

	public static bool operator ==(Entity a, Entity b) => a.Id == b.Id;
	public static bool operator !=(Entity a, Entity b) => a.Id != b.Id;
}
