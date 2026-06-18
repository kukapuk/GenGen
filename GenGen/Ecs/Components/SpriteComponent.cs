using GenGen.ECS.Abstract;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.ECS.Components;

/// <summary>
/// Набор слоёв для sprite stacking рендеринга.
/// </summary>
public class SpriteStackComponent(Texture2D[] layers, float scale = 1f) : IComponent
{
	public Texture2D[] Layers { get; } = layers;
	public float Scale { get; set; } = scale;
}
