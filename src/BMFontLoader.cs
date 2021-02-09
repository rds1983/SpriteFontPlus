using System;
using System.Collections.Generic;
using Cyotek.Drawing.BitmapFont;
using System.Linq;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using StbImageSharp;

namespace SpriteFontPlus
{
	public class TextureWithOffset
	{
		public Texture2D Texture { get; set; }
		public Point Offset { get; set; }

		public TextureWithOffset(Texture2D texture)
		{
			if (texture == null)
			{
				throw new ArgumentNullException("texture");
			}

			Texture = texture;
		}

		public TextureWithOffset(Texture2D texture, Point offset): this(texture)
		{
			Offset = offset;
		}
	}

	public static class BMFontLoader
	{
		private static SpriteFont Load(BitmapFont data, Func<string, TextureWithOffset> textureGetter)
		{
			if (data.Pages.Length > 1)
			{
				throw new NotSupportedException("For now only BMFonts with single texture are supported");
			}

			var texture = textureGetter(data.Pages[0].FileName);

			var glyphBounds = new List<Rectangle>();
			var cropping = new List<Rectangle>();
			var chars = new List<char>();
			var kerning = new List<Vector3>();

			var characters = data.Characters.Values.OrderBy(c => c.Char);
			foreach (var character in characters)
			{
				var bounds = new Rectangle(character.X, character.Y, character.Width, character.Height);

				bounds.Offset(texture.Offset);
				glyphBounds.Add(bounds);
				cropping.Add(new Rectangle(character.XOffset, character.YOffset, bounds.Width, bounds.Height));

				chars.Add(character.Char);

				kerning.Add(new Vector3(0, character.Width, character.XAdvance - character.Width));
			}

			var constructorInfo = typeof(SpriteFont).GetTypeInfo().DeclaredConstructors.First();
			var result = (SpriteFont)constructorInfo.Invoke(new object[]
			{
				texture.Texture, glyphBounds, cropping,
				chars, data.LineHeight, 0, kerning, ' '
			});

			return result;
		}

		private static BitmapFont LoadBMFont(string data)
		{
			var bmFont = new BitmapFont();
			if (data.StartsWith("<"))
			{
				// xml
				bmFont.LoadXml(data);
			}
			else
			{
				bmFont.LoadText(data);
			}

			return bmFont;
		}

		public static SpriteFont Load(string data, Func<string, TextureWithOffset> textureGetter)
		{
			var bmFont = LoadBMFont(data);

			return Load(bmFont, textureGetter);
		}

		public unsafe static SpriteFont Load(string data, Func<string, Stream> imageStreamOpener, GraphicsDevice device)
		{
			var bmFont = LoadBMFont(data);

			var textures = new Dictionary<string, Texture2D>();
			for (var i = 0; i < bmFont.Pages.Length; ++i)
			{
				var fileName = bmFont.Pages[i].FileName;
				Stream stream = null;
				try
				{
					stream = imageStreamOpener(fileName);
					if (!stream.CanSeek)
					{
						// If stream isn't seekable, use MemoryStream instead
						var ms = new MemoryStream();
						stream.CopyTo(ms);
						ms.Seek(0, SeekOrigin.Begin);
						stream.Dispose();
						stream = ms;
					}

					var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
					if (image.SourceComp == ColorComponents.Grey)
					{
						// If input image is single byte per pixel, then StbImageSharp will set alpha to 255 in the resulting 32-bit image
						// Such behavior isn't acceptable for us
						// So reset alpha to color value
						for (var j = 0; j < image.Data.Length / 4; ++j)
						{
							image.Data[j * 4 + 3] = image.Data[j * 4];
						}
					}

					var texture = new Texture2D(device, image.Width, image.Height);
					var bounds = new Rectangle(0, 0, image.Width, image.Height);
					texture.SetData(0, bounds, image.Data, 0, bounds.Width * bounds.Height * 4);

					textures[fileName] = texture;
				}
				finally
				{
					stream.Dispose();
				}
			}

			return Load(bmFont, fileName => new TextureWithOffset(textures[fileName]));
		}
	}
}