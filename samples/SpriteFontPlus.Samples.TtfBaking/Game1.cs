using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus.Ttf;

namespace SpriteFontPlus.Samples.TtfBaking
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private const int FontBitmapWidth = 1024;
		private const int FontBitmapHeight = 1024;

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		private SpriteFont _font;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1024,
				PreferredBackBufferHeight = 768
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
			var fontBakeResult = FontBaker.Bake(FontBitmapWidth, 
				FontBitmapHeight,
				25,
				new[]
				{
					new FontBakerEntry(File.ReadAllBytes("Fonts/DroidSans.ttf"),
						new[]
						{
							CharacterRange.BasicLatin,
							CharacterRange.Latin1Supplement,
							CharacterRange.LatinExtendedA,
							CharacterRange.Cyrillic,
						}),
					new FontBakerEntry(File.ReadAllBytes("Fonts/DroidSansJapanese.ttf"),
						new[]
						{
							CharacterRange.Hiragana,
							CharacterRange.Katakana
						})
				});

			_font = fontBakeResult.CreateSpriteFont(GraphicsDevice);

			GC.Collect();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			// Render some text
			_spriteBatch.DrawString(_font, "A: The quick brown fox jumps over the lazy dog",
				new Vector2(0, 0), Color.White);
/*			_spriteBatch.DrawString(_font, "B: Üben quält finſteren Jagdſchloß höfliche Bäcker größeren, N: Blåbærsyltetøy",
				new Vector2(0, 30), Color.White);
			_spriteBatch.DrawString(_font, "C: Høj bly gom vandt fræk sexquiz på wc, S: bäckasiner söka",
				new Vector2(0, 60), Color.White);
			_spriteBatch.DrawString(_font, "D: Sævör grét áðan því úlpan var ónýt, P: Pchnąć w tę łódź jeża lub osiem skrzyń fig",
				new Vector2(0, 90), Color.White);
			_spriteBatch.DrawString(_font, "E: Příliš žluťoučký kůň úpěl ďábelské kódy, R: В чащах юга жил-был цитрус? Да, но фальшивый экземпляр! ёъ.",
				new Vector2(0, 120), Color.White);
			_spriteBatch.DrawString(_font, "F: kilómetros y frío, añoraba, P: vôo à noite, F: Les naïfs ægithales hâtifs pondant à Noël où",
				new Vector2(0, 150), Color.White);
			_spriteBatch.DrawString(_font, "G: いろはにほへど",
				new Vector2(0, 180), Color.White);*/

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}