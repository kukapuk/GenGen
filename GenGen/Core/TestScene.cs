using GenGen.Core.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GenGen.Rendering;

namespace GenGen.Core;

public class TestScene(SpriteStackRenderer renderer) : IScene
{
    private Texture2D[] _layers = [];
    private GraphicsDevice _graphicsDevice = null!;

    private Vector2 _position;
    private float _rotation;

    public void Initialize(GenGenGame game)
    {
        _graphicsDevice = game.GraphicsDevice;
        _position = new Vector2(
            game.GraphicsDevice.Viewport.Width / 2f,
            game.GraphicsDevice.Viewport.Height / 2f
        );

        _layers = GenerateTestLayers(8, 32);
    }

    public void Update(GameTime gameTime)
    {
        var kb = Keyboard.GetState();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (kb.IsKeyDown(Keys.Left))  _rotation -= 2f * dt;
        if (kb.IsKeyDown(Keys.Right)) _rotation += 2f * dt;
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        renderer.DrawStack(_layers, _position, _rotation, scale: 4f);
    }

    public void Dispose()
    {
        foreach (var t in _layers)
            t?.Dispose();
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
}
