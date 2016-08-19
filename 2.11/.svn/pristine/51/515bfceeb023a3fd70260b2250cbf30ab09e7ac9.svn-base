using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace System.Security
{
	public class Encryption
	{
		public Encryption()
		{
		}

		public static string CreateHash(string clearText)
		{
			string base64String = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(clearText)));
			base64String = string.Concat("«6\"£k&36 2", base64String, "mmñÃ5d*");
			return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(base64String))).TrimEnd(new char[] { '=' });
		}

		private static RSACryptoServiceProvider CreateRSACryptoServiceProvider()
		{
			string str = string.Concat("SpiderContainer", Guid.NewGuid());
			CspParameters cspParameter = new CspParameters(1)
			{
				KeyContainerName = str,
				Flags = CspProviderFlags.UseMachineKeyStore,
				ProviderName = "Microsoft Strong Cryptographic Provider"
			};
			return new RSACryptoServiceProvider(cspParameter);
		}

		public static string Decrypt(string encryptedText, string password)
		{
			string str;
			Encoding aSCII = Encoding.ASCII;
			int length = password.Length;
			PasswordDeriveBytes passwordDeriveByte = new PasswordDeriveBytes(password, aSCII.GetBytes(length.ToString()));
			byte[] numArray = Convert.FromBase64String(encryptedText);
			ICryptoTransform cryptoTransform = (new RijndaelManaged()).CreateDecryptor(passwordDeriveByte.GetBytes(32), passwordDeriveByte.GetBytes(16));
			MemoryStream memoryStream = new MemoryStream(numArray);
			try
			{
				CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
				try
				{
					byte[] numArray1 = new byte[(int)numArray.Length];
					int num = cryptoStream.Read(numArray1, 0, (int)numArray1.Length);
					str = Encoding.Unicode.GetString(numArray1, 0, num);
				}
				finally
				{
					if (cryptoStream != null)
					{
						((IDisposable)cryptoStream).Dispose();
					}
				}
			}
			finally
			{
				if (memoryStream != null)
				{
					((IDisposable)memoryStream).Dispose();
				}
			}
			return str;
		}

		public static string DecryptAsymmetric(string encodedText, string publicPrivateKeyXml)
		{
			string str;
			if (encodedText.IsEmpty())
			{
				throw new ArgumentNullException("encodedText");
			}
			if (publicPrivateKeyXml.IsEmpty())
			{
				throw new ArgumentNullException("publicPrivateKeyXml");
			}
			if (!encodedText.Contains("|"))
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = Encryption.CreateRSACryptoServiceProvider();
				byte[] numArray = Convert.FromBase64String(encodedText);
				rSACryptoServiceProvider.FromXmlString(publicPrivateKeyXml);
				byte[] numArray1 = rSACryptoServiceProvider.Decrypt(numArray, false);
				str = Encoding.UTF8.GetString(numArray1);
			}
			else
			{
				char[] chrArray = new char[] { '|' };
				str = (
					from p in encodedText.Split(chrArray)
					select Encryption.DecryptAsymmetric(p, publicPrivateKeyXml)).ToString<string>(string.Empty);
			}
			return str;
		}

		public static string Encrypt(string text, string password)
		{
			string base64String;
			byte[] bytes = Encoding.Unicode.GetBytes(text);
			ICryptoTransform cryptoTransform = (new RijndaelManaged()).CreateEncryptor();
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
				try
				{
					cryptoStream.Write(bytes, 0, (int)bytes.Length);
					cryptoStream.FlushFinalBlock();
					base64String = Convert.ToBase64String(memoryStream.ToArray());
				}
				finally
				{
					if (cryptoStream != null)
					{
						((IDisposable)cryptoStream).Dispose();
					}
				}
			}
			finally
			{
				if (memoryStream != null)
				{
					((IDisposable)memoryStream).Dispose();
				}
			}
			return base64String;
		}

		public static string EncryptAsymmetric(string text, string publicKeyXml)
		{
			string base64String;
			if (text.IsEmpty())
			{
				throw new ArgumentNullException("text");
			}
			if (publicKeyXml.IsEmpty())
			{
				throw new ArgumentNullException("publicKeyXml");
			}
			if (text.Length <= 117)
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = Encryption.CreateRSACryptoServiceProvider();
				rSACryptoServiceProvider.FromXmlString(publicKeyXml);
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				base64String = Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(bytes, false));
			}
			else
			{
				base64String = (
					from p in text.Split(117)
					select Encryption.EncryptAsymmetric(p, publicKeyXml)).ToString<string>("|");
			}
			return base64String;
		}

		public static KeyValuePair<string, string> GenerateAsymmetricKeys()
		{
			RSACryptoServiceProvider rSACryptoServiceProvider = Encryption.CreateRSACryptoServiceProvider();
			KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(rSACryptoServiceProvider.ToXmlString(false), rSACryptoServiceProvider.ToXmlString(true));
			return keyValuePair;
		}

		private static PasswordDeriveBytes GenerateSymmetricKey(string password)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(password.Length.ToString());
			return new PasswordDeriveBytes(password, bytes);
		}
	}
}