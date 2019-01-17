# SpriteFontPlus
[![NuGet](https://img.shields.io/nuget/v/SpriteFontPlus.MonoGame.svg)](https://www.nuget.org/packages/SpriteFontPlus.MonoGame/) [![Build status](https://ci.appveyor.com/api/projects/status/2mbacxymarcxq4we?svg=true)](https://ci.appveyor.com/project/RomanShapiro/spritefontplus)

Library that extends functionality of the SpriteFont.

# Usage
1. `Install-Package SpriteFontPlus.MonoGame`
2. Following code creates SpriteFont dynamically:
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

			_font = fontBakeResult.CreateSpriteFont(GraphicsDevice);
```
Now text could be drawn:
[](/images/sample.png)
