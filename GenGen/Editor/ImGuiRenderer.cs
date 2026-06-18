using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using Vector2 = System.Numerics.Vector2;

namespace GenGen.Editor;

/// <summary>
/// Интегрирует Dear ImGui в MonoGame DesktopGL.
/// </summary>
public class ImGuiRenderer
{
	private GraphicsDevice _graphicsDevice = null!;
	private BasicEffect _effect = null!;
	private RasterizerState _rasterizerState = null!;

	private byte[] _vertexData = [];
	private byte[] _indexData  = [];
	private VertexBuffer? _vertexBuffer;
	private IndexBuffer?  _indexBuffer;
	private int _vertexBufferSize;
	private int _indexBufferSize;

	private readonly Dictionary<IntPtr, Texture2D> _textures = new();
	private int _textureId;
	private Texture2D? _fontTexture;

	private int _scrollWheelValue;
	private readonly float _uiScale = 1f;

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		_graphicsDevice = graphicsDevice;

		ImGui.CreateContext();

		var io = ImGui.GetIO();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
		io.ConfigFlags  |= ImGuiConfigFlags.DockingEnable;
		io.DisplaySize   = new Vector2(
			graphicsDevice.Viewport.Width,
			graphicsDevice.Viewport.Height
		);

		_effect = new BasicEffect(graphicsDevice)
		{
			World           = Matrix.Identity,
			View            = Matrix.Identity,
			VertexColorEnabled = true,
			TextureEnabled  = true,
		};

		_rasterizerState = new RasterizerState
		{
			CullMode  = CullMode.None,
			DepthClipEnable = false,
			ScissorTestEnable = true,
		};

