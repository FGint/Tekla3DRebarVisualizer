using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Tekla.Structures;

namespace cs_net_lib
{
	public class ContentAttributesReader
	{
		private List<string> attributesFilesTexts;

		public ContentAttributesReader()
		{
			this.attributesFilesTexts = new List<string>();
			string empty = string.Empty;
			TeklaStructuresSettings.GetAdvancedOption("XS_DIR", ref empty);
			empty = string.Concat(empty, "\\nt\\TplEd\\settings");
			string contentAttributesGlobalText = ContentAttributesReader.GetContentAttributesGlobalText(empty, "\\contentattributes_global.lst");
			this.attributesFilesTexts.Add(ContentAttributesReader.GetTextWithoutComments(contentAttributesGlobalText, true));
			contentAttributesGlobalText = ContentAttributesReader.GetContentAttributesGlobalText(empty, "\\contentattributes_userdefined.lst");
			this.attributesFilesTexts.Add(ContentAttributesReader.GetReadableText(ContentAttributesReader.GetTextWithoutComments(contentAttributesGlobalText, true)));
			contentAttributesGlobalText = ContentAttributesReader.GetContentAttributesGlobalText(empty, "\\contentattributes_ArchiCAD.lst");
			this.attributesFilesTexts.Add(ContentAttributesReader.GetTextWithoutComments(contentAttributesGlobalText, true));
			contentAttributesGlobalText = ContentAttributesReader.GetContentAttributesGlobalText(empty, "\\contentattributes_MagiCAD.lst");
			this.attributesFilesTexts.Add(ContentAttributesReader.GetTextWithoutComments(contentAttributesGlobalText, true));
			contentAttributesGlobalText = ContentAttributesReader.GetContentAttributesGlobalText(empty, "\\contentattributes_QuickPen.lst");
			this.attributesFilesTexts.Add(ContentAttributesReader.GetTextWithoutComments(contentAttributesGlobalText, true));
			contentAttributesGlobalText = ContentAttributesReader.GetContentAttributesGlobalText(empty, "\\contentattributes_Revit.lst");
			this.attributesFilesTexts.Add(ContentAttributesReader.GetTextWithoutComments(contentAttributesGlobalText, true));
		}

		private static string FindAttributeAndType(string name, string text)
		{
			string str = "notExist";
			string[] newLine = new string[] { Environment.NewLine };
			string[] strArrays = text.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			while (num < (int)strArrays.Length)
			{
				string str1 = strArrays[num];
				string[] strArrays1 = new string[] { " ", "\t" };
				string[] strArrays2 = str1.Split(strArrays1, StringSplitOptions.RemoveEmptyEntries);
				if (!strArrays2[0].Equals(name))
				{
					if (strArrays2[0].Equals("[BINDINGS]"))
					{
						break;
					}
					num++;
				}
				else
				{
					str = strArrays2[1];
					break;
				}
			}
			return str;
		}

		private static ContentAttributesReader.TargetTypes FindTargetType(string type)
		{
			ContentAttributesReader.TargetTypes targetType = ContentAttributesReader.TargetTypes.String;
			string str = type;
			string str1 = str;
			if (str != null)
			{
				if (str1 == "CHARACTER")
				{
					targetType = ContentAttributesReader.TargetTypes.String;
					return targetType;
				}
				else if (str1 == "INTEGER")
				{
					targetType = ContentAttributesReader.TargetTypes.Integer;
					return targetType;
				}
				else
				{
					if (str1 != "FLOAT")
					{
						goto Label1;
					}
					targetType = ContentAttributesReader.TargetTypes.Double;
					return targetType;
				}
			}
			targetType = ContentAttributesReader.TargetTypes.NotExist;
			return targetType;
		Label1:
			if (str1 != "notExist")
			{
				targetType = ContentAttributesReader.TargetTypes.NotExist;
				return targetType;
			}
			else
			{
				targetType = ContentAttributesReader.TargetTypes.NotExist;
				return targetType;
			}
		}

		private static string GetContentAttributesGlobalText(string path, string fileName)
		{
			string empty = string.Empty;
			try
			{
				using (StreamReader streamReader = new StreamReader(string.Concat(path, fileName)))
				{
					empty = streamReader.ReadToEnd();
				}
			}
			catch (Exception exception)
			{
				exception.ToString();
			}
			return empty;
		}

