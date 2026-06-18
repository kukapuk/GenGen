using GenGen.ECS.Abstract;
using Microsoft.Xna.Framework;

namespace GenGen.ECS.Components;

/// <summary>
/// Скорость объекта в пикселях в секунду.
/// </summary>
public class VelocityComponent(Vector2 velocity) : IComponent
{
	public Vector2 Velocity { get; set; } = velocity;
}
