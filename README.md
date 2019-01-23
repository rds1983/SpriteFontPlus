# SpriteFontPlus
[![NuGet](https://img.shields.io/nuget/v/SpriteFontPlus.MonoGame.svg)](https://www.nuget.org/packages/SpriteFontPlus.MonoGame/) [![Build status](https://ci.appveyor.com/api/projects/status/2mbacxymarcxq4we?svg=true)](https://ci.appveyor.com/project/RomanShapiro/spritefontplus)

Library that extends functionality of the SpriteFont. For now it has only one feature - ability to create SpiteFont dynamically from ttf.

# Adding Reference
1. `Install-Package SpriteFontPlus.MonoGame` (or `Install-Package SpriteFontPlus.FNA` for FNA)

# Loading SpriteFont from a ttf
2. Following code creates a SpriteFont from a ttf:
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

# Using DynamicSpriteFont
DynamicSpriteFont allows to render fonts on demand to a texture atlas.

Following code creates it from a ttf:
```c#
DynamicSpriteFont font = DynamicSpriteFont.FromTtf(File.ReadAllBytes(@"C:\\Windows\\Fonts\msyh.ttf"));
```

And following code renders some text with different sizes:
```c#
_spriteBatch.DrawString(_font, 10, "The quick brown fox jumps over the lazy dog", 
	new Vector2(0, 0), Color.White);
_spriteBatch.DrawString(_font, 14, "Üben quält finſteren Jagdſchloß höfliche Bäcker größeren, N: Blåbærsyltetøy",
	new Vector2(0, 30), Color.White);
_spriteBatch.DrawString(_font, 18, "Høj bly gom vandt fræk sexquiz på wc, S: bäckasiner söka",
	new Vector2(0, 60), Color.White);
_spriteBatch.DrawString(_font, 22, "Sævör grét áðan því úlpan var ónýt, P: Pchnąć w tę łódź jeża lub osiem skrzyń fig",
	new Vector2(0, 90), Color.White);
_spriteBatch.DrawString(_font, 26, "Příliš žluťoučký kůň úpěl ďábelské kódy, R: В чащах юга жил-был цитрус? Да, но фальшивый экземпляр! ёъ.",
	new Vector2(0, 120), Color.White);
_spriteBatch.DrawString(_font, 30, "kilómetros y frío, añoraba, P: vôo à noite, F: Les naïfs ægithales hâtifs pondant à Noël où",
	new Vector2(0, 150), Color.White);
_spriteBatch.DrawString(_font, 34, "いろはにほへど",
	new Vector2(0, 180), Color.White);
```
It would render following:
![](/images/sampleDynamicSpriteFont.png)

## Credits
* [MonoGame](http://www.monogame.net/)
* [Cyotek.Drawing.BitmapFont](https://github.com/cyotek/Cyotek.Drawing.BitmapFont)
* [stb](https://github.com/nothings/stb)
* [fontstash] (https://github.com/memononen/fontstash)
