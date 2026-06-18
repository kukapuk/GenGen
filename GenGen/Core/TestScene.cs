using GenGen.Core.Abstract;
using GenGen.Map;
using GenGen.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GenGen.Core;

/// <summary>
/// Тестовая сцена для проверки рендерера и карты.
/// </summary>
public class TestScene(SpriteStackRenderer renderer, SpriteBatch spriteBatch) : IScene
{
    private Texture2D[] _layers = [];
    private TileMap _map = null!;
    private TileMapRenderer _mapRenderer = null!;
    private Texture2D _tileset = null!;
    private GraphicsDevice _graphicsDevice = null!;
    private Vector2 _position;
    private float _rotation = 0f;

    public void Initialize(GenGenGame game)
    {
        _graphicsDevice = game.GraphicsDevice;
        _position = new Vector2(
            game.GraphicsDevice.Viewport.Width / 2f,
            game.GraphicsDevice.Viewport.Height / 2f
        );

        _layers = GenerateTestLayers(8, 32);

        _map = new TileMap(20, 15, 16);
        FillTestMap();

        _tileset = GenerateTestTileset(4, 16);
        _mapRenderer = new TileMapRenderer(spriteBatch);
    }

    public void Update(GameTime gameTime)
    {
        var kb = Keyboard.GetState();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (kb.IsKeyDown(Keys.Left))  _rotation -= 2f * dt;
        if (kb.IsKeyDown(Keys.Right)) _rotation += 2f * dt;
    }

    public void DrawMap(SpriteBatch sb, GameTime gameTime)
    {
        var scale = 3f;
        var mapPixelWidth = _map.Width * _map.TileSize * scale;
        var mapPixelHeight = _map.Height * _map.TileSize * scale;
        var offset = new Vector2(
            (_graphicsDevice.Viewport.Width - mapPixelWidth) / 2f,
            (_graphicsDevice.Viewport.Height - mapPixelHeight) / 2f
        );

        _mapRenderer.Draw(_map, _tileset, offset, scale);
    }

    public void DrawSprites(SpriteBatch sb, GameTime gameTime)
    {
        renderer.DrawStack(_layers, _position, _rotation, scale: 4f);
    }

    public void Dispose()
    {
        foreach (var t in _layers) t?.Dispose();
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
            var tex = new Texture2D(_graphicsDevice, size, size);
            var pixels = new Color[size * size];
            var t = i / (float)(layerCount - 1);
            var color = Color.Lerp(new Color(80, 40, 20), new Color(220, 160, 80), t);
            var center = new Vector2(size / 2f, size / 2f);

            for (var p = 0; p < pixels.Length; p++)
            {
                var px = p % size;
                var py = p / size;
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
        var tex = new Texture2D(_graphicsDevice, tileCount * tileSize, tileSize);
        var pixels = new Color[tileCount * tileSize * tileSize];

        var colors = new Color[]
        {
            new(60, 120, 60),
            new(80, 60, 40),
            new(100, 100, 120),
            new(160, 140, 80),
        };

        for (var i = 0; i < tileCount; i++)
        {
            for (var y = 0; y < tileSize; y++)
            for (var x = 0; x < tileSize; x++)
                pixels[y * tileCount * tileSize + i * tileSize + x] = colors[i % colors.Length];
        }

        tex.SetData(pixels);
        return tex;
    }
}
