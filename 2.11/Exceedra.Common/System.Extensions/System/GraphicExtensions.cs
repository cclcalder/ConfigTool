using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.CompilerServices;

namespace System
{
	public static class GraphicExtensions
	{
		private const double HALF_PI = 1.5707963267949;

		private const double RADIANS = 0.0174532925199433;

		public static Bitmap GetColumn(this Bitmap image, int columnIndex)
		{
			int height = image.Height;
			Bitmap bitmap = new Bitmap(1, height, PixelFormat.Format32bppArgb);
			for (int i = 0; i < height; i++)
			{
				bitmap.SetPixel(0, i, image.GetPixel(columnIndex, i));
			}
			return bitmap;
		}

		public static int GetWidth(this Font font, string text, bool useAntialias)
		{
			int num;
			Bitmap bitmap = new Bitmap(1, 1);
			try
			{
				Graphics graphic = Graphics.FromImage(bitmap);
				try
				{
					if (useAntialias)
					{
						graphic.TextRenderingHint = TextRenderingHint.AntiAlias;
					}
					SizeF sizeF = graphic.MeasureString(text, font);
					int width = (int)sizeF.Width;
					int length = text.Length;
					char[] chrArray = new char[] { ' ' };
					int length1 = length - text.TrimEnd(chrArray).Length;
					if (length1 > 0)
					{
						sizeF = graphic.MeasureString(" ", font);
						width = width + length1 * (int)sizeF.Width;
					}
					num = width;
				}
				finally
				{
					if (graphic != null)
					{
						((IDisposable)graphic).Dispose();
					}
				}
			}
			finally
			{
				if (bitmap != null)
				{
					((IDisposable)bitmap).Dispose();
				}
			}
			return num;
		}

		public static Bitmap Insert(this Bitmap host, int columnIndex, Bitmap image)
		{
			Color pixel;
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			if (image.Height != host.Height)
			{
				throw new ArgumentException("The height of the specified image is different from the host image.");
			}
			int height = host.Height;
			int width = host.Width + image.Width;
			int num = columnIndex + image.Width;
			Bitmap bitmap = new Bitmap(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (j >= columnIndex)
					{
						pixel = (j >= num ? host.GetPixel(j - image.Width, i) : image.GetPixel(j - columnIndex, i));
					}
					else
					{
						pixel = host.GetPixel(j, i);
					}
					if (pixel.A != 0)
					{
						bitmap.SetPixel(j, i, pixel);
					}
					else
					{
						bitmap.SetPixel(j, i, Color.Transparent);
					}
				}
			}
			return bitmap;
		}

		public static Bitmap Rotate(this Image image, double degrees)
		{
			double num;
			double num1;
			double num2;
			double num3;
			Point[] pointArray;
			Point[] point;
			bool flag;
			if (image == null)
			{
				throw new ArgumentNullException("image");
			}
			degrees = degrees * 0.0174532925199433;
			double width = (double)image.Width;
			double height = (double)image.Height;
			if (degrees < 0 || degrees >= 1.5707963267949)
			{
				flag = (degrees >= 4.71238898038469 ? true : degrees < 3.14159265358979);
			}
			else
			{
				flag = false;
			}
			if (flag)
			{
				num = height * Math.Abs(Math.Sin(degrees));
				num1 = height * Math.Abs(Math.Cos(degrees));
				num2 = width * Math.Abs(Math.Sin(degrees));
				num3 = width * Math.Abs(Math.Cos(degrees));
			}
			else
			{
				num = width * Math.Abs(Math.Cos(degrees));
				num1 = width * Math.Abs(Math.Sin(degrees));
				num2 = height * Math.Abs(Math.Cos(degrees));
				num3 = height * Math.Abs(Math.Sin(degrees));
			}
			double num4 = num3 + num;
			double num5 = num1 + num2;
			int num6 = (int)Math.Ceiling(num4);
			int num7 = (int)Math.Ceiling(num5);
			Bitmap bitmap = new Bitmap(num6, num7);
			Graphics graphic = Graphics.FromImage(bitmap);
			try
			{
				if (!(degrees < 0 ? true : degrees >= 1.5707963267949))
				{
					point = new Point[] { new Point((int)num3, 0), new Point(num6, (int)num1), new Point(0, (int)num2) };
					pointArray = point;
				}
				else if (!(degrees >= 3.14159265358979 ? true : degrees < 1.5707963267949))
				{
					point = new Point[] { new Point(num6, (int)num1), new Point((int)num, num7), new Point((int)num3, 0) };
					pointArray = point;
				}
				else if ((degrees >= 4.71238898038469 ? true : degrees < 3.14159265358979))
				{
					point = new Point[] { new Point(0, (int)num2), new Point((int)num3, 0), new Point((int)num, num7) };
					pointArray = point;
				}
				else
				{
					point = new Point[] { new Point((int)num, num7), new Point(0, (int)num2), new Point(num6, (int)num1) };
					pointArray = point;
				}
				graphic.DrawImage(image, pointArray);
			}
			finally
			{
				if (graphic != null)
				{
					((IDisposable)graphic).Dispose();
				}
			}
			return bitmap;
		}

        //public static void SaveAsGif(this Bitmap image, string path, bool transparent)
        //{
        //    (new GifProcessor(image)).Save(path, transparent);
        //}

		public static Bitmap Stretch(this Bitmap image, int width)
		{
			if ((image == null ? true : image.Width != 1))
			{
				throw new Exception("Bitmap.Stretch() should be called on an image with one column only.");
			}
			Bitmap bitmap = new Bitmap(width, image.Height);
			for (int i = 0; i < image.Height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					bitmap.SetPixel(j, i, image.GetPixel(0, i));
				}
			}
			return bitmap;
		}
	}
}