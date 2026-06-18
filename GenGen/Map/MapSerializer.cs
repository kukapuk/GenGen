using System.Text.Json;

namespace GenGen.Map;

/// <summary>
/// Сохраняет и загружает TileMap в JSON.
/// </summary>
public static class MapSerializer
{
	private static readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true,
	};

	public static void Save(TileMap map, string path)
	{
		var json = JsonSerializer.Serialize(map, _options);
		File.WriteAllText(path, json);
	}

	public static TileMap? Load(string path)
	{
		if (!File.Exists(path)) return null;
		var json = File.ReadAllText(path);
		return JsonSerializer.Deserialize<TileMap>(json);
	}
}
