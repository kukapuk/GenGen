using GenGen.ECS.Abstract;

namespace GenGen.ECS;

/// <summary>
/// Хранит и запускает системы в порядке добавления.
/// </summary>
public class SystemManager
{
	private readonly List<ISystem> _systems = [];
	private readonly List<IRenderSystem> _renderSystems = [];

	public void Add(ISystem system) => _systems.Add(system);
	public void Add(IRenderSystem system) => _renderSystems.Add(system);
	public void Remove(ISystem system) => _systems.Remove(system);
	public void Remove(IRenderSystem system) => _renderSystems.Remove(system);

	/// <summary>
	/// Обновляет все системы логики.
	/// </summary>
	public void Update(float deltaTime)
	{
		foreach (var system in _systems)
			system.Update(deltaTime);
	}

	/// <summary>
	/// Обновляет все системы рендеринга. Вызывать только между Begin/End SpriteBatch.
	/// </summary>
	public void Draw(float deltaTime)
	{
		foreach (var system in _renderSystems)
			system.Draw(deltaTime);
	}
}
