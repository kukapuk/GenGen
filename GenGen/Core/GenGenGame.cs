using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GenGen.Core;

public class GenGenGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch = null!;

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
		// инициализация систем движка
		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);
		// загрузка текстур, карт и т.д.
	}

	protected override void Update(GameTime gameTime)
	{
		if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		// обновление логики
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(30, 30, 35));

		// отрисовка
		base.Draw(gameTime);
	}
}
