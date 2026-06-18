using GenGen.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GenGen.Core;

/// <summary>
/// Главный класс игры.
/// </summary>
public class GenGenGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch = null!;
	private SceneManager _sceneManager = null!;
	private SpriteStackRenderer _stackRenderer = null!;

	public GenGenGame()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth  = 1280,
			PreferredBackBufferHeight = 720,
		};
		Content.RootDirectory = "Content";
		Window.Title = "GenGen";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		_sceneManager = new SceneManager(this);
		_stackRenderer = new SpriteStackRenderer();
		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		_stackRenderer.Initialize(GraphicsDevice, _spriteBatch);
		_sceneManager.LoadScene(new TestScene(_stackRenderer));
	}

	protected override void Update(GameTime gameTime)
	{
		if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_sceneManager.Update(gameTime);
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(30, 30, 35));

		_spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);
		_sceneManager.Draw(_spriteBatch, gameTime);
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}
