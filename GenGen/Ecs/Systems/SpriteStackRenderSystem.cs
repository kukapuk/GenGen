using GenGen.ECS.Abstract;
using GenGen.ECS.Components;
using GenGen.Rendering;

namespace GenGen.ECS.Systems;

/// <summary>
/// Рисует все объекты у которых есть Transform и SpriteStack.
/// </summary>
public class SpriteStackRenderSystem(World world, SpriteStackRenderer renderer) : IRenderSystem
{
	public void Draw(float deltaTime)
	{
		foreach (var (_, transform, sprite) in world.Query<TransformComponent, SpriteStackComponent>())
			renderer.DrawStack(sprite.Layers, transform.Position, transform.Rotation, sprite.Scale);
	}
}
