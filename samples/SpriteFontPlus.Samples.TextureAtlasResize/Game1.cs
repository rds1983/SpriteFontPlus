using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpriteFontPlus.Samples.TtfBaking
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;
		private DynamicSpriteFont _font;
		private Texture2D _white;
		private readonly Random _random = new Random();

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			_font = DynamicSpriteFont.FromTtf(File.ReadAllBytes(@"Fonts/DroidSans.ttf"), 20, 128, 128);

			_font.AtlasFull += (s, a) =>
			{
				if (_font.Texture.Width < 512)
				{
					// Expand atlas size 2x
					_font.ExpandAtlas(_font.Texture.Width * 2, _font.Texture.Height * 2);
				}
				else
				{
					// Reset back to 128x128
					_font.ResetAtlas(128, 128);
				}
			};

			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			GC.Collect();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_spriteBatch.Begin();

			var c = (char)_random.Next(32, 100);

			_font.Size = _random.Next(20, 40);
			_font.Blur = _random.Next(0, 4);
			_spriteBatch.DrawString(_font, c.ToString(), Vector2.Zero, Color.White);

			_spriteBatch.Draw(_white, new Rectangle(0, 50, _font.Texture.Width, _font.Texture.Height), Color.Green);
			_spriteBatch.Draw(_font.Texture, new Vector2(0, 50), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}