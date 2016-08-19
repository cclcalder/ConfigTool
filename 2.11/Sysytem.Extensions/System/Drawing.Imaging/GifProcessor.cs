using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Drawing.Imaging
{
	internal class GifProcessor
	{
		private static object syncLock;

		private Bitmap Source;

		private ColorPalette CorrectPallete;

		static GifProcessor()
		{
			GifProcessor.syncLock = new object();
		}

		public GifProcessor(Bitmap sourceImage)
		{
			if (sourceImage == null)
			{
				throw new ArgumentNullException("sourceImage");
			}
			this.Source = sourceImage;
			this.CorrectPallete = GifPalleteGenerator.GeneratePallete(sourceImage);
		}

        //private unsafe Bitmap CreateBitmapWithIndexedColors(Bitmap source)
        //{
        //    int width = source.Width;
        //    int height = source.Height;
        //    Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed)
        //    {
        //        Palette = this.CorrectPallete
        //    };
        //    BitmapData bitmapDatum = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
        //    byte* pointer = (byte*)bitmapDatum.Scan0.ToPointer();
        //    if (bitmapDatum.Stride <= 0)
        //    {
        //        pointer = pointer + bitmapDatum.Stride * (height - 1);
        //    }
        //    uint num = (uint)Math.Abs(bitmapDatum.Stride);
        //    for (uint i = 0; (ulong)i < (long)width; i++)
        //    {
        //        for (uint j = 0; (ulong)j < (long)height; j++)
        //        {
        //            Color pixel = source.GetPixel((int)i, (int)j);
        //            byte* numPointer = i + pointer + j * num;
        //            *numPointer = this.FindPalleteEntryIndex(pixel);
        //        }
        //    }
        //    bitmap.UnlockBits(bitmapDatum);
        //    return bitmap;
        //}

		private static Bitmap CreateCopy(Bitmap source)
		{
			Bitmap bitmap = source.Clone(new Rectangle(0, 0, source.Width, source.Height), PixelFormat.Format32bppArgb);
			return bitmap;
		}

		private byte FindPalleteEntryIndex(Color color)
		{
			byte index;
			if (color.A != 0)
			{
				byte num = 0;
				while (num < 255)
				{
					if (!(this.CorrectPallete.Entries[num] == color))
					{
						num = (byte)(num + 1);
					}
					else
					{
						index = num;
						return index;
					}
				}
				index = ((IEnumerable<Color>)this.CorrectPallete.Entries).Select((Color c, int i) => new { Index = (byte)i, Color = c }).WithMin((entry) => GifProcessor.GetDifference(color, entry.Color)).Index;
			}
			else
			{
				index = 0;
			}
			return index;
		}

		private static int GetDifference(Color c1, Color c2)
		{
			int num = Math.Abs((int)(c1.A - c2.A)) + Math.Abs((int)(c1.R - c2.R)) + Math.Abs((int)(c1.G - c2.G)) + Math.Abs((int)(c1.B - c2.B));
			return num;
		}

        //public void Save(string filename, bool transparent)
        //{
        //    int width = this.Source.Width;
        //    int height = this.Source.Height;
        //    lock (GifProcessor.syncLock)
        //    {
        //        Bitmap bitmap = GifProcessor.CreateCopy(this.Source);
        //        try
        //        {
        //            this.CreateBitmapWithIndexedColors(bitmap).Save(filename, ImageFormat.Gif);
        //        }
        //        finally
        //        {
        //            if (bitmap != null)
        //            {
        //                ((IDisposable)bitmap).Dispose();
        //            }
        //        }
        //    }
        //}
	}
}