		BuildFontAtlas();
	}
	
	/// <summary>
	/// Регистрирует текстуру для использования в ImGui. Возвращает указатель-хэндл.
	/// </summary>
	public IntPtr BindTexture(Texture2D texture)
	{
		var id = new IntPtr(_textureId++);
		_textures[id] = texture;
		return id;
	}

	/// <summary>
	/// Удаляет зарегистрированную текстуру.
	/// </summary>
	public void UnbindTexture(IntPtr id) => _textures.Remove(id);
	
	/// <summary>
	/// Вызывать в начале Update, до любого ImGui кода.
	/// </summary>
	public void BeforeLayout(GameTime gameTime)
	{
		var io = ImGui.GetIO();

		io.DeltaTime   = (float)gameTime.ElapsedGameTime.TotalSeconds;
		io.DisplaySize = new Vector2(
			_graphicsDevice.Viewport.Width,
			_graphicsDevice.Viewport.Height
		);

		UpdateInput(io);

		ImGui.NewFrame();
	}

	/// <summary>
	/// Вызывать в конце Draw, после всего остального рендера.
	/// </summary>
	public void AfterLayout()
	{
		ImGui.Render();
		RenderDrawData(ImGui.GetDrawData());
	}
	
	private void UpdateInput(ImGuiIOPtr io)
	{
		var mouse    = Mouse.GetState();
		var keyboard = Keyboard.GetState();

		io.MousePos    = new Vector2(mouse.X, mouse.Y);
		io.MouseDown[0] = mouse.LeftButton   == ButtonState.Pressed;
		io.MouseDown[1] = mouse.RightButton  == ButtonState.Pressed;
		io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

		var scrollDelta = mouse.ScrollWheelValue - _scrollWheelValue;
		io.MouseWheel       = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
		_scrollWheelValue   = mouse.ScrollWheelValue;

		io.KeyCtrl  = keyboard.IsKeyDown(Keys.LeftControl)  || keyboard.IsKeyDown(Keys.RightControl);
		io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift)    || keyboard.IsKeyDown(Keys.RightShift);
		io.KeyAlt   = keyboard.IsKeyDown(Keys.LeftAlt)      || keyboard.IsKeyDown(Keys.RightAlt);
		io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows)  || keyboard.IsKeyDown(Keys.RightWindows);

		io.SetKeyEventNativeData(ImGuiKey.Tab,        (int)Keys.Tab,        (int)Keys.Tab);
		io.SetKeyEventNativeData(ImGuiKey.LeftArrow,  (int)Keys.Left,       (int)Keys.Left);
		io.SetKeyEventNativeData(ImGuiKey.RightArrow, (int)Keys.Right,      (int)Keys.Right);
		io.SetKeyEventNativeData(ImGuiKey.UpArrow,    (int)Keys.Up,         (int)Keys.Up);
		io.SetKeyEventNativeData(ImGuiKey.DownArrow,  (int)Keys.Down,       (int)Keys.Down);
		io.SetKeyEventNativeData(ImGuiKey.Delete,     (int)Keys.Delete,     (int)Keys.Delete);
		io.SetKeyEventNativeData(ImGuiKey.Backspace,  (int)Keys.Back,       (int)Keys.Back);
		io.SetKeyEventNativeData(ImGuiKey.Enter,      (int)Keys.Enter,      (int)Keys.Enter);
		io.SetKeyEventNativeData(ImGuiKey.Escape,     (int)Keys.Escape,     (int)Keys.Escape);
		io.SetKeyEventNativeData(ImGuiKey.A,          (int)Keys.A,          (int)Keys.A);
		io.SetKeyEventNativeData(ImGuiKey.C,          (int)Keys.C,          (int)Keys.C);
		io.SetKeyEventNativeData(ImGuiKey.V,          (int)Keys.V,          (int)Keys.V);
		io.SetKeyEventNativeData(ImGuiKey.X,          (int)Keys.X,          (int)Keys.X);
		io.SetKeyEventNativeData(ImGuiKey.Y,          (int)Keys.Y,          (int)Keys.Y);
		io.SetKeyEventNativeData(ImGuiKey.Z,          (int)Keys.Z,          (int)Keys.Z);
	}

	private void BuildFontAtlas()
	{
		var io = ImGui.GetIO();
		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixelData, out var width, out var height, out _);

		var pixels = new byte[width * height * 4];
		Marshal.Copy(pixelData, pixels, 0, pixels.Length);

		_fontTexture = new Texture2D(_graphicsDevice, width, height, false, SurfaceFormat.Color);
		_fontTexture.SetData(pixels);

		io.Fonts.SetTexID(BindTexture(_fontTexture));
		io.Fonts.ClearTexData();
	}

	private void RenderDrawData(ImDrawDataPtr drawData)
	{
		if (drawData.CmdListsCount == 0)
			return;

		var viewport = _graphicsDevice.Viewport;

		UpdateBuffers(drawData);

		_graphicsDevice.SetVertexBuffer(_vertexBuffer);
		_graphicsDevice.Indices = _indexBuffer;

		_effect.Projection = Matrix.CreateOrthographicOffCenter(
			0, viewport.Width,
			viewport.Height, 0,
			-1f, 1f
		);

		var prevScissor    = _graphicsDevice.ScissorRectangle;
		var prevBlend      = _graphicsDevice.BlendState;
		var prevDepth      = _graphicsDevice.DepthStencilState;
		var prevRasterizer = _graphicsDevice.RasterizerState;

		_graphicsDevice.BlendState        = BlendState.NonPremultiplied;
		_graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
		_graphicsDevice.RasterizerState   = _rasterizerState;

		var vertexOffset = 0;
		var indexOffset  = 0;

		for (var n = 0; n < drawData.CmdListsCount; n++)
		{
			var cmdList = drawData.CmdLists[n];

			for (var i = 0; i < cmdList.CmdBuffer.Size; i++)
			{
				var cmd = cmdList.CmdBuffer[i];

				if (cmd.UserCallback != IntPtr.Zero)
					continue;

				if (!_textures.TryGetValue(cmd.TextureId, out var texture))
					throw new InvalidOperationException($"ImGui texture not found: {cmd.TextureId}");

				_graphicsDevice.ScissorRectangle = new Rectangle(
					(int)cmd.ClipRect.X,
					(int)cmd.ClipRect.Y,
					(int)(cmd.ClipRect.Z - cmd.ClipRect.X),
					(int)(cmd.ClipRect.W - cmd.ClipRect.Y)
				);

				_effect.Texture = texture;

				foreach (var pass in _effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					_graphicsDevice.DrawIndexedPrimitives(
						PrimitiveType.TriangleList,
						vertexOffset + (int)cmd.VtxOffset,
						indexOffset  + (int)cmd.IdxOffset,
						(int)cmd.ElemCount / 3
					);
				}
			}

			vertexOffset += cmdList.VtxBuffer.Size;
			indexOffset  += cmdList.IdxBuffer.Size;
		}

		_graphicsDevice.BlendState        = prevBlend;
		_graphicsDevice.DepthStencilState = prevDepth;
		_graphicsDevice.RasterizerState   = prevRasterizer;
		_graphicsDevice.ScissorRectangle  = prevScissor;
	}

	private void UpdateBuffers(ImDrawDataPtr drawData)
	{
		var totalVtx = drawData.TotalVtxCount;
		var totalIdx = drawData.TotalIdxCount;

		if (totalVtx == 0 || totalIdx == 0)
			return;

		var vtxSize = totalVtx * Marshal.SizeOf<ImDrawVert>();
		var idxSize = totalIdx * sizeof(ushort);

		if (vtxSize > _vertexData.Length) _vertexData = new byte[vtxSize];
		if (idxSize > _indexData.Length)  _indexData  = new byte[idxSize];

		var vtxOffset = 0;
		var idxOffset = 0;

		for (var n = 0; n < drawData.CmdListsCount; n++)
		{
			var cmdList = drawData.CmdLists[n];

			var vtxBytes = cmdList.VtxBuffer.Size * Marshal.SizeOf<ImDrawVert>();
			var idxBytes = cmdList.IdxBuffer.Size * sizeof(ushort);

			unsafe
			{
				Marshal.Copy((IntPtr)cmdList.VtxBuffer.Data, _vertexData, vtxOffset, vtxBytes);
				Marshal.Copy((IntPtr)cmdList.IdxBuffer.Data, _indexData,  idxOffset, idxBytes);
			}

			vtxOffset += vtxBytes;
			idxOffset += idxBytes;
		}

		if (_vertexBufferSize < totalVtx)
		{
			_vertexBuffer?.Dispose();
			_vertexBuffer     = new VertexBuffer(_graphicsDevice, ImGuiVertexDeclaration.Declaration, totalVtx, BufferUsage.WriteOnly);
			_vertexBufferSize = totalVtx;
		}

		if (_indexBufferSize < totalIdx)
		{
			_indexBuffer?.Dispose();
			_indexBuffer     = new IndexBuffer(_graphicsDevice, IndexElementSize.SixteenBits, totalIdx, BufferUsage.WriteOnly);
			_indexBufferSize = totalIdx;
		}

		_vertexBuffer!.SetData(_vertexData, 0, vtxOffset);
		_indexBuffer!.SetData(_indexData,   0, idxOffset);
	}
}
