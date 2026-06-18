using Microsoft.Xna.Framework;

namespace GenGen.Map;

/// <summary>
/// Тайловая карта. Хранит слои тайлов и умеет сериализоваться в JSON.
/// </summary>
public class TileMap
{
	public int Width { get; init; }
	public int Height { get; init; }
	public int TileSize { get; init; } = 16;

	public List<TileLayer> Layers { get; init; } = [];

	public TileMap(int width, int height, int tileSize = 16)
	{
		Width = width;
		Height = height;
		TileSize = tileSize;
		Layers.Add(new TileLayer("Ground", width, height));
	}

	public TileLayer? GetLayer(string name) =>
		Layers.FirstOrDefault(l => l.Name == name);

	public void AddLayer(string name) =>
		Layers.Add(new TileLayer(name, Width, Height));
}
