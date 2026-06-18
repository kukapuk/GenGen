using GenGen.Core.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.Core;

/// <summary>
/// Управляет сценами. Поддерживает стек — например, пауза поверх игры.
/// </summary>
public class SceneManager(GenGenGame game)
{
	private readonly Stack<IScene> _sceneStack = new();

	public IScene? CurrentScene => _sceneStack.Count > 0 ? _sceneStack.Peek() : null;

	public void LoadScene(IScene scene)
	{
		while (_sceneStack.Count > 0)
			PopScene();

		PushScene(scene);
	}

	public void PushScene(IScene scene)
	{
		scene.Initialize(game);
		_sceneStack.Push(scene);
	}

	public void PopScene()
	{
		if (_sceneStack.TryPop(out var scene))
			scene.Dispose();
	}

	public void Update(GameTime gameTime)          => CurrentScene?.Update(gameTime);
	public void DrawMap(SpriteBatch spriteBatch, GameTime gameTime)     => CurrentScene?.DrawMap(spriteBatch, gameTime);
	public void DrawSprites(SpriteBatch spriteBatch, GameTime gameTime) => CurrentScene?.DrawSprites(spriteBatch, gameTime);
	public void DrawEditor(GameTime gameTime)      => CurrentScene?.DrawEditor(gameTime);
}
