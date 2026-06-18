using System.Runtime.InteropServices;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.Editor;

/// <summary>
/// Декларация вершины ImGui для MonoGame.
/// </summary>
internal static class ImGuiVertexDeclaration
{
	public static readonly VertexDeclaration Declaration = new(
		Marshal.SizeOf<ImDrawVert>(),
		new VertexElement(0,  VertexElementFormat.Vector2, VertexElementUsage.Position,          0),
		new VertexElement(8,  VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(16, VertexElementFormat.Color,   VertexElementUsage.Color,             0)
	);
}
