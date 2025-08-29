using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UndertaleModTool.ProjectTool
{
    public static class YYJson
    {
        static string INDENT = "  ";

		/// <summary>
		/// Depth for objects to start being flattened
		/// </summary>
		static int flatDepth = 2;

		/// <summary>
		/// Add newline and indentation depending on depth
		/// </summary>
		static void YYIndent(this StringBuilder sb, int depth)
		{
			sb.Append('\n');
			foreach (var item in Enumerable.Range(0, depth))
				sb.Append(INDENT);
		}

		/// <summary>
		/// Format a JSON like the weird inconsistent way GameMaker does it
		/// </summary>
        public static string Format(string json)
        {
            StringBuilder tcws = new();
			Stack<bool> depth = new();

			bool isObject = false;
			bool escapeNextChar = false;
			bool isString = false;

			bool isTileData = false;
			uint tileDataWidth = 0; // SerialiseWidth
			uint tileCount = 0; // Each ,

            for (var _ = 0; _ < json.Length; _++)
            {
				char c = json[_];
				char nextC = '\0';
				char prevC = '\0';

				if (_ > 0)
					prevC = json[_ - 1];
				if (_ < json.Length - 1)
					nextC = json[_ + 1];

                switch (c)
                {
					case '\\':
						escapeNextChar = !escapeNextChar;
						tcws.Append(c);
						break;

					case '\"':
						if (escapeNextChar)
							goto default;

						isString = !isString;
						tcws.Append(c);
						break;

                    case '{':
						if (isString)
							goto default;

						tcws.Append(c);
						depth.Push(isObject);
						if (depth.Count <= flatDepth)
						{
							isObject = false;
							if (nextC != '}')
								tcws.YYIndent(depth.Count);
						}
						else
							isObject = true;
						break;

                    case '}':
						if (isString)
							goto default;

						if (prevC != '{')
						{
							tcws.Append(',');
							if (!isObject)
								tcws.YYIndent(depth.Count - 1);
						}
						isObject = depth.Pop();
						tcws.Append(c);
						break;

					case '[':
						if (isString)
							goto default;

						var temporaryString = tcws.ToString();
						isTileData = temporaryString.EndsWith("\"TileCompressedData\":") || temporaryString.EndsWith("\"TileSerialiseData\":");

						if (isTileData)
						{
							tileDataWidth = 0;

							var regex = Regex.Match(temporaryString, @"""SerialiseWidth"":(\d+)", RegexOptions.RightToLeft);
							if (regex.Success)
								tileDataWidth = uint.Parse(regex.Groups[1].Value);

							if (tileDataWidth <= 0)
								tileDataWidth = 50;
						}

						tcws.Append(c);
						depth.Push(isObject);
						isObject = false;

						if (nextC != ']')
						{
							if (isTileData)
								tcws.YYIndent(0);
							else
								tcws.YYIndent(depth.Count);
						}
						break;

					case ']':
						if (isString)
							goto default;

						isObject = depth.Pop();
						if (prevC != '[')
						{
							tcws.Append(',');
							if (!isTileData)
								tcws.YYIndent(depth.Count);
						}
						isTileData = false;
						tcws.Append(c);
						break;

                    case ':':
						if (isString)
							goto default;

						tcws.Append(c);
						if (!isObject)
							tcws.Append(' ');
						break;

					case ',':
						if (isString)
							goto default;

						tcws.Append(c);

						if (isTileData)
						{
							if (++tileCount >= tileDataWidth)
							{
								tcws.YYIndent(0);
								tileCount = 0;
							}
						}
						else if (!isObject)
							tcws.YYIndent(depth.Count);
						break;

					default:
						tcws.Append(c);
						break;
                }

				if (c != '\\')
					escapeNextChar = false;
			}

            return tcws.ToString();
        }
    }
}
