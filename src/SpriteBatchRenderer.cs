using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpriteFontPlus
{
	class SpriteBatchRenderer : IFontStashRenderer
	{
		public static readonly SpriteBatchRenderer Instance = new SpriteBatchRenderer();

		private SpriteBatch _batch;

		public SpriteBatch Batch
		{
			get
			{
				return _batch;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_batch = value;
			}
		}

		private SpriteBatchRenderer()
		{
		}

		public void Draw(ITexture2D texture, Rectangle dest, Rectangle source, Color color, float depth)
		{
			var textureWrapper = (Texture2DWrapper)texture;

			_batch.Draw(textureWrapper.Texture,
				dest,
				source,
				color,
				0f,
				Vector2.Zero,
				SpriteEffects.None,
				depth);
		}
	}
}
