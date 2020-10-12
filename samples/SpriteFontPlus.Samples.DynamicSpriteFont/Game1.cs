using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private const int EffectAmount = 2;

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;
		private DynamicSpriteFont _font;
		private DynamicSpriteFont[] _fonts;

		private Texture2D _white;
		private bool _drawBackground = false;

		private static readonly Color[] _colors = new Color[]
		{
			Color.Red,
			Color.Blue,
			Color.Green,
			Color.Aquamarine,
			Color.Azure,
			Color.Chartreuse,
			Color.Lavender,
			Color.OldLace,
			Color.PaleGreen,
			Color.SaddleBrown,
			Color.IndianRed,
			Color.ForestGreen,
			Color.Khaki
		};

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

		private DynamicSpriteFont LoadFont(int blurAmount, int strokeAmount)
		{
			var result = DynamicSpriteFont.FromTtf(GraphicsDevice, File.ReadAllBytes(@"Fonts/DroidSans.ttf"), 20, 1024, 1024, blurAmount, strokeAmount);
			result.AddTtf(File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
			result.AddTtf(File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));

			return result;
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
			var fonts = new List<DynamicSpriteFont>();

			// Simple font
			fonts.Add(LoadFont(0, 0));

			// Blurry font
			fonts.Add(LoadFont(EffectAmount, 0));

			// Stroked font
			fonts.Add(LoadFont(0, EffectAmount));

			_fonts = fonts.ToArray();
			_font = _fonts[0];


			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			GC.Collect();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			KeyboardUtils.Begin();

			if (KeyboardUtils.IsPressed(Keys.Space))
			{
				_drawBackground = !_drawBackground;
			}

			if (KeyboardUtils.IsPressed(Keys.Tab))
			{
				var i = 0;

				for(; i < _fonts.Length; ++i)
				{
					if (_font == _fonts[i])
					{
						break;
					}
				}

				++i;
				if (i >= _fonts.Length)
				{
					i = 0;
				}

				_font = _fonts[i];
			}

			if (KeyboardUtils.IsPressed(Keys.Enter))
			{
				_font.UseKernings = !_font.UseKernings;

			}

			KeyboardUtils.End();
		}

		private void DrawString(string text, int y, Color[] glyphColors)
		{
			if (_drawBackground)
			{
				var size = _font.MeasureString(text);
				_spriteBatch.Draw(_white, new Rectangle(0, y, (int)size.X, (int)size.Y), Color.Green);
			}

			_spriteBatch.DrawString(_font, text, new Vector2(0, y), glyphColors);
		}

		private void DrawString(string text, int y, Color color)
		{
			if (_drawBackground)
			{
				var size = _font.MeasureString(text);
				_spriteBatch.Draw(_white, new Rectangle(0, y, (int)size.X, (int)size.Y), Color.Green);
			}

			_spriteBatch.DrawString(_font, text, new Vector2(0, y), color);
		}

		private void DrawString(string text, int y)
		{
			DrawString(text, y, Color.White);
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

			// Render some text
			_font.FontSize = 18;
			DrawString("The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog adfasoqiw yraldh ald halwdha ldjahw dlawe havbx get872rq", 0);

			_font.FontSize = 30;
			DrawString("The quick いろは brown\nfox にほへ jumps over\nt🙌h📦e l👏a👏zy dog", 80, Color.Bisque);

			DrawString("Colored Text", 200, _colors);

			_font.FontSize = 26;
			DrawString("Texture:", 380);
			
			var texture = _font.Textures.First();
			_spriteBatch.Draw(texture, new Vector2(0, 410), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}