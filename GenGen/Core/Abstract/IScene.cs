using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.Core.Abstract;

public interface IScene : IDisposable
{
	void Initialize(GenGenGame game);
	void Update(GameTime gameTime);
	void DrawMap(SpriteBatch spriteBatch, GameTime gameTime);
	void DrawSprites(SpriteBatch spriteBatch, GameTime gameTime);
}
