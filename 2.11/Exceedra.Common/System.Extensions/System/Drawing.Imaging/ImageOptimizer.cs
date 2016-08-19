using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace System.Drawing.Imaging
{
	public class ImageOptimizer
	{
		public int MaximumHeight
		{
			get;
			set;
		}

		public int MaximumWidth
		{
			get;
			set;
		}

		public ImageOptimizer.ImageFormat OutputFormat
		{
			get;
			set;
		}

		public int Quality
		{
			get;
			set;
		}

		public ImageOptimizer() : this(900, 700, 80)
		{
		}

		public ImageOptimizer(int maxWidth, int maxHeight, int quality)
		{
			this.MaximumWidth = maxWidth;
			this.MaximumHeight = maxHeight;
			this.Quality = quality;
			this.OutputFormat = ImageOptimizer.ImageFormat.Jpeg;
		}

		private ImageCodecInfo GenerateCodecInfo()
		{
			return ImageCodecInfo.GetImageEncoders()[(int)this.OutputFormat];
		}

		private EncoderParameters GenerateEncoderParameters()
		{
			EncoderParameters encoderParameter = new EncoderParameters(1);
			encoderParameter.Param[0] = new EncoderParameter(Encoder.Quality, (long)this.Quality);
			return encoderParameter;
		}

		public Image Optimize(Image source)
		{
			Bitmap bitmap = new Bitmap(source);
			int width = source.Width;
			int height = source.Height;
			if (width > this.MaximumWidth)
			{
				height = (int)((double)height * (1 * (double)this.MaximumWidth) / (double)width);
				width = this.MaximumWidth;
			}
			if (height > this.MaximumHeight)
			{
				width = (int)((double)width * (1 * (double)this.MaximumHeight) / (double)height);
				height = this.MaximumHeight;
			}
			if ((width != source.Width ? true : height != source.Height))
			{
				bitmap = new Bitmap(bitmap, width, height);
			}
			return bitmap;
		}

		public byte[] Optimize(byte[] sourceData)
		{
			byte[] buffer;
			try
			{
				Image image = BitmapHelper.FromBuffer(sourceData);
				try
				{
					buffer = this.Optimize(image).ToBuffer();
				}
				finally
				{
					if (image != null)
					{
						((IDisposable)image).Dispose();
					}
				}
			}
			catch
			{
				buffer = sourceData;
			}
			return buffer;
		}

		public void Optimize(string souceImagePath, string optimizedImagePath)
		{
			Image image;
			if (!File.Exists(souceImagePath))
			{
				throw new Exception(string.Concat("Could not find the file: ", souceImagePath));
			}
			try
			{
				image = Image.FromFile(souceImagePath);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new Exception("Could not obtain bitmap data from the file: {0}.".FormatWith(souceImagePath, new object[0]), exception);
			}
			Image image1 = image;
			try
			{
				Image image2 = this.Optimize(image);
				try
				{
					image2.Save(optimizedImagePath, this.GenerateCodecInfo(), this.GenerateEncoderParameters());
				}
				finally
				{
					if (image2 != null)
					{
						((IDisposable)image2).Dispose();
					}
				}
			}
			finally
			{
				if (image1 != null)
				{
					((IDisposable)image1).Dispose();
				}
			}
		}

		public void Optimize(string imagePath)
		{
			this.Optimize(imagePath, imagePath);
		}

		public enum ImageFormat
		{
			Bmp = 0,
			Jpeg = 1,
			Gif = 2,
			Png = 4
		}
	}
}