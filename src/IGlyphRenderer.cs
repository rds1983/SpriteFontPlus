using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpriteFontPlus
{
    public interface IGlyphRenderer
    {
        GraphicsDevice GraphicsDevice { get; }
        
        void Draw(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Color color, float rotation, Vector2 origin,
            SpriteEffects effect, float depth);
    }

    public class SpriteBatchGlyphRenderer : IGlyphRenderer
    {
        private SpriteBatch _batch;

        public SpriteBatchGlyphRenderer(SpriteBatch batch)
        {
            _batch = batch ?? throw new ArgumentNullException(nameof(batch));
        }

        public GraphicsDevice GraphicsDevice => _batch.GraphicsDevice;

        public void Draw(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Color color, float rotation,
            Vector2 origin,
            SpriteEffects effect, float depth)
        {
            _batch.Draw(texture, destRect, sourceRect, color, rotation, origin, effect, depth);
        }
    }
}