using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace SpriteFontPlus
{
	public static class SpriteBatchExtensions
	{
		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string text, Vector2 pos, Color color, float depth = 0.0f)
		{
			return font.DrawString(batch, text, pos, color, depth);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, string text, Vector2 pos, Color[] glyphColors, float depth = 0.0f)
		{
			return font.DrawString(batch, text, pos, glyphColors, depth);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder text, Vector2 pos, Color color, float depth = 0.0f)
		{
			return font.DrawString(batch, text, pos, color, depth);
		}

		public static float DrawString(this SpriteBatch batch, DynamicSpriteFont font, StringBuilder text, Vector2 pos, Color[] glyphColors, float depth = 0.0f)
		{
			return font.DrawString(batch, text, pos, glyphColors, depth);
		}
	}
}