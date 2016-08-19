using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
	public static class StringExtensions
	{
		public static IEnumerable<int> AllIndicesOf(this string text, string pattern)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			List<int> nums = new List<int>();
			int num = -1;
			do
			{
				num = text.IndexOf(pattern, num + 1);
				if (num <= -1)
				{
					continue;
				}
				nums.Add(num);
			}
			while (num > -1);
			return nums;
		}

		public static bool Contains(this string text, string subString, bool caseSensitive)
		{
			bool flag;
			if (!(text != null ? true : subString != null))
			{
				flag = true;
			}
			else if (text == null)
			{
				flag = false;
			}
			else if (!subString.IsEmpty())
			{
				flag = (!caseSensitive ? text.ToUpper().Contains(subString.Get<string, string>((string s) => s.ToUpper())) : text.Contains(subString));
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public static string Count<T>(this IEnumerable<T> list, string objectTitle)
		{
			if (objectTitle.IsEmpty())
			{
				objectTitle = typeof(T).Name.SeparateAtUpperCases();
			}
			return objectTitle.ToCountString(list.Count<T>());
		}

		public static string Count<T>(this IEnumerable<T> list, string objectTitle, string zeroQualifier)
		{
			if (objectTitle.IsEmpty())
			{
				objectTitle = typeof(T).Name.SeparateAtUpperCases();
			}
			string countString = objectTitle.ToCountString(list.Count<T>(), zeroQualifier);
			return countString;
		}

		public static bool EndsWithAny(this string input, params string[] listOfEndings)
		{
			bool flag;
			string[] strArrays = listOfEndings;
			int num = 0;
			while (true)
			{
				if (num >= (int)strArrays.Length)
				{
					flag = false;
					break;
				}
				else if (!input.EndsWith(strArrays[num]))
				{
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public static string FormatWith(this string format, object arg, params object[] additionalArgs)
		{
			string str;
			if ((additionalArgs != null && (int)additionalArgs.Length != 0))
			{
				object[] objArray = new object[] { arg };
				str = string.Format(format, objArray.Concat<object>(additionalArgs).ToArray<object>());
			}
			else
			{
				str = string.Format(format, arg);
			}
			return str;
		}

		private static string GetIrregularPlural(string singular)
		{
			string str;
			singular = singular.ToLower();
			string str1 = singular;
			if (str1 != null)
			{
				switch (str1)
				{
					case "addendum":
					{
						str = "addenda";
						break;
					}
					case "alga":
					{
						str = "algae";
						break;
					}
					case "alumna":
					{
						str = "alumnae";
						break;
					}
					case "alumnus":
					{
						str = "alumni";
						break;
					}
					case "analysis":
					{
						str = "analyses";
						break;
					}
					case "apparatus":
					{
						str = "apparatuses";
						break;
					}
					case "appendix":
					{
						str = "appendices";
						break;
					}
					case "axis":
					{
						str = "axes";
						break;
					}
					case "bacillus":
					{
						str = "bacilli";
						break;
					}
					case "bacterium":
					{
						str = "bacteria";
						break;
					}
					case "basis":
					{
						str = "bases";
						break;
					}
					case "beau":
					{
						str = "beaux";
						break;
					}
					case "bison":
					{
						str = "bison";
						break;
					}
					case "buffalo":
					{
						str = "buffaloes";
						break;
					}
					case "bureau":
					{
						str = "bureaus";
						break;
					}
					case "calf":
					{
						str = "calves";
						break;
					}
					case "child":
					{
						str = "children";
						break;
					}
					case "corps":
					{
						str = "corps";
						break;
					}
					case "crisis":
					{
						str = "crises";
						break;
					}
					case "criterion":
					{
						str = "criteria";
						break;
					}
					case "curriculum":
					{
						str = "curricula";
						break;
					}
					case "datum":
					{
						str = "data";
						break;
					}
					case "deer":
					{
						str = "deer";
						break;
					}
					case "die":
					{
						str = "dice";
						break;
					}
					case "dwarf":
					{
						str = "dwarfs";
						break;
					}
					case "diagnosis":
					{
						str = "diagnoses";
						break;
					}
					case "echo":
					{
						str = "echoes";
						break;
					}
					case "elf":
					{
						str = "elves";
						break;
					}
					case "ellipsis":
					{
						str = "ellipses";
						break;
					}
					case "embargo":
					{
						str = "embargoes";
						break;
					}
					case "emphasis":
					{
						str = "emphases";
						break;
					}
					case "erratum":
					{
						str = "errata";
						break;
					}
					case "fireman":
					{
						str = "firemen";
						break;
					}
					case "fish":
					{
						str = "fish";
						break;
					}
					case "focus":
					{
						str = "focus";
						break;
					}
					case "foot":
					{
						str = "feet";
						break;
					}
					case "formula":
					{
						str = "formulas";
						break;
					}
					case "fungus":
					{
						str = "fungi";
						break;
					}
					case "genus":
					{
						str = "genera";
						break;
					}
					case "goose":
					{
						str = "geese";
						break;
					}
					case "half":
					{
						str = "halves";
						break;
					}
					case "hero":
					{
						str = "heroes";
						break;
					}
					case "hippopotamus":
					{
						str = "hippopotami";
						break;
					}
					case "hoof":
					{
						str = "hoofs";
						break;
					}
					case "hypothesis":
					{
						str = "hypotheses";
						break;
					}
					case "index":
					{
						str = "indices";
						break;
					}
					case "knife":
					{
						str = "knives";
						break;
					}
					case "leaf":
					{
						str = "leaves";
						break;
					}
					case "life":
					{
						str = "lives";
						break;
					}
					case "loaf":
					{
						str = "loaves";
						break;
					}
					case "louse":
					{
						str = "lice";
						break;
					}
					case "man":
					{
						str = "men";
						break;
					}
					case "matrix":
					{
						str = "matrices";
						break;
					}
					case "means":
					{
						str = "means";
						break;
					}
					case "medium":
					{
						str = "media";
						break;
					}
					case "memorandum":
					{
						str = "memoranda";
						break;
					}
					case "millennium":
					{
						str = "milennia";
						break;
					}
					case "moose":
					{
						str = "moose";
						break;
					}
					case "mosquito":
					{
						str = "mosquitoes";
						break;
					}
					case "mouse":
					{
						str = "mice";
						break;
					}
					case "nebula":
					{
						str = "nebulas";
						break;
					}
					case "neurosis":
					{
						str = "neuroses";
						break;
					}
					case "nucleus":
					{
						str = "nuclei";
						break;
					}
					case "oasis":
					{
						str = "oases";
						break;
					}
					case "octopus":
					{
						str = "octopi";
						break;
					}
					case "ovum":
					{
						str = "ova";
						break;
					}
					case "ox":
					{
						str = "oxen";
						break;
					}
					case "paralysis":
					{
						str = "paralyses";
						break;
					}
					case "parenthesis":
					{
						str = "parentheses";
						break;
					}
					case "person":
					{
						str = "people";
						break;
					}
					case "phenomenon":
					{
						str = "phenomena";
						break;
					}
					case "potato":
					{
						str = "potatoes";
						break;
					}
					case "scarf":
					{
						str = "scarfs";
						break;
					}
					case "self":
					{
						str = "selves";
						break;
					}
					case "series":
					{
						str = "series";
						break;
					}
					case "sheep":
					{
						str = "sheep";
						break;
					}
					case "shelf":
					{
						str = "shelves";
						break;
					}
					case "scissors":
					{
						str = "scissors";
						break;
					}
					case "species":
					{
						str = "species";
						break;
					}
					case "stimulus":
					{
						str = "stimuli";
						break;
					}
					case "stratum":
					{
						str = "strata";
						break;
					}
					case "synthesis":
					{
						str = "syntheses";
						break;
					}
					case "synopsis":
					{
						str = "synopses";
						break;
					}
					case "tableau":
					{
						str = "tableaux";
						break;
					}
					case "that":
					{
						str = "those";
						break;
					}
					case "thesis":
					{
						str = "theses";
						break;
					}
					case "thief":
					{
						str = "thieves";
						break;
					}
					case "this":
					{
						str = "these";
						break;
					}
					case "tomato":
					{
						str = "tomatoes";
						break;
					}
					case "tooth":
					{
						str = "teeth";
						break;
					}
					case "torpedo":
					{
						str = "torpedoes";
						break;
					}
					case "vertebra":
					{
						str = "vertebrae";
						break;
					}
					case "veto":
					{
						str = "vetoes";
						break;
					}
					case "vita":
					{
						str = "vitae";
						break;
					}
					case "watch":
					{
						str = "watches";
						break;
					}
					case "wife":
					{
						str = "wives";
						break;
					}
					case "wolf":
					{
						str = "wolves";
						break;
					}
					case "woman":
					{
						str = "women";
						break;
					}
					default:
					{
						str = "";
						return str;
					}
				}
			}
			else
			{
				str = "";
				return str;
			}
			return str;
		}

		public static string GetLastChar(this string input)
		{
			string str;
			if (!input.HasValue())
			{
				str = null;
			}
			else
			{
				str = (input.Length < 1 ? input : input.Substring(input.Length - 1, 1));
			}
			return str;
		}

		private static string GetRegularPlural(string singular)
		{
			char chr;
			string str;
			char lower = char.ToLower(singular[singular.Length - 1]);
			chr = (singular.Length <= 1 ? '\0' : char.ToLower(singular[singular.Length - 2]));
			if ((lower == 's' || string.Concat(chr.ToString(), lower.ToString()) == "ch" ? false : !(string.Concat(chr.ToString(), lower.ToString()) == "sh")))
			{
				str = ((lower != 'y' || chr == 'a' || chr == 'e' || chr == 'o' || chr == 'i' ? true : chr == 'u') ? string.Concat(singular, "s") : string.Concat(singular.Substring(0, singular.Length - 1), "ies"));
			}
			else
			{
				str = string.Concat(singular, "es");
			}
			return str;
		}

		public static bool HasValue(this string text)
		{
			return !string.IsNullOrEmpty(text);
		}

		public static bool IsEmpty(this string text)
		{
			return string.IsNullOrEmpty(text);
		}

		public static bool IsLetter(this char c)
		{
			return char.IsLetter(c);
		}

		public static bool IsLetterOrDigit(this char c)
		{
			return char.IsLetterOrDigit(c);
		}

		public static bool IsLower(this char c)
		{
			return char.IsLower(c);
		}

		public static bool IsUpper(this char c)
		{
			return char.IsUpper(c);
		}

		public static string Left(this string s, int length)
		{
			string str;
			length = Math.Max(length, 0);
		 	str = s != null ? (s.Length <= length ? s : s.Substring(0, length)) : "No Name";
			return str;
		}

        public static string MaybeValue(this string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value)
                ? defaultValue
                : value;
        }

        public static string OnlyWhen(this string code, bool condition)
		{
			return (!condition ? string.Empty : code);
		}

		public static string Or(this string text, string defaultValue)
		{
			return (!string.IsNullOrEmpty(text) ? text : defaultValue);
		}

		public static string Remove(this string text, params string[] substringsToExclude)
		{
			string str;
			if (!text.IsEmpty())
			{
				string str1 = text;
				string[] strArrays = substringsToExclude;
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					str1 = str1.Replace(strArrays[i], "");
				}
				str = str1;
			}
			else
			{
				str = text;
			}
			return str;
		}

		public static string RemoveHtmlTags(this string source)
		{
			source = source.Replace("<br/>", Environment.NewLine).Replace("<br />", Environment.NewLine).Replace("<br>", Environment.NewLine).Replace("<br >", Environment.NewLine).Replace("<p>", Environment.NewLine);
			string[] strArrays = new string[] { "&quot;", "&apos;", "&amp;", "&lt;", "&gt;", "&nbsp;", "&iexcl;", "&cent;", "&pound;", "&curren;", "&yen;", "&brvbar;", "&sect;", "&uml;", "&copy;", "&ordf;", "&laquo;", "&not;", "&shy;", "&reg;", "&macr;", "&deg;", "&plusmn;", "&sup2;", "&sup3;", "&acute;", "&micro;", "&para;", "&middot;", "&cedil;", "&sup1;", "&ordm;", "&raquo;", "&frac14;", "&frac12;", "&frac34;", "&iquest;", "&times;", "&divide;", "&Agrave;", "&Aacute;", "&Acirc;", "&Atilde;", "&Auml;", "&Aring;", "&AElig;", "&Ccedil;", "&Egrave;", "&Eacute;", "&Ecirc;", "&Euml;", "&Igrave;", "&Iacute;", "&Icirc;", "&Iuml;", "&ETH;", "&Ntilde;", "&Ograve;", "&Oacute;", "&Ocirc;", "&Otilde;", "&Ouml;", "&Oslash;", "&Ugrave;", "&Uacute;", "&Ucirc;", "&Uuml;", "&Yacute;", "&THORN;", "&szlig;", "&agrave;", "&aacute;", "&acirc;", "&atilde;", "&auml;", "&aring;", "&aelig;", "&ccedil;", "&egrave;", "&eacute;", "&ecirc;", "&euml;", "&igrave;", "&iacute;", "&icirc;", "&iuml;", "&eth;", "&ntilde;", "&ograve;", "&oacute;", "&ocirc;", "&otilde;", "&ouml;", "&oslash;", "&ugrave;", "&uacute;", "&ucirc;", "&uuml;", "&yacute;", "&thorn;", "&yuml;" };
			string[] strArrays1 = strArrays;
			strArrays = new string[] { "\"", "'", "&", "<", ">", " ", "¡", "¢", "£", "¤", "¥", "¦", "§", "¨", "©", "ª", "«", "¬", "-", "®", "¯", "°", "±", "²", "³", "´", "µ", "¶", "•", "¸", "¹", "º", "»", "¼", "½", "¾", "¿", "×", "÷", "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Ð", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "Ø", "Ù", "Ú", "Û", "Ü", "Ý", "Þ", "ß", "à", "á", "â", "ã", "ä", "å", "æ", "ç", "è", "é", "ê", "ë", "ì", "í", "î", "ï", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "ø", "ù", "ú", "û", "ü", "ý", "þ", "ÿ" };
			string[] strArrays2 = strArrays;
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				source = source.Replace(strArrays1[i], strArrays2[i]);
			}
			return Regex.Replace(source, "<(.|\\n)*?>", " ").Trim();
		}

		public static string Repeat(this string text, int times)
		{
			return text.Repeat(times, null);
		}

		public static string Repeat(this string text, int times, string seperator)
		{
			if (times < 1)
			{
				throw new ArgumentOutOfRangeException("times", "times should be more than 0");
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 1; i <= times; i++)
			{
				stringBuilder.Append(text);
				if (seperator != null)
				{
					stringBuilder.Append(seperator);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ReplaceAll(this string text, string original, string substitute)
		{
			string str;
			if (!text.IsEmpty())
			{
				while (text.Contains(original))
				{
					text = text.Replace(original, substitute);
				}
				str = text;
			}
			else
			{
				str = text;
			}
			return str;
		}

        public static string ReplaceWholeWord(this string text, string toReplace, string replaceWith)
        {
            return Regex.Replace(text, "\\b" + toReplace + "\\b", replaceWith);
        }

        public static string Right(this string s, int length)
		{
			string str;
			length = Math.Max(length, 0);
			str = (s.Length <= length ? s : s.Substring(s.Length - length, length));
			return str;
		}

		public static string SeparateAtUpperCases(this string pascalCase)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < pascalCase.Length; i++)
			{
				if ((!char.IsUpper(pascalCase[i]) ? false : i > 0))
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(pascalCase[i]);
			}
			return stringBuilder.ToString().ToLower();
		}

		public static bool StartsWithAny(this string input, params string[] listOfBeginnings)
		{
			bool flag;
			string[] strArrays = listOfBeginnings;
			int num = 0;
			while (true)
			{
				if (num >= (int)strArrays.Length)
				{
					flag = false;
					break;
				}
				else if (!input.StartsWith(strArrays[num]))
				{
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public static string Summarize(this string text, int maximumLength, bool enforceMaxLength)
		{
			string str = text.Summarize(maximumLength);
			if ((!enforceMaxLength ? false : str.Length > maximumLength))
			{
				str = string.Concat(text.Substring(0, maximumLength - 3), "...");
			}
			return str;
		}

		public static string Summarize(this string text, int maximumLength)
		{
			string str;
			if (!text.IsEmpty())
			{
				if (text.Length > maximumLength)
				{
					text = text.Substring(0, maximumLength);
					int num = -1;
					char[] charArray = " \r\n\t".ToCharArray();
					for (int i = 0; i < (int)charArray.Length; i++)
					{
						char chr = charArray[i];
						num = Math.Max(text.LastIndexOf(chr), num);
					}
					if (num > maximumLength / 2)
					{
						text = text.Substring(0, num);
					}
					text = string.Concat(text, "...");
				}
				str = text;
			}
			else
			{
				str = text;
			}
			return str;
		}

		public static string ToCountString(this string name, int count)
		{
			string str = "no";
			if ((!name.HasValue() ? false : char.IsUpper(name[0])))
			{
				str = "No";
			}
			return name.ToCountString(count, str);
		}

		public static string ToCountString(this string name, int count, string zeroQualifier)
		{
			string str;
			name = name.Or("").Trim();
			if (name.IsEmpty())
			{
				throw new Exception("'name' cannot be empty for ToCountString().");
			}
			if (count < 0)
			{
				throw new ArgumentException("count should be greater than or equal to 0.");
			}
			if (count != 0)
			{
				str = (count != 1 ? string.Format("{0} {1}", count, name.ToPlural()) : string.Concat("1 ", name));
			}
			else
			{
				str = string.Concat(zeroQualifier, " ", name);
			}
			return str;
		}

		public static string[] ToLines(this string text)
		{
			string[] newLine = new string[] { Environment.NewLine };
			return text.Split(newLine, StringSplitOptions.None);
		}

		public static char ToLower(this char c)
		{
			return char.ToLower(c);
		}

		public static string ToPlural(this string singular)
		{
			string regularPlural;
			string empty;
			if (!singular.IsEmpty())
			{
				string str = singular;
				string str1 = "";
				char[] chrArray = new char[] { ' ' };
				if ((int)str.Split(chrArray).Length > 1)
				{
					str1 = string.Concat(str.Substring(0, str.LastIndexOf(" ")), " ");
					singular = str.Substring(str.LastIndexOf(" ") + 1);
				}
				string irregularPlural = StringExtensions.GetIrregularPlural(singular);
				if (!(irregularPlural != ""))
				{
					regularPlural = StringExtensions.GetRegularPlural(singular);
				}
				else
				{
					if (str1 == "")
					{
						irregularPlural = string.Concat(char.ToUpper(irregularPlural[0]), irregularPlural.Substring(1));
					}
					regularPlural = irregularPlural;
				}
				empty = string.Concat(str1, regularPlural);
			}
			else
			{
				empty = string.Empty;
			}
			return empty;
		}

		public static char ToUpper(this char c)
		{
			return char.ToUpper(c);
		}

		public static string TrimEnd(this string text, int numberOfCharacters)
		{
			string str;
			if (numberOfCharacters < 0)
			{
				throw new ArgumentException("numberOfCharacters must be greater than 0.");
			}
			if (numberOfCharacters != 0)
			{
				str = ((text.IsEmpty() ? false : text.Length > numberOfCharacters) ? text.Substring(0, text.Length - numberOfCharacters) : string.Empty);
			}
			else
			{
				str = text;
			}
			return str;
		}

		public static string TrimEnd(this string text, string unnecessaryText)
		{
			string str;
			if (unnecessaryText.IsEmpty())
			{
				throw new ArgumentNullException("unnecessaryText");
			}
			if (!text.IsEmpty())
			{
				str = (!text.EndsWith(unnecessaryText) ? text : text.TrimEnd(unnecessaryText.Length));
			}
			else
			{
				str = text;
			}
			return str;
		}

		public static string TrimStart(this string text, string textToTrim)
		{
			string str;
			str = (!text.StartsWith(textToTrim) ? text : text.Substring(textToTrim.Length).TrimStart(textToTrim));
			return str;
		}

		public static string WithPrefix(this string text, string prefix)
		{
			string str;
			str = (!text.IsEmpty() ? string.Concat(prefix, text) : string.Empty);
			return str;
		}

		public static string WithSuffix(this string text, string suffix)
		{
			string str;
			str = (!text.IsEmpty() ? string.Concat(text, suffix) : string.Empty);
			return str;
		}

		public static string WithWrappers(this string text, string left, string right)
		{
			string str;
			str = (!text.IsEmpty() ? string.Concat(left, text, right) : string.Empty);
			return str;
		}

     
        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static float AsNumericAbs(this string s)
        {
            float output;
            float.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out output);
            return Math.Abs(output);
        }

        public static float AsNumeric(this string s)
        {
            float output;
            float.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out output);
            return output;
        }

        public static double AsNumericDouble(this string s)
        {
            double output;
            double.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out output);
            return output;
        }

        public static int AsNumericAbsInt(this string s)
        {
            int output;
            int.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out output);
            return Math.Abs(output);
        }

        public static int AsNumericInt(this string s)
        {
            int output;
            Int32.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out output);
            return output;
        }

        public static decimal AsNumericDecimal(this string s)
        {
            decimal output;
            decimal.TryParse(s, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out output);
            return output;
        }

        public static string Clean(this string str)
        {
            char[] arr = str.ToCharArray();

            arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                              || char.IsWhiteSpace(c)
                                              || c == '-')));
            str = new string(arr);

            return str;
        }

        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}