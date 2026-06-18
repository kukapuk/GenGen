using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GenGen.Core.Abstract;

public interface IScene : IDisposable
{
	void Initialize(GenGenGame game);
	void Update(GameTime gameTime);
	void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}