		private static string GetReadableText(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] newLine = new string[] { Environment.NewLine };
			string[] strArrays = text.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				if (str.Contains(" ") || str.Contains("[BINDINGS]") || str.Contains("[ALL]"))
				{
					if (!str.Contains("\""))
					{
						stringBuilder.Append(str);
						stringBuilder.Append(Environment.NewLine);
					}
					else
					{
						string[] strArrays1 = str.Split(new char[] { '\"' });
						if ((int)strArrays1.Length == 3)
						{
							stringBuilder.Append(strArrays1[0]);
							stringBuilder.Append(strArrays1[2]);
							stringBuilder.Append(Environment.NewLine);
						}
						else if ((int)strArrays1.Length == 2)
						{
							stringBuilder.Append(strArrays1[0]);
							stringBuilder.Append(Environment.NewLine);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		public ContentAttributesReader.TargetTypes GetTargetType(string name)
		{
			string empty = string.Empty;
			ContentAttributesReader.TargetTypes targetType = ContentAttributesReader.TargetTypes.NotExist;
			TeklaStructuresSettings.GetAdvancedOption("XS_DIR", ref empty);
			empty = string.Concat(empty, "\\nt\\TplEd\\settings");
			if (name.Contains("EXTERNAL"))
			{
				for (int i = 2; targetType == ContentAttributesReader.TargetTypes.NotExist && i < this.attributesFilesTexts.Count; i++)
				{
					targetType = ContentAttributesReader.GetTargetType(this.attributesFilesTexts[i], name);
				}
			}
			else
			{
				if (!name.Contains("USERDEFINED"))
				{
					targetType = ContentAttributesReader.GetTargetType(this.attributesFilesTexts[0], name);
				}
				if (targetType == ContentAttributesReader.TargetTypes.NotExist)
				{
					targetType = ContentAttributesReader.GetTargetType(this.attributesFilesTexts[1], name);
				}
			}
			return targetType;
		}

		private static ContentAttributesReader.TargetTypes GetTargetType(string text, string name)
		{
			if (name.Contains(".") && ContentAttributesReader.IsNameValid(text, name))
			{
				string[] strArrays = new string[] { "." };
				string[] strArrays1 = name.Split(strArrays, StringSplitOptions.None);
				name = strArrays1[(int)strArrays1.Length - 1];
			}
			return ContentAttributesReader.FindTargetType(ContentAttributesReader.FindAttributeAndType(name, text));
		}

		private static string GetTextWithoutComments(string text, bool withoutEmptyLines = false)
		{
			string empty = string.Empty;
			string str = "/\\*(.*?)\\*/";
			string str1 = "//(.*?)\\r?\\n";
			string str2 = "\"((\\\\[^\\n]|[^\"\\n])*)\"";
			string str3 = "@(\"[^\"]*\")+";
			string str4 = text;
			string[] strArrays = new string[] { str, "|", str1, "|", str2, "|", str3 };
			empty = Regex.Replace(str4, string.Concat(strArrays), (Match line) => {
				if (!line.Value.StartsWith("/*") && !line.Value.StartsWith("//"))
				{
					return line.Value;
				}
				if (!line.Value.StartsWith("//"))
				{
					return string.Empty;
				}
				return Environment.NewLine;
			}, RegexOptions.Singleline);
			if (withoutEmptyLines)
			{
				empty = Regex.Replace(empty, "^\\s+$[\\r\\n]*", string.Empty, RegexOptions.Multiline);
			}
			return empty;
		}

		private static bool IsNameValid(string text, string name)
		{
			bool flag = false;
			bool flag1 = false;
			string[] strArrays = new string[] { " ", "\t" };
			string[] newLine = new string[] { Environment.NewLine };
			string[] strArrays1 = text.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)strArrays1.Length; i++)
			{
				string str = strArrays1[i];
				if (str.Contains("[BINDINGS]"))
				{
					flag1 = true;
				}
				if (flag1 && !str.Contains("[ALL]"))
				{
					string[] strArrays2 = str.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
					if ((int)strArrays2.Length >= 3 && strArrays2[2].Equals(name))
					{
						flag = true;
						break;
					}
				}
				else if (str.Contains("[ALL]"))
				{
					break;
				}
			}
			return flag;
		}

		public enum TargetTypes
		{
			String,
			Integer,
			Double,
			Date,
			NotExist
		}
	}
}