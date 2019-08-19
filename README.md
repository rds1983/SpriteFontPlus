# SpriteFontPlus
[![NuGet](https://img.shields.io/nuget/v/SpriteFontPlus.MonoGame.svg)](https://www.nuget.org/packages/SpriteFontPlus.MonoGame/) [![Build status](https://ci.appveyor.com/api/projects/status/2mbacxymarcxq4we?svg=true)](https://ci.appveyor.com/project/RomanShapiro/spritefontplus)

Library that extends functionality of the SpriteFont.

# Features
* Creation of SpriteFont in the run-time from ttf.
* Creation of SpriteFont in the run-time from AngelCode BMFont(only XML with single texture works for now).
* DynamicSpriteFont class that renders glyphs on demand to the underlying texture atlas. Also it supports 32-bit characters and blurry text.

# Adding Reference
There are two ways of referencing SpriteFontPlus in the project:
1. Through nuget: `install-package SpriteFontPlus.MonoGame` for MonoGame(or `install-package SpriteFontPlus.FNA` for FNA)
2. As submodule:
    
    a. `git submodule add https://github.com/rds1983/SpriteFontPlus.git`
    
    b. `git submodule update --init --recursive`
    
    c. Copy SolutionDefines.targets from SpriteFontPlus/build/MonoGame(or SpriteFontPlus/build/FNA) to your solution folder.

      * If FNA is used, SolutionDefines.targets needs to be edited and FNAProj variable should be updated to the location of FNA.csproj next to the SpriteFontPlus.csproj location.
    
    c. Add SpriteFontPlus/src/SpriteFontPlus/SpriteFontPlus.csproj to the solution.
    
# Loading SpriteFont from a ttf
Following code creates a SpriteFont from a ttf:
```c#
var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(@"C:\\Windows\\Fonts\arial.ttf"),
	25,
	1024,
	1024,
	new[]
	{
		CharacterRange.BasicLatin,
		CharacterRange.Latin1Supplement,
		CharacterRange.LatinExtendedA,
		CharacterRange.Cyrillic
	}
);

SpriteFont font = fontBakeResult.CreateSpriteFont(GraphicsDevice);
```
Full sample is here:
[samples/SpriteFontPlus.Samples.TtfBaking](samples/SpriteFontPlus.Samples.TtfBaking)


# Loading SpriteFont from AngelCode BMFont
```c#
Texture2D texture;
using (var stream = TitleContainer.OpenStream("Fonts/test_0.png"))
{
	texture = Texture2D.FromStream(GraphicsDevice, stream);
}

string fontData;
using (var stream = TitleContainer.OpenStream("Fonts/test.fnt"))
{
	using (var reader = new StreamReader(stream))
	{
		fontData = reader.ReadToEnd();
	}
}

// As we use font with one texture, always return it independently from requested name
SpriteFont font = BMFontLoader.LoadXml(fontData, name => texture);
```

Full sample is here:
[samples/SpriteFontPlus.Samples.BMFont](samples/SpriteFontPlus.Samples.BMFont)


# DynamicSpriteFont
DynamicSpriteFont renders glyphs on demand to the underlying texture atlas. Thus it doesnt require to explicity specify character ranges that are going to be used during the font creation.

Following code creates DynamicSpriteFont from 3 different ttfs:
```c#
	_font = DynamicSpriteFont.FromTtf(File.ReadAllBytes(@"Fonts/DroidSans.ttf"), 20);
	_fontIdJapanese = _font.AddTtf("Japanese", File.ReadAllBytes(@"Fonts/DroidSansJapanese.ttf"));
	_fontIdEmojis = _font.AddTtf("Emojis", File.ReadAllBytes(@"Fonts/Symbola-Emoji.ttf"));
```			

And following code renders text with it along with the backing texture:
```c#
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
```

It would render following:
![](/images/sampleDynamicSpriteFont.png)

Full sample is here:
[samples/SpriteFontPlus.Samples.DynamicSpriteFont](samples/SpriteFontPlus.Samples.DynamicSpriteFont)

## Credits
* [MonoGame](http://www.monogame.net/)
* [FNA](https://github.com/FNA-XNA/FNA)
* [BMFont](https://www.angelcode.com/products/bmfont/)
* [Cyotek.Drawing.BitmapFont](https://github.com/cyotek/Cyotek.Drawing.BitmapFont)
* [stb](https://github.com/nothings/stb)
* [fontstash](https://github.com/memononen/fontstash)
