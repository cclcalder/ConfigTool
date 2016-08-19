using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Drawing.Imaging
{
	public static class BitmapHelper
	{
		public static Image Brighten(this Image source, int level)
		{
			Image image;
			if ((level < 0 ? true : level > 255))
			{
				throw new ArgumentException("Level must be between 0 and 255.");
			}
			Bitmap bitmap = new Bitmap(source);
			Graphics graphic = Graphics.FromImage(bitmap);
			try
			{
				graphic.FillRectangle(new SolidBrush(Color.FromArgb(level, Color.White)), new Rectangle(Point.Empty, source.Size));
				image = bitmap;
			}
			finally
			{
				if (graphic != null)
				{
					((IDisposable)graphic).Dispose();
				}
			}
			return image;
		}

		public static Graphics CreateGraphics(this Image image)
		{
			Graphics empty = Graphics.FromImage(image);
			empty.PageUnit = GraphicsUnit.Pixel;
			empty.RenderingOrigin = Point.Empty;
			empty.PageScale = 1f;
			empty.TextRenderingHint = TextRenderingHint.AntiAlias;
			empty.SmoothingMode = SmoothingMode.AntiAlias;
			return empty;
		}

		public static Image Crop(this Image image, Rectangle rectangle)
		{
			Bitmap bitmap = new Bitmap(image, rectangle.Size);
			Graphics graphic = bitmap.CreateGraphics();
			try
			{
				graphic.DrawImage(image, new Rectangle(Point.Empty, bitmap.Size), rectangle, GraphicsUnit.Pixel);
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

		public static Image FromBuffer(byte[] buffer)
		{
			Image image;
			MemoryStream memoryStream = new MemoryStream(buffer);
			try
			{
				image = Image.FromStream(memoryStream);
			}
			finally
			{
				if (memoryStream != null)
				{
					((IDisposable)memoryStream).Dispose();
				}
			}
			return image;
		}

		private static ImageCodecInfo GenerateCodecInfo(ImageFormat format)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			return imageEncoders.FirstOrDefault<ImageCodecInfo>((ImageCodecInfo a) => a.FormatID == format.Guid) ?? imageEncoders.ElementAt<ImageCodecInfo>(1);
		}

		private static EncoderParameters GenerateEncoderParameters(int quality)
		{
			EncoderParameters encoderParameter = new EncoderParameters(1);
			encoderParameter.Param[0] = new EncoderParameter(Encoder.Quality, (long)quality);
			return encoderParameter;
		}

		public static bool IsValidImage(byte[] buffer)
		{
			bool flag;
			try
			{
				flag = BitmapHelper.FromBuffer(buffer) != null;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static Image Resize(this Image source, int newWidth, int newHeight)
		{
			Image image;
			Bitmap bitmap = new Bitmap(newWidth, newHeight);
			Graphics graphic = Graphics.FromImage(bitmap);
			try
			{
				graphic.DrawImage(source, 0, 0, newWidth, newHeight);
				image = bitmap;
			}
			finally
			{
				if (graphic != null)
				{
					((IDisposable)graphic).Dispose();
				}
			}
			return image;
		}

		public static byte[] ToBuffer(this Image image)
		{
			return image.ToBuffer(image.RawFormat, 100);
		}

		public static byte[] ToBuffer(this Image image, ImageFormat format, int quality)
		{
			byte[] array;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				image.Save(memoryStream, BitmapHelper.GenerateCodecInfo(format), BitmapHelper.GenerateEncoderParameters(quality));
				array = memoryStream.ToArray();
			}
			finally
			{
				if (memoryStream != null)
				{
					((IDisposable)memoryStream).Dispose();
				}
			}
			return array;
		}
	}
}