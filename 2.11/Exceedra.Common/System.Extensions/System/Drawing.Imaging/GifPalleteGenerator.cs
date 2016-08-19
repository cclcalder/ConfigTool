using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Drawing.Imaging
{
	public static class GifPalleteGenerator
	{
		private const int STANDARD_COLOR_PALLETTE_SIZE = 256;

		private readonly static Color TRANSPARENT;

		static GifPalleteGenerator()
		{
			GifPalleteGenerator.TRANSPARENT = Color.FromArgb(0, 0, 0, 0);
		}

		private static ColorPalette CreateEmptyPallette()
		{
			ColorPalette palette;
			Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
			try
			{
				palette = bitmap.Palette;
			}
			finally
			{
				if (bitmap != null)
				{
					((IDisposable)bitmap).Dispose();
				}
			}
			return palette;
		}

		private static List<KeyValuePair<Color, int>> FindAllColours(Bitmap image)
		{
			Dictionary<Color, int> colors = new Dictionary<Color, int>();
			for (int i = 0; i < image.Width; i++)
			{
				for (int j = 0; j < image.Height; j++)
				{
					Color pixel = image.GetPixel(i, j);
					if (!colors.ContainsKey(pixel))
					{
						colors.Add(pixel, 1);
					}
					else
					{
						Dictionary<Color, int> colors1 = colors;
						Dictionary<Color, int> colors2 = colors1;
						Color color = pixel;
						colors1[color] = colors2[color] + 1;
					}
				}
			}
			IEnumerable<KeyValuePair<Color, int>> value = 
				from item in colors
				orderby item.Value descending
				select new KeyValuePair<Color, int>(item.Key, item.Value);
			return value.ToList<KeyValuePair<Color, int>>();
		}

		public static ColorPalette GeneratePallete(Bitmap image)
		{
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			ColorPalette tRANSPARENT = GifPalleteGenerator.CreateEmptyPallette();
			List<KeyValuePair<Color, int>> keyValuePairs = GifPalleteGenerator.FindAllColours(image);
			tRANSPARENT.Entries[0] = GifPalleteGenerator.TRANSPARENT;
			for (int i = 1; i < 256; i++)
			{
				if (keyValuePairs.Count < i)
				{
					tRANSPARENT.Entries[i] = Color.White;
				}
				else
				{
					KeyValuePair<Color, int> item = keyValuePairs[i - 1];
					tRANSPARENT.Entries[i] = item.Key;
				}
			}
			return tRANSPARENT;
		}
	}
}