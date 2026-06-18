using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.Rendering;

/// <summary>
/// Рисует SpriteStack — набор слоёв PNG, смещённых по Y,
/// создавая псевдо-3D эффект. Поддерживает поворот всего стека.
/// </summary>
public class SpriteStackRenderer
{
    private SpriteBatch _spriteBatch = null!;

    public float LayerSpacing { get; set; } = 1.5f;

    public void Initialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    /// <summary>
    /// Рисует стек спрайтов.
    /// </summary>
    /// <param name="layers">Слои снизу вверх (layers[0] — основание)</param>
    /// <param name="position">Позиция на экране</param>
    /// <param name="rotation">Поворот в радианах</param>
    /// <param name="scale">Масштаб</param>
    /// <param name="color">Цветовой тинт</param>
    public void DrawStack(
        Texture2D[] layers,
        Vector2 position,
        float rotation = 0f,
        float scale = 1f,
        Color? color = null)
    {
        var tint = color ?? Color.White;

        for (var i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];

            var origin = new Vector2(layer.Width / 2f, layer.Height / 2f);

            var yOffset = i * LayerSpacing * scale;

            var offset = new Vector2(
                -MathF.Sin(rotation) * yOffset,
                -MathF.Cos(rotation) * yOffset
            );

            var depth = 1f - (i / (float)(layers.Length + 1)) * 0.01f;

            _spriteBatch.Draw(
                layer,
                position + offset,
                null,
                tint,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                depth
            );
        }
    }
}
