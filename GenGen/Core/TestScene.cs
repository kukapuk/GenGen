using GenGen.Core.Abstract;
using GenGen.ECS;
using GenGen.ECS.Components;
using GenGen.ECS.Systems;
using GenGen.Editor;
using GenGen.Map;
using GenGen.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GenGen.Core;

/// <summary>
/// Тестовая сцена для проверки рендерера, карты и ECS.
/// </summary>
public class TestScene(SpriteStackRenderer renderer, SpriteBatch spriteBatch, ImGuiRenderer imGui) : IScene
{
	private TileMap _map = null!;
	private TileMapRenderer _mapRenderer = null!;
	private Texture2D _tileset = null!;
	private GraphicsDevice _graphicsDevice = null!;

	private World _world = null!;
	private SystemManager _systems = null!;
	private EditorLayer _editor = null!;
	private Entity _player;

	public void Initialize(GenGenGame game)
	{
		_graphicsDevice = game.GraphicsDevice;

		_map = new TileMap(20, 15, 16);
		FillTestMap();
		_tileset     = GenerateTestTileset(4, 16);
		_mapRenderer = new TileMapRenderer(spriteBatch);

		_world   = new World();
		_systems = new SystemManager();
		_editor  = new EditorLayer(_world);

		_systems.Add(new MovementSystem(_world));
		_systems.Add(new SpriteStackRenderSystem(_world, renderer));

		var center = new Vector2(
			game.GraphicsDevice.Viewport.Width  / 2f,
			game.GraphicsDevice.Viewport.Height / 2f
		);

		_player = _world.CreateEntity();
		_world.Add(_player, new TransformComponent(center));
		_world.Add(_player, new VelocityComponent(Vector2.Zero));
		_world.Add(_player, new SpriteStackComponent(GenerateTestLayers(8, 32), scale: 4f));
	}

	public void Update(GameTime gameTime)
	{
		var kb        = Keyboard.GetState();
		var dt        = (float)gameTime.ElapsedGameTime.TotalSeconds;
		var velocity  = _world.Get<VelocityComponent>(_player)!;
		var transform = _world.Get<TransformComponent>(_player)!;

		var dir = Vector2.Zero;
		if (kb.IsKeyDown(Keys.W)) dir.Y -= 1;
		if (kb.IsKeyDown(Keys.S)) dir.Y += 1;
		if (kb.IsKeyDown(Keys.A)) dir.X -= 1;
		if (kb.IsKeyDown(Keys.D)) dir.X += 1;

		velocity.Velocity = dir * 200f;

		if (kb.IsKeyDown(Keys.Left))  transform.Rotation -= 2f * dt;
		if (kb.IsKeyDown(Keys.Right)) transform.Rotation += 2f * dt;

		_systems.Update(dt);
	}

	public void DrawMap(SpriteBatch sb, GameTime gameTime)
	{
		var scale          = 3f;
		var mapPixelWidth  = _map.Width  * _map.TileSize * scale;
		var mapPixelHeight = _map.Height * _map.TileSize * scale;
		var offset = new Vector2(
			(_graphicsDevice.Viewport.Width  - mapPixelWidth)  / 2f,
			(_graphicsDevice.Viewport.Height - mapPixelHeight) / 2f
		);

		_mapRenderer.Draw(_map, _tileset, offset, scale);
	}

	public void DrawSprites(SpriteBatch sb, GameTime gameTime)
	{
		_systems.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
	}

	public void DrawEditor(GameTime gameTime)
	{
		_editor.Draw();
	}

	public void Dispose()
	{
		foreach (var (_, sprite) in _world.Query<SpriteStackComponent>())
			foreach (var layer in sprite.Layers)
				layer?.Dispose();

		_tileset?.Dispose();
	}

	private void FillTestMap()
	{
		var ground = _map.GetLayer("Ground")!;
		for (var y = 0; y < _map.Height; y++)
		for (var x = 0; x < _map.Width; x++)
			ground.Set(x, y, (x + y) % 2 == 0 ? 1 : 2);
	}

	private Texture2D[] GenerateTestLayers(int layerCount, int size)
	{
		var layers = new Texture2D[layerCount];

		for (var i = 0; i < layerCount; i++)
		{
			var tex    = new Texture2D(_graphicsDevice, size, size);
			var pixels = new Color[size * size];
			var t      = i / (float)(layerCount - 1);
			var color  = Color.Lerp(new Color(80, 40, 20), new Color(220, 160, 80), t);
			var center = new Vector2(size / 2f, size / 2f);

			for (var p = 0; p < pixels.Length; p++)
			{
				var px   = p % size;
				var py   = p / size;
				var dist = Vector2.Distance(new Vector2(px, py), center);
				pixels[p] = dist < size / 2f - 1 ? color : Color.Transparent;
			}

			tex.SetData(pixels);
			layers[i] = tex;
		}

		return layers;
	}

	private Texture2D GenerateTestTileset(int tileCount, int tileSize)
	{
		var tex    = new Texture2D(_graphicsDevice, tileCount * tileSize, tileSize);
		var pixels = new Color[tileCount * tileSize * tileSize];

		var colors = new Color[]
		{
			new(60,  120, 60),
			new(80,  60,  40),
			new(100, 100, 120),
			new(160, 140, 80),
		};

		for (var i = 0; i < tileCount; i++)
		for (var y = 0; y < tileSize; y++)
		for (var x = 0; x < tileSize; x++)
			pixels[y * tileCount * tileSize + i * tileSize + x] = colors[i % colors.Length];

		tex.SetData(pixels);
		return tex;
	}
}
