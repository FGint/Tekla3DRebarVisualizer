using System;
using System.Configuration;
using System.IO;

namespace cs_net_lib
{
	public class Settings
	{
		private string file = "configuration.xml";

		private Configuration configuration;

		private bool doReplaceDecimalChar;

		public string FileName
		{
			get
			{
				return this.file;
			}
			set
			{
				this.file = value;
			}
		}

		public Settings()
		{
			this.CheckDecimalChar();
		}

		public Settings(string file)
		{
			this.file = file;
			this.CheckDecimalChar();
		}

		private void CheckDecimalChar()
		{
			if (string.Concat(new decimal(1, 0, 0, false, 1))[1] == ',')
			{
				this.doReplaceDecimalChar = true;
			}
		}

		private string DoubleToString(double data)
		{
			string str = data.ToString();
			if (this.doReplaceDecimalChar)
			{
				str = str.Replace(',', '.');
			}
			return str;
		}

		public void Open()
		{
			try
			{
				ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap()
				{
					ExeConfigFilename = this.file
				};
				this.configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
			}
			catch (Exception exception)
			{
				try
				{
					if (File.Exists(this.file))
					{
						File.Delete(this.file);
						this.Open();
					}
				}
				catch
				{
				}
			}
		}

		public string Read(string name, string defaultValue)
		{
			return this.ReadString(name, defaultValue);
		}

		public int Read(string name, int defaultValue)
		{
			return int.Parse(this.ReadString(name, defaultValue.ToString()));
		}

		public bool Read(string name, bool defaultValue)
		{
			return bool.Parse(this.ReadString(name, defaultValue.ToString()));
		}

		public double Read(string name, double defaultValue)
		{
			return this.StringToDouble(this.ReadString(name, this.DoubleToString(defaultValue)));
		}

		private string ReadString(string name, string defaultValue)
		{
			if (this.configuration != null)
			{
				KeyValueConfigurationElement item = this.configuration.AppSettings.Settings[name];
				if (item != null)
				{
					return item.Value;
				}
			}
			return defaultValue;
		}

		public void Save()
		{
			if (this.configuration != null)
			{
				try
				{
					this.configuration.Save();
				}
				catch
				{
					this.Open();
					this.configuration.Save();
				}
			}
		}

		private double StringToDouble(string data)
		{
			if (this.doReplaceDecimalChar)
			{
				data = data.Replace('.', ',');
			}
			return double.Parse(data);
		}

		public void Write(string name, string value)
		{
			this.WriteString(name, value);
		}

		public void Write(string name, int value)
		{
			this.WriteString(name, value.ToString());
		}

		public void Write(string name, bool value)
		{
			this.WriteString(name, value.ToString());
		}

		public void Write(string name, double value)
		{
			this.WriteString(name, this.DoubleToString(value));
		}

		private void WriteString(string name, string value)
		{
			if (this.configuration != null)
			{
				KeyValueConfigurationElement item = this.configuration.AppSettings.Settings[name];
				if (item != null)
				{
					item.Value = value;
					return;
				}
				this.configuration.AppSettings.Settings.Add(name, value);
			}
		}
	}
}