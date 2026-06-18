using GenGen.ECS.Abstract;
using Microsoft.Xna.Framework;

namespace GenGen.ECS.Components;

/// <summary>
/// Позиция и поворот объекта в мире.
/// </summary>
public class TransformComponent(Vector2 position, float rotation = 0f) : IComponent
{
	public Vector2 Position { get; set; } = position;
	public float Rotation { get; set; } = rotation;
}
