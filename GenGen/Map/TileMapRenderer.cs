using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.Map;

/// <summary>
/// Рисует TileMap на экране.
/// </summary>
public class TileMapRenderer(SpriteBatch spriteBatch)
{
	public void Draw(TileMap map, Texture2D tileset, Vector2 offset = default, float scale = 1f)
	{
		var scaledTile = (int)(map.TileSize * scale);

		foreach (var layer in map.Layers)
		{
			if (!layer.Visible) continue;

			for (var y = 0; y < map.Height; y++)
			{
				for (var x = 0; x < map.Width; x++)
				{
					var tileId = layer.Get(x, y);
					if (tileId == 0) continue;

					var srcX = (tileId - 1) % (tileset.Width / map.TileSize);
					var srcY = (tileId - 1) / (tileset.Width / map.TileSize);

					var src = new Rectangle(
						srcX * map.TileSize,
						srcY * map.TileSize,
						map.TileSize,
						map.TileSize
					);

					var dst = new Rectangle(
						(int)(offset.X + x * scaledTile),
						(int)(offset.Y + y * scaledTile),
						scaledTile,
						scaledTile
					);

					spriteBatch.Draw(tileset, dst, src, Color.White);
				}
			}
		}
	}
}
