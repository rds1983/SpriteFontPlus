using System;
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
		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;
		private DynamicSpriteFont _font;
		private Texture2D _white;
		private bool _drawBackground = false;
		private bool _wasSpaceDown, _wasEnterDown;

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
			_font.AddTtf(File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
			_font.AddTtf(File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));

			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			GC.Collect();
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			var state = Keyboard.GetState();

			var isSpaceDown = state.IsKeyDown(Keys.Space);
			if (isSpaceDown && !_wasSpaceDown)
			{
				_drawBackground = !_drawBackground;
			}

			_wasSpaceDown = isSpaceDown;

			var isEnterDown = state.IsKeyDown(Keys.Enter);
			if (isEnterDown && !_wasEnterDown)
			{
				_font.UseKernings = !_font.UseKernings;
			}

			_wasEnterDown = isEnterDown;
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
			_font.Size = 18;
			DrawString("The quick brown fox jumps over the lazy dog", 0);

			_font.Size = 20;
			DrawString("Üben quält finſteren Jagdſchloß höfliche Bäcker größeren, N: Blåbærsyltetøy", 30);

			_font.Size = 22;
			DrawString("Høj bly gom vandt fræk sexquiz på wc, S: bäckasiner söka", 60);

			_font.Size = 24;
			DrawString("Sævör grét áðan því úlpan var ónýt, P: Pchnąć w tę łódź jeża lub osiem skrzyń fig", 90);

			_font.Size = 26;
			DrawString("Příliš žluťoučký kůň úpěl ďábelské kódy,\nR: В чащах юга жил-был цитрус?\nДа, но фальшивый экземпляр! ёъ.", 120);

			_font.Size = 28;
			DrawString("kilómetros y frío, añoraba,\nP: vôo à noite, F: Les naïfs ægithales\nhâtifs pondant à Noël où", 210);

			_font.Size = 30;
			DrawString("いろはにほへど", 310);

			_font.Size = 32;
			DrawString("🙌📦👏🔥👍😻😂🎉💻😍🚀😁🙈🇧🇪👩😉🍻🎶🏆👀👉👶💕😎😱🌌🌻🍺🏀👇👯💁💝💩😃😅🙏🚄🇫🌧🌾🍀🍁🍓🍕🎾🏈", 340, Color.Gold);

			_font.Size = 26;
			DrawString("Texture:", 380);
			
			
			var texture = _font.Textures.First();
			_spriteBatch.Draw(texture, new Vector2(0, 410), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}