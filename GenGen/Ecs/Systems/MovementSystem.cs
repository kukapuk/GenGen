using GenGen.ECS.Abstract;
using GenGen.ECS.Components;

namespace GenGen.ECS.Systems;

/// <summary>
/// Двигает все объекты у которых есть Transform и Velocity.
/// </summary>
public class MovementSystem(World world) : ISystem
{
	public void Update(float deltaTime)
	{
		foreach (var (_, transform, velocity) in world.Query<TransformComponent, VelocityComponent>())
			transform.Position += velocity.Velocity * deltaTime;
	}
}
