namespace GenGen.Map;

/// <summary>
/// Один слой тайлов внутри карты.
/// </summary>
public class TileLayer(string name, int width, int height)
{
	public string Name { get; set; } = name;
	public bool Visible { get; set; } = true;

	// Плоский массив тайлов: [y * width + x]
	// 0 = пусто, остальное — ID тайла
	public int[] Tiles { get; init; } = new int[width * height];

	public int Get(int x, int y) => Tiles[y * width + x];
	public void Set(int x, int y, int tileId) => Tiles[y * width + x] = tileId;
}
