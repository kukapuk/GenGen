namespace GenGen.ECS.Abstract;

/// <summary>
/// Система логики. Вызывается в Update.
/// </summary>
public interface ISystem
{
	void Update(float deltaTime);
}
