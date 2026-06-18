using GenGen.ECS.Abstract;

namespace GenGen.ECS;

/// <summary>
/// Хранит все энтити и компоненты. Центральная точка ECS.
/// </summary>
public class World
{
	private readonly Dictionary<Type, Dictionary<int, IComponent>> _components = new();
	private readonly HashSet<int> _entities = [];
	private int _nextId;

	/// <summary>
	/// Создаёт новый энтити и возвращает его.
	/// </summary>
	public Entity CreateEntity()
	{
		var entity = new Entity(_nextId++);
		_entities.Add(entity.Id);
		return entity;
	}

	/// <summary>
	/// Удаляет энтити и все его компоненты.
	/// </summary>
	public void DestroyEntity(Entity entity)
	{
		foreach (var store in _components.Values)
			store.Remove(entity.Id);

		_entities.Remove(entity.Id);
	}

	/// <summary>
	/// Добавляет компонент к энтити.
	/// </summary>
	public void Add<T>(Entity entity, T component) where T : IComponent
	{
		var type = typeof(T);

		if (!_components.TryGetValue(type, out var store))
		{
			store = new Dictionary<int, IComponent>();
			_components[type] = store;
		}

		store[entity.Id] = component;
	}

	/// <summary>
	/// Возвращает компонент энтити или null если не найден.
	/// </summary>
	public T? Get<T>(Entity entity) where T : class, IComponent
	{
		if (_components.TryGetValue(typeof(T), out var store) &&
		    store.TryGetValue(entity.Id, out var component))
			return (T)component;

		return null;
	}

	/// <summary>
	/// Проверяет наличие компонента у энтити.
	/// </summary>
	public bool Has<T>(Entity entity) where T : IComponent =>
		_components.TryGetValue(typeof(T), out var store) && store.ContainsKey(entity.Id);

	/// <summary>
	/// Удаляет компонент с энтити.
	/// </summary>
	public void Remove<T>(Entity entity) where T : IComponent
	{
		if (_components.TryGetValue(typeof(T), out var store))
			store.Remove(entity.Id);
	}

	/// <summary>
	/// Возвращает все энтити у которых есть компонент T.
	/// </summary>
	public IEnumerable<(Entity, T)> Query<T>() where T : class, IComponent
	{
		if (!_components.TryGetValue(typeof(T), out var store))
			yield break;

		foreach (var (id, component) in store)
			yield return (new Entity(id), (T)component);
	}

	/// <summary>
	/// Возвращает все энтити у которых есть оба компонента.
	/// </summary>
	public IEnumerable<(Entity, T1, T2)> Query<T1, T2>()
		where T1 : class, IComponent
		where T2 : class, IComponent
	{
		if (!_components.TryGetValue(typeof(T1), out var store))
			yield break;

		foreach (var (id, c1) in store)
		{
			var entity = new Entity(id);

			if (Get<T2>(entity) is { } c2)
				yield return (entity, (T1)c1, c2);
		}
	}

	/// <summary>
	/// Возвращает все энтити у которых есть все три компонента.
	/// </summary>
	public IEnumerable<(Entity, T1, T2, T3)> Query<T1, T2, T3>()
		where T1 : class, IComponent
		where T2 : class, IComponent
		where T3 : class, IComponent
	{
		if (!_components.TryGetValue(typeof(T1), out var store))
			yield break;

		foreach (var (id, c1) in store)
		{
			var entity = new Entity(id);

			if (Get<T2>(entity) is { } c2 && Get<T3>(entity) is { } c3)
				yield return (entity, (T1)c1, c2, c3);
		}
	}
}
