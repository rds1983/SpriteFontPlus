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
		private int _fontIdJapanese, _fontIdEmojis;

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
			_font = DynamicSpriteFont.FromTtf(File.ReadAllBytes(@"Fonts/DroidSans.ttf"), 20);
			_fontIdJapanese = _font.AddTtf("Japanese", File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
			_fontIdEmojis = _font.AddTtf("Emojis", File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));

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
			_spriteBatch.Begin();

			_font.FontId = _font.DefaultFontId;
			// Render some text
			_font.Size = 18;
			_spriteBatch.DrawString(_font, "The quick brown fox jumps over the lazy dog",
				new Vector2(0, 0), Color.White);

			_font.Size = 20;
			_spriteBatch.DrawString(_font, "Üben quält finſteren Jagdſchloß höfliche Bäcker größeren, N: Blåbærsyltetøy",
				new Vector2(0, 30), Color.White);

			_font.Size = 22;
			_spriteBatch.DrawString(_font, "Høj bly gom vandt fræk sexquiz på wc, S: bäckasiner söka",
				new Vector2(0, 60), Color.White);

			_font.Size = 24;
			_spriteBatch.DrawString(_font, "Sævör grét áðan því úlpan var ónýt, P: Pchnąć w tę łódź jeża lub osiem skrzyń fig",
				new Vector2(0, 90), Color.White);

			_font.Size = 26;
			_spriteBatch.DrawString(_font, "Příliš žluťoučký kůň úpěl ďábelské kódy, R: В чащах юга жил-был цитрус? Да, но фальшивый экземпляр! ёъ.",
				new Vector2(0, 120), Color.White);

			_font.Size = 28;
			_spriteBatch.DrawString(_font, "kilómetros y frío, añoraba, P: vôo à noite, F: Les naïfs ægithales hâtifs pondant à Noël où",
				new Vector2(0, 150), Color.White);

			_font.FontId = _fontIdJapanese;
			_font.Size = 30;
			_spriteBatch.DrawString(_font, "いろはにほへど", new Vector2(0, 180), Color.White);

			_font.FontId = _fontIdEmojis;
			_font.Size = 32;
			_spriteBatch.DrawString(_font, "🙌📦👏🔥👍😻😂🎉💻😍🚀😁🙈🇧🇪👩😉🍻🎶🏆👀👉👶💕😎😱🌌🌻🍺🏀👇👯💁💝💩😃😅🙏🚄🇫🌧🌾🍀🍁🍓🍕🎾🏈",
				new Vector2(0, 220), Color.Gold);

			_font.FontId = _font.DefaultFontId;
			_font.Size = 26;
			_spriteBatch.DrawString(_font, "Texture:",
				new Vector2(0, 300), Color.White);

			_spriteBatch.Draw(_font.Texture, new Vector2(0, 330), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}