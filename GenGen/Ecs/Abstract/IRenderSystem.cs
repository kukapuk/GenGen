namespace GenGen.ECS.Abstract;

/// <summary>
/// Система рендеринга. Вызывается в Draw, когда SpriteBatch уже открыт.
/// </summary>
public interface IRenderSystem
{
	void Draw(float deltaTime);
}
